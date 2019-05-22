' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.UI.Core
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MyFeedBack
    Inherits Page



    Private Async Sub feedbackButton_Click(sender As Object, e As RoutedEventArgs) Handles feedbackButton.Click
        Dim device = CoreWindow.GetForCurrentThread().Bounds
        Dim c As New Dictionary(Of String, String)
        c.Add("width", device.Width)
        Dim launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault()
        Await launcher.LaunchAsync()


    End Sub

    Private Sub MyFeedBack_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported Then
            feedbackButton.Visibility = Visibility.Visible
        End If
    End Sub

End Class
