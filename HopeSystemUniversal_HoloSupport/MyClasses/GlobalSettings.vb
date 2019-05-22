
Public Class GlobalSettings
    Public Shared Property AzureDataMarketClientId As String = "Deaf"
    Public Shared Property AzureDataMarketClientSecret As String = "gazagazagazagazagaza"
End Class

Public Class Three
    Sub New(l As String, k As String, v As String)
        Language = l
        Key = k
        Value = v
    End Sub
    Public Property Language As String
    Public Property Key As String
    Public Property Value As String
End Class