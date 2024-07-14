''' <summary>
''' Definiert Flags, die das Verhalten des Mapping-Prozesses steuern.
''' </summary>
<Flags>
Public Enum MapperFlags
    ''' <summary>
    ''' Wirft eine Ausnahme, wenn eine entsprechende Eigenschaft im Zieltyp fehlt.
    ''' </summary>
    ThrowOnMissing = 1
    ''' <summary>
    ''' Ignoriert fehlende Eigenschaften im Zieltyp.
    ''' </summary>
    IgnoreMissing = 2
    ''' <summary>
    ''' Erzwingt den Neuaufbau des Zielobjekts, auch wenn es bereits existiert.
    ''' </summary>
    Rebuild = 3
End Enum