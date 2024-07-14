Imports System.Linq.Expressions

''' <summary>
''' Eine Implementierung von <see cref="IMapper"/>, die dynamisches Mapping zwischen Objekttypen ermöglicht.
''' </summary>
Public Class DynamicMapper
    Implements IMapper

    ''' <summary>
    ''' Stellt die Map Methoden bereit
    ''' </summary>
    Private ReadOnly MethodProvider As IMapMethodProvider

    ''' <summary>
    ''' Initialisiert eine neue Instanz der <see cref="DynamicMapper"/> Klasse.
    ''' </summary>
    ''' <param name="methodProvider">Eine Instanz von <see cref="IMapMethodProvider"/>, die Methoden zur Durchführung des Mappings bereitstellt.</param>
    Public Sub New(methodProvider As IMapMethodProvider)
        Me.MethodProvider = methodProvider
    End Sub

    ''' <summary>
    ''' Mappt ein Objekt vom Typ <typeparamref name="F"/> auf ein neues Objekt vom Typ <typeparamref name="T"/>.
    ''' Wirft einen Fehler wenn eine Eigenschaft nicht vorhanden ist.
    ''' </summary>
    ''' <typeparam name="F">Der Quelltyp.</typeparam>
    ''' <typeparam name="T">Der Zieltyp.</typeparam>
    ''' <param name="from">Das Quellobjekt.</param>
    ''' <returns>Ein neues Objekt vom Typ <typeparamref name="T"/>.</returns>
    Public Function Map(Of F As Class, T As {Class, New})(from As F) As T Implements IMapper.Map
        Return Map(Of F, T)(from, MapperFlags.ThrowOnMissing)
    End Function

    ''' <summary>
    ''' Mappt ein Objekt vom Typ <typeparamref name="F"/> auf ein neues Objekt vom Typ <typeparamref name="T"/> und ignoriert dabei bestimmte Eigenschaften.
    ''' Wirft einen Fehler wenn eine Eigenschaft nicht vorhanden ist.
    ''' </summary>
    ''' <typeparam name="F">Der Quelltyp.</typeparam>
    ''' <typeparam name="T">Der Zieltyp.</typeparam>
    ''' <param name="from">Das Quellobjekt.</param>
    ''' <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen.</param>
    ''' <returns>Ein neues Objekt vom Typ <typeparamref name="T"/>.</returns>
    Public Function Map(Of F As Class, T As {Class, New})(from As F, ParamArray ignore() As Expression(Of PropertySelector(Of F))) As T Implements IMapper.Map
        Return Map(Of F, T)(from, MapperFlags.ThrowOnMissing, ignore)
    End Function

    ''' <summary>
    ''' Mappt ein Objekt vom Typ <typeparamref name="F"/> auf ein neues Objekt vom Typ <typeparamref name="T"/> unter Verwendung spezifischer Flags und ignoriert dabei bestimmte Eigenschaften.
    ''' </summary>
    ''' <typeparam name="F">Der Quelltyp.</typeparam>
    ''' <typeparam name="T">Der Zieltyp.</typeparam>
    ''' <param name="from">Das Quellobjekt.</param>
    ''' <param name="flags">Flags, die das Verhalten des Mapping-Prozesses steuern.</param>
    ''' <param name="ignore">Eine Liste von Eigenschaften des Quellobjekts, die beim Mapping ignoriert werden sollen.</param>
    ''' <returns>Ein neues Objekt vom Typ <typeparamref name="T"/>.</returns>
    Public Function Map(Of F As Class, T As {Class, New})(from As F, flags As MapperFlags, ParamArray ignore() As Expression(Of PropertySelector(Of F))) As T Implements IMapper.Map
        Dim methodInfo = MethodProvider.GetOrCreate(Of F, T)(flags, ignore)

        Return methodInfo.Invoke(Nothing, New Object() {from})
    End Function
End Class