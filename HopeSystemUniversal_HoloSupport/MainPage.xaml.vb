' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Frame.Navigate(GetType(TheVoiceTranslator))
        'Frame.Navigate(GetType(SpeechSettings))
    End Sub

    Private Sub HamburgerMenu_OptionsItemClick(sender As Object, e As ItemClickEventArgs)

    End Sub

    Private Sub HamburgerMenu_ItemClick(sender As Object, e As ItemClickEventArgs)

    End Sub
End Class
