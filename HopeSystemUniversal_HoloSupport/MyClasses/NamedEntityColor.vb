Imports Windows.UI

Public Module NamedEntityColor

    Public Function ChangeColor(a As ZZAzureServices.NamedEntity) As Brush
        Dim b As New SolidColorBrush()
        Select Case a.NamedEntityType
            Case ZZAzureServices.NamedEntityType.Human
                b.Color = Colors.Blue
            Case ZZAzureServices.NamedEntityType.Location
                b.Color = Colors.Red
            Case ZZAzureServices.NamedEntityType.Organization
                b.Color = Colors.Green
            Case Else
                b.Color = Colors.Brown
        End Select
        Return b
    End Function
    Public Function GetArabicType(a As ZZAzureServices.NamedEntity) As String
        Select Case a.NamedEntityType
            Case ZZAzureServices.NamedEntityType.Human
                Return "شخصية"
            Case ZZAzureServices.NamedEntityType.Location
                Return "دولة أو عنوان"
            Case ZZAzureServices.NamedEntityType.Organization
                Return "منظمة"
            Case Else
                Return "لا شيء"
        End Select
    End Function

End Module
