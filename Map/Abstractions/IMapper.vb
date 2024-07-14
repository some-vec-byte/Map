Imports System.Linq.Expressions

''' <summary>
''' Definiert einen Delegaten, der eine Eigenschaft eines Typs F auswählt.
''' </summary>
''' <typeparam name="F">Der Typ des Objekts, von dem die Eigenschaft ausgewählt wird.</typeparam>
''' <param name="f">Das Objekt, von dem die Eigenschaft ausgewählt wird.</param>
''' <returns>Der Wert der ausgewählten Eigenschaft als Object.</returns>
Public Delegate Function PropertySelector(Of F)(f As F) As Object

''' <summary>
''' Stellt eine Schnittstelle für Mapping-Operationen zwischen zwei Objekttypen bereit.
''' </summary>
Public Interface IMapper

    ''' <summary>
    ''' Mappt ein Objekt vom Typ F auf ein neues Objekt vom Typ T.
    ''' </summary>
    ''' <typeparam name="F">Der Quelltyp.</typeparam>
    ''' <typeparam name="T">Der Zieltyp.</typeparam>
    ''' <param name="from">Das Quellobjekt.</param>
    ''' <returns>Ein neues Objekt vom Typ T.</returns>
    Function Map(Of F As Class, T As {Class, New})(from As F) As T

    ''' <summary>
    ''' Mappt ein Objekt vom Typ F auf ein neues Objekt vom Typ T und ignoriert dabei bestimmte Eigenschaften.
    ''' </summary>
    ''' <typeparam name="F">Der Quelltyp.</typeparam>
    ''' <typeparam name="T">Der Zieltyp.</typeparam>
    ''' <param name="from">Das Quellobjekt.</param>
    ''' <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen. Unterstützt Eigenschaften von Unterobjekten.</param>
    ''' <returns>Ein neues Objekt vom Typ T.</returns>
    Function Map(Of F As Class, T As {Class, New})(from As F, ParamArray ignore() As Expression(Of PropertySelector(Of F))) As T

    ''' <summary>
    ''' Mappt ein Objekt vom Typ F auf ein neues Objekt vom Typ T unter Verwendung spezifischer Flags und ignoriert dabei bestimmte Eigenschaften.
    ''' </summary>
    ''' <typeparam name="F">Der Quelltyp.</typeparam>
    ''' <typeparam name="T">Der Zieltyp.</typeparam>
    ''' <param name="from">Das Quellobjekt.</param>
    ''' <param name="flags">Flags, die das Verhalten des Mapping-Prozesses steuern.</param>
    ''' <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen. Unterstützt Eigenschaften von Unterobjekten.</param>
    ''' <returns>Ein neues Objekt vom Typ T.</returns>
    Function Map(Of F As Class, T As {Class, New})(from As F, flags As MapperFlags, ParamArray ignore() As Expression(Of PropertySelector(Of F))) As T
End Interface
