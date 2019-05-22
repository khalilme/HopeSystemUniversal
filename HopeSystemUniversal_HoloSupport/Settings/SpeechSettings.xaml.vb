' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.Devices.Enumeration
Imports Windows.Media
Imports Windows.Media.Audio
Imports Windows.Media.Devices
Imports Windows.Media.Render
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SpeechSettings
    Inherits Page
    Dim a As New MicrosoftSpeechLib.KSpeechFunctions(GlobalSettings.AzureDataMarketClientId, GlobalSettings.AzureDataMarketClientSecret)
    Private langVoiceDict As New List(Of Three)()

    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        For Each device In Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector())
            micBox.Items.Add(device)
        Next

        For Each device In Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector())
            speakerBox.Items.Add(device)
        Next


        micBox.SelectedIndex = 0
        speakerBox.SelectedIndex = 0


        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "ar-EG", .Content = "Arabic"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "de-DE", .Content = "German"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "en-US", .Content = "English"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "es-ES", .Content = "Spanish"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "fr-FR", .Content = "French"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "it-IT", .Content = "Italian"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "pt-BR", .Content = "Portuguese"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "zh-CN", .Content = "Chinese Simplified"})
        fromComboBox.Items.Add(New ComboBoxItem() With {.Name = "zh-TW", .Content = "Chinese Traditional"})


        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "ar-EG", .Content = "Arabic"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "de-DE", .Content = "German"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "en-US", .Content = "English"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "es-ES", .Content = "Spanish"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "fr-FR", .Content = "French"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "it-IT", .Content = "Italian"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "pt-BR", .Content = "Portuguese"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "zh-CN", .Content = "Chinese Simplified"})
        toComboBox.Items.Add(New ComboBoxItem() With {.Name = "zh-TW", .Content = "Chinese Traditional"})



        langVoiceDict.Add(New Three("ar", "ar-EG-Hoda", "Hoda (female) (Egypt)"))
        langVoiceDict.Add(New Three("ca", "ca-ES-Herena", "Herena (female) (Spain)"))
        langVoiceDict.Add(New Three("da", "da-DK-Helle", "Helle (female) (Denmark)"))
        langVoiceDict.Add(New Three("de", "de-DE-Hedda", "Hedda (female) (Germany)"))
        langVoiceDict.Add(New Three("en", "en-AU-Catherine", "Catherine (female) (Australia)"))
        langVoiceDict.Add(New Three("es", "es-ES-Laura", "Laura (female) (Spain)"))
        langVoiceDict.Add(New Three("fi", "fi-FI-Heidi", "Heidi (female) (Finland)"))
        langVoiceDict.Add(New Three("fr", "fr-CA-Caroline", "Caroline (female) (Canada)"))
        langVoiceDict.Add(New Three("it", "it-IT-Cosimo", "Cosimo (male) (Italy)"))
        langVoiceDict.Add(New Three("ja", "ja-JP-Ayumi", "Ayumi (female) (Japan)"))
        langVoiceDict.Add(New Three("ko", "ko-KR-Minjoon", "Minjoon (male) (Korea)"))
        langVoiceDict.Add(New Three("nb", "nb-NO-Jon", "Jon (male) (Norway)"))
        langVoiceDict.Add(New Three("nl", "nl-NL-Frank", "Frank (male) (Netherlands)"))
        langVoiceDict.Add(New Three("pl", "pl-PL-Adam", "Adam (male) (Poland)"))
        langVoiceDict.Add(New Three("pt", "pt-BR-Daniel", "Daniel (male) (Brazil)"))
        langVoiceDict.Add(New Three("ru", "ru-RU-Irina", "Irina (female) (Russia)"))
        langVoiceDict.Add(New Three("sv", "sv-SE-Bengt", "Bengt (male) (Sweden)"))
        langVoiceDict.Add(New Three("tr", "tr-TR-Seda", "Seda (female) (Turkey)"))
        langVoiceDict.Add(New Three("zh-Hans", "zh-CN-HuihuiRUS", "HuihuiRUS (female) (People's Republic of China)"))
        langVoiceDict.Add(New Three("zh-Hant", "zh-HK-Danny", "Danny (male) (Hong Kong S.A.R.)"))

        fromComboBox.SelectedIndex = 0
        toComboBox.SelectedIndex = 0
        MyBase.OnNavigatedTo(e)
    End Sub



    Private Sub toComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        voiceComboBox.Items.Clear()
        Dim item = CType(toComboBox.SelectedValue, ComboBoxItem)


        Dim st As String = item.Name.Split("-")(0)

        Dim exists = (From k In langVoiceDict Where k.Language = st Select k).FirstOrDefault()





        If (exists IsNot Nothing) Then
            voiceComboBox.Items.Add(New ComboBoxItem() With {.Name = exists.Key, .Content = exists.Value})
            voiceComboBox.SelectedIndex = 0
        End If


    End Sub

    Dim fromValue, toValue, voiceValue As String

    Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Async Sub StartButton_Click(sender As Object, e As RoutedEventArgs)
        a.ClearAsync()
        fromValue = CType(fromComboBox.SelectedValue, ComboBoxItem).Name
        toValue = CType(toComboBox.SelectedValue, ComboBoxItem).Name
        If voiceComboBox.SelectedValue Is Nothing Then
            voiceValue = Nothing
        Else
            voiceValue = CType(voiceComboBox.SelectedValue, ComboBoxItem).Name
        End If
        a.StartAsync(fromValue, toValue, voiceValue)
        statusText.Text = "Creating AudioGraph"
        a.St(micBox.SelectedItem)


        statusText.Text = "Connecting to Speech Translate Service"




        statusText.Text = "Ready"

        'StartButton.IsEnabled = False
        'StopButton.IsEnabled = True

    End Sub


End Class
