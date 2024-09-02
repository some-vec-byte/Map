using System.Linq.Expressions;

namespace Map
{

    /// <summary>
    /// Eine Implementierung von <see cref="IMapper"/>, die dynamisches Mapping zwischen Objekttypen ermöglicht.
    /// </summary>
    public class DynamicMapper : IMapper
    {

        /// <summary>
        /// Stellt die Map Methoden bereit
        /// </summary>
        private readonly IMapMethodProvider MethodProvider;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="DynamicMapper"/> Klasse.
        /// </summary>
        /// <param name="methodProvider">Eine Instanz von <see cref="IMapMethodProvider"/>, die Methoden zur Durchführung des Mappings bereitstellt.</param>
        public DynamicMapper(IMapMethodProvider methodProvider)
        {
            MethodProvider = methodProvider;
        }

        /// <summary>
        /// Mappt ein Objekt vom Typ <typeparamref name="TFrom"/> auf ein neues Objekt vom Typ <typeparamref name="TTarget"/>.
        /// Wirft einen Fehler wenn eine Eigenschaft nicht vorhanden ist.
        /// </summary>
        /// <typeparam name="TFrom">Der Quelltyp.</typeparam>
        /// <typeparam name="TTarget">Der Zieltyp.</typeparam>
        /// <param name="from">Das Quellobjekt.</param>
        /// <returns>Ein neues Objekt vom Typ <typeparamref name="TTarget"/>.</returns>
        public TTarget Map<TFrom, TTarget>(TFrom @from)
                where TFrom : class
                where TTarget : class, new()
        {
            return Map<TFrom, TTarget>(from, MapperFlags.ThrowOnMissing);
        }

        /// <summary>
        /// Mappt ein Objekt vom Typ <typeparamref name="TFrom"/> auf ein neues Objekt vom Typ <typeparamref name="TTarget"/> und ignoriert dabei bestimmte Eigenschaften.
        /// Wirft einen Fehler wenn eine Eigenschaft nicht vorhanden ist.
        /// </summary>
        /// <typeparam name="TFrom">Der Quelltyp.</typeparam>
        /// <typeparam name="TTarget">Der Zieltyp.</typeparam>
        /// <param name="from">Das Quellobjekt.</param>
        /// <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen.</param>
        /// <returns>Ein neues Objekt vom Typ <typeparamref name="TTarget"/>.</returns>
        public TTarget Map<TFrom, TTarget>(TFrom @from, params Expression<PropertySelector<TFrom>>[] ignore)
                where TFrom : class
                where TTarget : class, new()
        {
            return Map<TFrom, TTarget>(from, MapperFlags.ThrowOnMissing, ignore);
        }

        /// <summary>
        /// Mappt ein Objekt vom Typ <typeparamref name="TFrom"/> auf ein neues Objekt vom Typ <typeparamref name="TTarget"/> unter Verwendung spezifischer Flags und ignoriert dabei bestimmte Eigenschaften.
        /// </summary>
        /// <typeparam name="TFrom">Der Quelltyp.</typeparam>
        /// <typeparam name="TTarget">Der Zieltyp.</typeparam>
        /// <param name="from">Das Quellobjekt.</param>
        /// <param name="flags">Flags, die das Verhalten des Mapping-Prozesses steuern.</param>
        /// <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen.</param>
        /// <returns>Ein neues Objekt vom Typ <typeparamref name="TTarget"/>.</returns>
        public TTarget Map<TFrom, TTarget>(TFrom @from, MapperFlags flags, params Expression<PropertySelector<TFrom>>[] ignore)
                where TFrom : class
                where TTarget : class, new()
        {
            var methodInfo = MethodProvider.GetOrCreate<TFrom, TTarget>(flags, ignore);

            return (TTarget)methodInfo.Invoke(null, new object[] { from });
        }
    }
}