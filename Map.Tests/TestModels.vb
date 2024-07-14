Imports NUnit.Framework

Public Class EmptyTestModelA

    Public Sub AssertEquals(model As EmptyTestModelB)
        Assert.That(model, [Is].Not.Null)
        Assert.That(Me, [Is].Not.Null)
    End Sub

    Public Sub AssertEquals(model As EmptyTestModelA)
        Assert.That(model, [Is].Not.Null)
        Assert.That(Me, [Is].Not.Null)
    End Sub
End Class

Public Class EmptyTestModelB

End Class

Public Class SimpleTestModelA

    Public Property Name As String

    Public Property Value As Integer

    Public Property ValueObj As Object

    Public Sub New(name As String, value As Integer, valueObj As Object)
        Me.Name = name
        Me.Value = value
        Me.ValueObj = valueObj
    End Sub

    Public Sub AssertEquals(model As SimpleTestModelB)
        Assert.That(Me.Name, [Is].EqualTo(model.Name))
        Assert.That(Me.Value, [Is].EqualTo(model.Value))
        Assert.That(Me.ValueObj, [Is].EqualTo(model.ValueObj))
    End Sub

    Public Sub AssertEquals(model As SimpleTestModelA)
        Assert.That(Me.Name, [Is].EqualTo(model.Name))
        Assert.That(Me.Value, [Is].EqualTo(model.Value))
        Assert.That(Me.ValueObj, [Is].EqualTo(model.ValueObj))
    End Sub
End Class

Public Class SimpleTestModelB

    Public Property Name As String

    Public Property Value As Integer

    Public Property ValueObj As Object
    Public Sub New()
    End Sub
    Public Sub New(name As String, value As Integer, valueObj As Object)
        Me.Name = name
        Me.Value = value
        Me.ValueObj = valueObj
    End Sub

End Class


Public Class ComplexTestModelA

    Public Property EmptyTestModel As EmptyTestModelA


    Public Property SimpleTestModel As SimpleTestModelA


    Public Property Name As String

    Public Property Value As Integer

    Public Sub New()
    End Sub
    Public Sub New(emptyTestModel As EmptyTestModelA, simpleTestModel As SimpleTestModelA, name As String, value As Integer)
        Me.EmptyTestModel = emptyTestModel
        Me.SimpleTestModel = simpleTestModel
        Me.Name = name
        Me.Value = value
    End Sub



    Public Sub AssertEquals(model As ComplexTestModelB)
        Me.EmptyTestModel.AssertEquals(model.EmptyTestModel)
        Me.SimpleTestModel.AssertEquals(model.SimpleTestModel)
        Assert.That(Me.Name, [Is].EqualTo(model.Name))
        Assert.That(Me.Value, [Is].EqualTo(model.Value))
    End Sub

End Class

Public Class ComplexTestModelB
    Public Property EmptyTestModel As EmptyTestModelA


    Public Property SimpleTestModel As SimpleTestModelA


    Public Property Name As String

    Public Property Value As Integer

    Public Sub New()
    End Sub
    Public Sub New(emptyTestModel As EmptyTestModelA, simpleTestModel As SimpleTestModelA, name As String, value As Integer)
        Me.EmptyTestModel = emptyTestModel
        Me.SimpleTestModel = simpleTestModel
        Me.Name = name
        Me.Value = value
    End Sub
End Class

Public Class ComplexMissingPropertyTestModelA

    Public Property EmptyTestModel As EmptyTestModelA


    Public Property SimpleTestModel As SimpleTestModelA


    Public Property Name As String

    Public Property Value As Integer

    Public Sub New()
    End Sub
    Public Sub New(emptyTestModel As EmptyTestModelA, simpleTestModel As SimpleTestModelA, name As String, value As Integer)
        Me.EmptyTestModel = emptyTestModel
        Me.SimpleTestModel = simpleTestModel
        Me.Name = name
        Me.Value = value
    End Sub


End Class

Public Class ComplexMissingPropertyTestModelB

    Public Property EmptyTestModel As EmptyTestModelA


    Public Property SimpleTestModel As SimpleTestModelA


    Public Property Name As String


    Public Sub New()
    End Sub
    Public Sub New(emptyTestModel As EmptyTestModelA, simpleTestModel As SimpleTestModelA, name As String, value As Integer)
        Me.EmptyTestModel = emptyTestModel
        Me.SimpleTestModel = simpleTestModel
        Me.Name = name
    End Sub
End Class