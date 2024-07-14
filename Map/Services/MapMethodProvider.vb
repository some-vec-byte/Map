Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Linq

''' <summary>
''' Eine Klasse, die Methoden zur Erstellung und Verwaltung von Mapping-Methoden bereitstellt.
''' </summary>
Public Class MapMethodProvider
    Implements IMapMethodProvider

    ''' <summary>
    ''' Die Assembly
    ''' </summary>
    Private ReadOnly Property AssmBuilder As AssemblyBuilder

    ''' <summary>
    ''' Das Modul in der Assembly.
    ''' </summary>
    Private ReadOnly Property ModuleBuilder As ModuleBuilder

    ''' <summary>
    ''' Enthält bereits erstellte Methoden.
    ''' </summary>
    Private ReadOnly Property Methods As Dictionary(Of MethodKey, MethodInfo)

    ''' <summary>
    ''' Der Name der Assembly.
    ''' </summary>
    Private ReadOnly Property AsmName As String = "FakeGen"

    ''' <summary>
    ''' Initialisiert eine neue Instanz der <see cref="MapMethodProvider"/> Klasse.
    ''' </summary>
    Public Sub New()
        Me.Methods = New Dictionary(Of MethodKey, MethodInfo)()
        Dim assemblyName As AssemblyName = New AssemblyName(AsmName)
        Me.AssmBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
        Me.ModuleBuilder = Me.AssmBuilder.DefineDynamicModule(assemblyName.Name)
    End Sub


    Public Function GetOrCreate(Of F, T As {Class, New})(flags As MapperFlags, ParamArray toIgnore() As Expression(Of PropertySelector(Of F))) As MethodInfo Implements IMapMethodProvider.GetOrCreate
        Dim fromType = GetType(F)
        Dim toType = GetType(T)

        Dim key = New MethodKey(fromType, toType)
        Dim methodInfo As MethodInfo = Nothing

        If (flags And MapperFlags.Rebuild) <> MapperFlags.Rebuild AndAlso Methods.TryGetValue(key, methodInfo) Then
            Return methodInfo
        End If

        Dim propsToIgnore = ParseExpressions(Of F)(toIgnore)
        methodInfo = CreateMethod(fromType, toType, flags, propsToIgnore)
        Methods(key) = methodInfo

        Return Methods(key)
    End Function

    ''' <summary>
    ''' Erstellt eine neue Mapping-Methode basierend auf den angegebenen Typen, Flags und zu ignorierenden Eigenschaften.
    ''' </summary>
    ''' <param name="fromType">Der Quelltyp für das Mapping.</param>
    ''' <param name="toType">Der Zieltyp für das Mapping.</param>
    ''' <param name="flags">Flags, die das Verhalten des Mapping-Prozesses steuern.</param>
    ''' <param name="propsToIgnore">Eine Liste von Eigenschaftsnamen des Quelltyps, die beim Mapping ignoriert werden sollen.</param>
    ''' <returns>Eine MethodInfo, die das Mapping von <paramref name="fromType"/> zu <paramref name="toType"/> durchführt.</returns>
    Private Function CreateMethod(fromType As Type, toType As Type, flags As MapperFlags, propsToIgnore As List(Of String)) As MethodInfo
        ' public class definieren
        Dim typeBuilder = Me.ModuleBuilder.DefineType($"DynMapper_{Guid.NewGuid()}", TypeAttributes.Public)

        ' methode definieren:
        ' Public Shared Function Map(fromType as FromType) as ToType
        Dim methodBuilder = typeBuilder.DefineMethod("Map", MethodAttributes.Public Or MethodAttributes.Static, toType, New Type() {fromType})

        Dim ilGen = methodBuilder.GetILGenerator()

        ' OpCodes für Methode hinzufügen
        ' ToType über einen leeren Konstruktor als Objekt erstellen
        ilGen.Emit(OpCodes.Newobj, toType.GetConstructor(Type.EmptyTypes))

        Dim propsToIter = fromType.GetProperties().
                                 Where(Function(info As PropertyInfo) propsToIgnore.Contains(info.Name) = False).
                                 ToArray()


        ' alle Eigenschaften von ToType iterieren und FromType hinzufügen
        For Each fromProp In propsToIter

            Dim toProp = toType.GetProperty(fromProp.Name)
            If toProp Is Nothing Then
                If (flags And MapperFlags.IgnoreMissing) <> MapperFlags.IgnoreMissing Then
                    Throw New MissingMemberException()
                Else
                    Continue For
                End If
            End If

            ' die Referenz zu dem neuen Objekt mit dem Typ ToType auf dem Stapel duplizieren
            ilGen.Emit(OpCodes.Dup)
            ' läd das erste Argument der neuen Methode auf den Stapel (FromType)
            ilGen.Emit(OpCodes.Ldarg_0)
            ' ruft die GetMethode von FromType auf
            ' entfernt FromType vom Stapel und fügt die Eigenschaft dem Stapel hinzu
            ilGen.Emit(OpCodes.Callvirt, fromProp.GetMethod)
            ' ruft den Setter von unserem neuen Objekt über die duplizierte referenz auf mit der eigenschaft auf und entfernt beide vom stack
            ilGen.Emit(OpCodes.Callvirt, toProp.SetMethod)

        Next
        ' Return des neuen Objektes, Abschluss der Methode
        ilGen.Emit(OpCodes.Ret)

        Dim createdType = typeBuilder.CreateType()

        Return createdType.GetMethod("Map", BindingFlags.Public Or BindingFlags.Static, New Type() {fromType})
    End Function

    ''' <summary>
    ''' Analysiert die angegebenen Ausdrücke, um eine Liste von Eigenschaftsnamen zu extrahieren, die beim Mapping ignoriert werden sollen.
    ''' </summary>
    ''' <typeparam name="F">Der Typ, von dem die Eigenschaften ignoriert werden sollen.</typeparam>
    ''' <param name="toIgnore">Die Ausdrücke, die die zu ignorierenden Eigenschaften repräsentieren.</param>
    ''' <returns>Eine Liste von Eigenschaftsnamen, die beim Mapping ignoriert werden sollen.</returns>
    Private Function ParseExpressions(Of F)(ParamArray toIgnore() As Expression(Of PropertySelector(Of F))) As List(Of String)
        Dim result = New List(Of String)

        If toIgnore Is Nothing OrElse toIgnore.Length = 0 Then
            Return result
        End If

        For Each exp In toIgnore
            Dim mem As MemberExpression = TryCast(exp.Body, MemberExpression)
            If mem IsNot Nothing Then
                result.Add(mem.Member.Name)
                Continue For
            End If

            Dim ue As UnaryExpression = TryCast(exp.Body, UnaryExpression)
            If ue IsNot Nothing Then
                Dim ueMem As MemberExpression = TryCast(ue.Operand, MemberExpression)
                If ueMem IsNot Nothing Then
                    result.Add(ueMem.Member.Name)
                End If
            End If
        Next

        Return result
    End Function



End Class
