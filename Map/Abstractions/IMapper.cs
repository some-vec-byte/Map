using System.Linq.Expressions;

namespace Map
{

    /// <summary>
    /// Definiert einen Delegaten, der eine Eigenschaft eines Typs F auswählt.
    /// </summary>
    /// <typeparam name="F">Der Typ des Objekts, von dem die Eigenschaft ausgewählt wird.</typeparam>
    /// <param name="f">Das Objekt, von dem die Eigenschaft ausgewählt wird.</param>
    /// <returns>Der Wert der ausgewählten Eigenschaft als Object.</returns>
    public delegate object PropertySelector<F>(F f);

    /// <summary>
    /// Stellt eine Schnittstelle für Mapping-Operationen zwischen zwei Objekttypen bereit.
    /// </summary>
    public interface IMapper
    {

        /// <summary>
        /// Mappt ein Objekt vom Typ F auf ein neues Objekt vom Typ T.
        /// </summary>
        /// <typeparam name="TFrom">Der Quelltyp.</typeparam>
        /// <typeparam name="T">Der Zieltyp.</typeparam>
        /// <param name="from">Das Quellobjekt.</param>
        /// <returns>Ein neues Objekt vom Typ T.</returns>
        TTarget Map<TFrom, TTarget>(TFrom @from)
                where TFrom : class
                where TTarget : class, new();

        /// <summary>
        /// Mappt ein Objekt vom Typ F auf ein neues Objekt vom Typ T und ignoriert dabei bestimmte Eigenschaften.
        /// </summary>
        /// <typeparam name="TFrom">Der Quelltyp.</typeparam>
        /// <typeparam name="TTarget">Der Zieltyp.</typeparam>
        /// <param name="from">Das Quellobjekt.</param>
        /// <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen. Unterstützt Eigenschaften von Unterobjekten.</param>
        /// <returns>Ein neues Objekt vom Typ T.</returns>
        TTarget Map<TFrom, TTarget>(TFrom @from, params Expression<PropertySelector<TFrom>>[] ignore)
                where TFrom : class
                where TTarget : class, new();

        /// <summary>
        /// Mappt ein Objekt vom Typ F auf ein neues Objekt vom Typ T unter Verwendung spezifischer Flags und ignoriert dabei bestimmte Eigenschaften.
        /// </summary>
        /// <typeparam name="TFrom">Der Quelltyp.</typeparam>
        /// <typeparam name="TTarget">Der Zieltyp.</typeparam>
        /// <param name="from">Das Quellobjekt.</param>
        /// <param name="flags">Flags, die das Verhalten des Mapping-Prozesses steuern.</param>
        /// <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen. Unterstützt Eigenschaften von Unterobjekten.</param>
        /// <returns>Ein neues Objekt vom Typ T.</returns>
        TTarget Map<TFrom, TTarget>(TFrom @from, MapperFlags flags, params Expression<PropertySelector<TFrom>>[] ignore)
                where TFrom : class
                where TTarget : class, new();
    }
}