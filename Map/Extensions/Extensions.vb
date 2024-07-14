Imports System.Runtime.CompilerServices
Imports Microsoft.Extensions.DependencyInjection

Public Module Extensions
    <Extension>
    Public Function AddMapping(services As IServiceCollection) As IServiceCollection
        services.AddTransient(Of IMapMethodProvider, MapMethodProvider)
        services.AddTransient(Of IMapper, DynamicMapper)

        Return services
    End Function


    Public Delegate Function ValueProvider(Of TValue)() As TValue
    <Extension>
    Public Function GetOrCreate(Of TKey, TValue)(dict As Dictionary(Of TKey, TValue), key As TKey, provider As ValueProvider(Of TValue)) As TValue
        Dim value As TValue = Nothing

        If dict.TryGetValue(key, value) = False Then
            value = provider()
            dict(key) = value
        End If

        Return value
    End Function
End Module
