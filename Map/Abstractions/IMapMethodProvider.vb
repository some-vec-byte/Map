Imports System.Linq.Expressions
Imports System.Reflection

''' <summary>
''' Definiert eine Schnittstelle welche eine Map Methode bereitstellt.
''' </summary>
Public Interface IMapMethodProvider

    ''' <summary>
    ''' Erstellt oder gibt eine bereits erstellte "Map" Methode zurück.
    ''' Die Methode hat folgende Signatur: 
    ''' public shared Function Map(Of <typeparamref name="F"/>, <typeparamref name="T"/>)(<typeparamref name="F"/> from) as <typeparamref name="T"/>
    ''' </summary>
    ''' <typeparam name="F">Der Typ des Quellobjekts.</typeparam>
    ''' <typeparam name="T">Der Typ des Zielobjekts.</typeparam>
    ''' <param name="flags">Die Konfiguration der Methode.</param>
    ''' <param name="toIgnore">Eine Expression mit Eigenschaften von <typeparamref name="F"/> die ignoriert werden sollen.</param>
    ''' <returns>Die erstellte Methode zur Umwandlung von <typeparamref name="F"/> in <typeparamref name="T"/>.</returns>
    Function GetOrCreate(Of F, T As {Class, New})(flags As MapperFlags, ParamArray toIgnore() As Expression(Of PropertySelector(Of F))) As MethodInfo

End Interface
