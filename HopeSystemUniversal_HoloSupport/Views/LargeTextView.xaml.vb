Imports Windows.Devices.Sensors
Imports Windows.Media.SpeechSynthesis
Imports Windows.Storage.Streams
Imports Windows.UI.Core
Imports Windows.UI.Popups

Public NotInheritable Class LargeTextView
    Inherits Page

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        Dim dialog As New MessageDialog(loader.GetString("SelectSpeachLang"))
        Try
            _synth = New SpeechSynthesizer
        Catch ex As Exception
            Frame.Navigate(GetType(MainPage))
        Finally
            'Await dialog.ShowAsync()
        End Try

        DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape
        DisplayInformation.AutoRotationPreferences = DisplayOrientations.LandscapeFlipped

        lblText.Text = e.Parameter.ToString()


        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
        Dim li As List(Of EnlargeSaver) = roamingSettings.Values("EnlargeSaver")

        Dim l = (From k In li Where k.Name = lblText.Text).FirstOrDefault
        slider.Value = l.Size
        roamingSettings.Values.Remove("EnlargeSaver")
        roamingSettings.Values("EnlargeSaver") = li
    End Sub



    Public Async Function SynthesizeTextToSpeechAsync(text As String) As Task(Of IRandomAccessStream)
        Dim Stream As IRandomAccessStream = Nothing
        Using synthesizer As New SpeechSynthesizer()
            Stream = Await synthesizer.SynthesizeTextToStreamAsync(text)
        End Using
        Return Stream
    End Function
    Async Sub PlaySound()
        Await SynthesizeTextToSpeechAsync(lblText.Text)
    End Sub

    Private _synth As SpeechSynthesizer
    Protected Property IsRunnable As Boolean = False
    Dim VoiceOn As Boolean = False
    Dim loader As New Windows.ApplicationModel.Resources.ResourceLoader()

    Public Sub New()

        InitializeComponent()

        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible
        AddHandler SystemNavigationManager.GetForCurrentView().BackRequested, Sub(s, a)
                                                                                  If (Frame.CanGoBack) Then
                                                                                      Try
                                                                                          IsRunnable = False
                                                                                          '_synth.CancelAll()
                                                                                      Catch ex As Exception

                                                                                      End Try

                                                                                  End If
                                                                              End Sub


    End Sub

    Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
        VoiceOn = Not VoiceOn
        If VoiceOn Then
            imgsound.Source = Nothing
            imgsound.Source = New BitmapImage(New Uri("/images/Sound.png", UriKind.Relative))



            Dim GoogleSound As Boolean = False

            For Each c In KCommon.UnsuportedSpeechLetter
                If lblText.Text.Contains(c) Then
                    GoogleSound = True
                    Exit For
                End If
            Next

            If GoogleSound Then
                IsRunnable = True
                'Try
                '    Dim fname As String = lblText.Text & ".mp3"
                '    Using myIsolatedStorage As IsolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication()
                '        Using fileStream As IsolatedStorageFileStream = myIsolatedStorage.OpenFile(fname, FileMode.Open, FileAccess.Read)
                '            meAudio.SetSource(fileStream)
                '        End Using
                '    End Using

                'Catch ex As Exception

                'End Try



            Else
                IsRunnable = True
                Try
                    While IsRunnable
                        Await SynthesizeTextToSpeechAsync(lblText.Text)
                    End While
                Catch ex As Exception

                End Try
            End If


        Else
            imgsound.Source = Nothing
            Dim uriSource = New Uri("/images/sound2.png", UriKind.Relative)
            imgsound.Source = New BitmapImage(uriSource)
            Try
                IsRunnable = False
                '_synth.CancelAll()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub ellWhite_Click(sender As Object, e As RoutedEventArgs) Handles ellWhite.Click
        Application.Current.Resources("PhoneDarkThemeVisibility") = 7
    End Sub


    Private Sub meAudio_MediaEnded(sender As Object, e As RoutedEventArgs)
        If IsRunnable = True Then
            meAudio.Position = TimeSpan.Zero
            meAudio.Play()
        Else
            meAudio.Stop()
        End If

    End Sub

    Private Sub slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles slider.ValueChanged
        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
        Dim li As List(Of EnlargeSaver) = roamingSettings.Values("EnlargeSaver")
        Dim l = (From k In li Where k.Name = lblText.Text).FirstOrDefault
        If l IsNot Nothing Then
            l.Size = e.NewValue
        End If

        slider.Value = l.Size
        roamingSettings.Values.Remove("EnlargeSaver")
        roamingSettings.Values("EnlargeSaver") = li
    End Sub
End Class
