' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class HoloGraphic
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        If Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily = "Windows.Holographic" Then

        End If
    End Sub
End Class
