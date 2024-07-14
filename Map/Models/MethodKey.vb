''' <summary>
''' Eine Klasse welche einen Schlüssel für eine MapMethode in einem Dictionary darstellt.
''' </summary>
Friend Class MethodKey
    Inherits Tuple(Of Type, Type)

    ''' <summary>
    ''' Erstellt einen neue Instanz der Klasse.
    ''' </summary>
    ''' <param name="item1">Der Typ F</param>
    ''' <param name="item2">Der Typ T</param>
    Public Sub New(item1 As Type, item2 As Type)
        MyBase.New(item1, item2)
    End Sub
End Class
