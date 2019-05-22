Public Class AChannel

    Public Property Name As String
    Public Property Image As String
    Public Property Url As String
    Public Property Playlists As List(Of PlayList)



End Class

Public Class PlayList
    Public Property Title As String
    Public Property TitleSmall As String
     
    Public Property URL As String
End Class