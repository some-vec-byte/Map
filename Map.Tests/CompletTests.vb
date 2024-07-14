Imports NUnit.Framework

Namespace Map.Tests

    Public Class Tests

        <Test>
        Public Sub Test_Empty()
            Dim methodProvider As IMapMethodProvider = New MapMethodProvider()
            Dim mapper As IMapper = New DynamicMapper(methodProvider)

            Dim emptyA = New EmptyTestModelA()

            Dim emptyB As EmptyTestModelB = mapper.Map(Of EmptyTestModelA, EmptyTestModelB)(emptyA)

            emptyA.AssertEquals(emptyB)

        End Sub
        <Test>
        Public Sub Test_Simple()
            Dim methodProvider As IMapMethodProvider = New MapMethodProvider()
            Dim mapper As IMapper = New DynamicMapper(methodProvider)

            Dim simpleA = New SimpleTestModelA("Test", 1, 2)

            Dim simpleB As SimpleTestModelB = mapper.Map(Of SimpleTestModelA, SimpleTestModelB)(simpleA)

            simpleA.AssertEquals(simpleB)
        End Sub

        <Test>
        Public Sub Test_Complex()
            Dim methodProvider As IMapMethodProvider = New MapMethodProvider()
            Dim mapper As IMapper = New DynamicMapper(methodProvider)

            Dim complexA = New ComplexTestModelA(New EmptyTestModelA(), New SimpleTestModelA("Test", 1, 2), "Test2", 3)
            Dim complexB = mapper.Map(Of ComplexTestModelA, ComplexTestModelB)(complexA)

            complexA.AssertEquals(complexB)
            ' Eigenschaft Name ignorieren, aber kein Method Rebuild
            complexB = mapper.Map(Of ComplexTestModelA, ComplexTestModelB)(complexA, Function(m) m.Name)
            ' methode aus dem Cache sollte verwendet werden
            complexA.AssertEquals(complexB)
            '  Eigenschaft Name ignorieren und Methode neu bauen
            complexB = mapper.Map(Of ComplexTestModelA, ComplexTestModelB)(complexA, MapperFlags.Rebuild Or MapperFlags.ThrowOnMissing, Function(m) m.Name)

            complexA.SimpleTestModel.AssertEquals(complexB.SimpleTestModel)
            complexA.EmptyTestModel.AssertEquals(complexB.EmptyTestModel)
            Assert.That(complexA.Value, [Is].EqualTo(complexB.Value))
            Assert.That(complexA.Name, [Is].Not.EqualTo(complexB.Name))

        End Sub

        <Test>
        Public Sub Test_Complex_Missing()
            Dim methodProvider As IMapMethodProvider = New MapMethodProvider()
            Dim mapper As IMapper = New DynamicMapper(methodProvider)

            Dim complexMissingA = New ComplexMissingPropertyTestModelA(New EmptyTestModelA(), New SimpleTestModelA("Test", 1, 2), "Test2", 3)

            Assert.Throws(Of MissingMemberException)(Sub() mapper.Map(Of ComplexMissingPropertyTestModelA, ComplexMissingPropertyTestModelB)(complexMissingA))

            Dim complexMissingB = mapper.Map(Of ComplexMissingPropertyTestModelA, ComplexMissingPropertyTestModelB)(complexMissingA, MapperFlags.Rebuild Or MapperFlags.IgnoreMissing)

            complexMissingA.EmptyTestModel.AssertEquals(complexMissingB.EmptyTestModel)
            complexMissingA.SimpleTestModel.AssertEquals(complexMissingB.SimpleTestModel)
            Assert.That(complexMissingA.Name, [Is].EqualTo(complexMissingB.Name))


            complexMissingB = mapper.Map(Of ComplexMissingPropertyTestModelA, ComplexMissingPropertyTestModelB)(complexMissingA, MapperFlags.Rebuild Or MapperFlags.IgnoreMissing, Function(m) m.Name)

            complexMissingA.EmptyTestModel.AssertEquals(complexMissingB.EmptyTestModel)
            complexMissingA.SimpleTestModel.AssertEquals(complexMissingB.SimpleTestModel)
            Assert.That(complexMissingA.Name, [Is].Not.EqualTo(complexMissingB.Name))
        End Sub

        <Test>
        Public Sub Test_Multiple()
            Dim methodProvider As IMapMethodProvider = New MapMethodProvider()
            Dim mapper As IMapper = New DynamicMapper(methodProvider)

            Dim emptyA = New EmptyTestModelA()

            Dim emptyB As EmptyTestModelB = mapper.Map(Of EmptyTestModelA, EmptyTestModelB)(emptyA)

            emptyA.AssertEquals(emptyB)

            Dim simpleA = New SimpleTestModelA("Test", 1, 2)

            Dim simpleB As SimpleTestModelB = mapper.Map(Of SimpleTestModelA, SimpleTestModelB)(simpleA)

            simpleA.AssertEquals(simpleB)

            Dim complexA = New ComplexTestModelA(New EmptyTestModelA(), New SimpleTestModelA("Test", 1, 2), "Test2", 3)
            Dim complexB = mapper.Map(Of ComplexTestModelA, ComplexTestModelB)(complexA)

            complexA.AssertEquals(complexB)
        End Sub




    End Class

End Namespace