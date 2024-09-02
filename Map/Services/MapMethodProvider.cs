using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Map
{

    /// <summary>
    /// Eine Klasse, die Methoden zur Erstellung und Verwaltung von Mapping-Methoden bereitstellt.
    /// </summary>
    public class MapMethodProvider : IMapMethodProvider
    {

        /// <summary>
        /// Die Assembly
        /// </summary>
        private AssemblyBuilder AssmBuilder { get; set; }

        /// <summary>
        /// Das Modul in der Assembly.
        /// </summary>
        private ModuleBuilder ModuleBuilder { get; set; }

        /// <summary>
        /// Enthält bereits erstellte Methoden.
        /// </summary>
        private Dictionary<MethodKey, MethodInfo> Methods { get; set; }

        /// <summary>
        /// Der Name der Assembly.
        /// </summary>
        private const string ASM_NAME = "DynamicMapper";

        /// <summary>
        /// Der Name der Methode
        /// </summary>
        private const string METHOD_NAME = "Map";

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="MapMethodProvider"/> Klasse.
        /// </summary>
        public MapMethodProvider()
        {
            Methods = new Dictionary<MethodKey, MethodInfo>();
            var assemblyName = new AssemblyName(ASM_NAME);
            AssmBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = AssmBuilder.DefineDynamicModule(assemblyName.Name);
        }


        public MethodInfo GetOrCreate<TFrom, TTarget>(MapperFlags flags, params Expression<PropertySelector<TFrom>>[] toIgnore) where TTarget : class, new()
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTarget);

            var key = new MethodKey(fromType, toType);

            if ((flags & MapperFlags.Rebuild) != MapperFlags.Rebuild && Methods.TryGetValue(key, out var methodInfo))
            {
                return methodInfo;
            }

            var propsToIgnore = ParseExpressions(toIgnore);
            methodInfo = CreateMethod(fromType, toType, flags, propsToIgnore);
            Methods[key] = methodInfo;

            return Methods[key];
        }

        /// <summary>
        /// Erstellt eine neue Mapping-Methode basierend auf den angegebenen Typen, Flags und zu ignorierenden Eigenschaften.
        /// </summary>
        /// <param name="fromType">Der Quelltyp für das Mapping.</param>
        /// <param name="toType">Der Zieltyp für das Mapping.</param>
        /// <param name="flags">Flags, die das Verhalten des Mapping-Prozesses steuern.</param>
        /// <param name="propsToIgnore">Eine Liste von Eigenschaftsnamen des Quelltyps, die beim Mapping ignoriert werden sollen.</param>
        /// <returns>Eine MethodInfo, die das Mapping von <paramref name="fromType"/> zu <paramref name="toType"/> durchführt.</returns>
        private MethodInfo CreateMethod(Type fromType, Type toType, MapperFlags flags, List<string> propsToIgnore)
        {
            // public class definieren
            var typeBuilder = ModuleBuilder.DefineType($"DynMapper_{Guid.NewGuid()}", TypeAttributes.Public);

            // methode definieren:
            // Public Shared Function Map(fromType as FromType) as ToType
            var methodBuilder = typeBuilder.DefineMethod(METHOD_NAME, MethodAttributes.Public | MethodAttributes.Static, toType, new Type[] { fromType });

            var ilGen = methodBuilder.GetILGenerator();

            // OpCodes für Methode hinzufügen
            // ToType über einen leeren Konstruktor als Objekt erstellen
            ilGen.Emit(OpCodes.Newobj, toType.GetConstructor(Type.EmptyTypes));

            // Properties filtern
            PropertyInfo[] propsToIter = fromType.GetProperties().Where((info) => propsToIgnore.Contains(info.Name) == false).ToArray();

            // alle Eigenschaften von FromType iterieren und ToType hinzufügen
            foreach (var fromProp in propsToIter)
            {

                var toProp = toType.GetProperty(fromProp.Name);
                if (toProp is null)
                {
                    if ((flags & MapperFlags.IgnoreMissing) != MapperFlags.IgnoreMissing)
                    {
                        throw new MissingMemberException();
                    }
                    continue;
                }

                // die Referenz zu dem neuen Objekt mit dem Typ ToType auf dem Stapel duplizieren
                ilGen.Emit(OpCodes.Dup);
                // läd das erste Argument der neuen Methode auf den Stapel (FromType)
                ilGen.Emit(OpCodes.Ldarg_0);
                // ruft die GetMethode von FromType auf
                // entfernt FromType vom Stapel und fügt die Eigenschaft dem Stapel hinzu
                ilGen.Emit(OpCodes.Callvirt, fromProp.GetMethod);
                // ruft den Setter von unserem neuen Objekt über die duplizierte referenz auf mit der eigenschaft auf und entfernt beide vom stack
                ilGen.Emit(OpCodes.Callvirt, toProp.SetMethod);

            }
            // Return des neuen Objektes, Abschluss der Methode
            ilGen.Emit(OpCodes.Ret);

            // Klasse erstellen
            var createdType = typeBuilder.CreateType();

            return createdType.GetMethod(METHOD_NAME, BindingFlags.Public | BindingFlags.Static, new Type[] { fromType });
        }

        /// <summary>
        /// Analysiert die angegebenen Ausdrücke, um eine Liste von Eigenschaftsnamen zu extrahieren, die beim Mapping ignoriert werden sollen.
        /// </summary>
        /// <typeparam name="F">Der Typ, von dem die Eigenschaften ignoriert werden sollen.</typeparam>
        /// <param name="toIgnore">Die Ausdrücke, die die zu ignorierenden Eigenschaften repräsentieren.</param>
        /// <returns>Eine Liste von Eigenschaftsnamen, die beim Mapping ignoriert werden sollen.</returns>
        private static List<string> ParseExpressions<F>(params Expression<PropertySelector<F>>[] toIgnore)
        {
            var result = new List<string>();

            if (toIgnore is null || toIgnore.Length == 0)
            {
                return result;
            }

            foreach (var exp in toIgnore)
            {
                MemberExpression mem = exp.Body as MemberExpression;
                if (mem is not null)
                {
                    result.Add(mem.Member.Name);
                    continue;
                }

                UnaryExpression ue = exp.Body as UnaryExpression;
                if (ue is not null)
                {
                    MemberExpression ueMem = ue.Operand as MemberExpression;
                    if (ueMem is not null)
                    {
                        result.Add(ueMem.Member.Name);
                    }
                }
            }

            return result;
        }
    }
}