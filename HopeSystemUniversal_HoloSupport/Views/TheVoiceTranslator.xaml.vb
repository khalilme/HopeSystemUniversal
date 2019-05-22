Imports System.Threading.Tasks
Imports Windows.Devices.Enumeration
Imports Windows.Media
Imports Windows.Media.Audio
Imports Windows.Media.Capture
Imports Windows.Media.Devices
Imports Windows.Media.MediaProperties
Imports Windows.Media.Render
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports System.Linq
Imports Windows.UI.Xaml.Navigation
Imports System.Net.Http
Imports Newtonsoft.Json.Linq
Imports System.Collections.Generic
Imports System.Xml.Serialization
Imports System.IO
Imports Newtonsoft.Json
Imports MicrosoftSpeechLib

Public NotInheritable Class TheVoiceTranslator
    Inherits Page

    Const AzureDataMarketClientId As String = "Deaf"
    Const AzureDataMarketClientSecret As String = "gazagazagazagazagaza"

    Private langVoiceDict As New Dictionary(Of String, List(Of ComboBoxItem))()

    Public Sub New()
        Me.InitializeComponent()
        Me.ViewModel = New ResultViewModel()
        Me.speechTranlateClient = New SpeechTranslateClient(AzureDataMarketClientId, AzureDataMarketClientSecret)
    End Sub

    Dim speakerLi As New List(Of DeviceInformation)
    Dim micBox As New List(Of DeviceInformation)
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        For Each device In Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector())
            micBox.Add(device)
        Next

        For Each device In Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector())
            speakerLi.Add(device)
        Next


        'Using httpClient = New HttpClient()
        '    Dim response = Await httpClient.GetAsync("https://dev.microsofttranslator.com/languages?api-version=1.0&scope=text,tts,speech")
        '    ' add header
        '    response.EnsureSuccessStatusCode()
        '    Dim jsonString = Await response.Content.ReadAsStringAsync()
        '    Dim jsonObject As Object = JObject.Parse(jsonString)
        '    textBox.Text = ""
        '    For Each s As Object In jsonObject.speech

        '        Me.fromComboBox.Items.Add(New ComboBoxItem() With {
        '        .Name = s.Name,
        '        .Content = s.Value.name
        '    })
        '    Next
        '    For Each s In jsonObject.text
        '        Me.toComboBox.Items.Add(New ComboBoxItem() With {
        '        .Name = s.Name,
        '        .Content = s.Value.name
        '    })
        '    Next
        '    For Each s In jsonObject.tts
        '        Dim lang As String = s.Value.language
        '        If Not langVoiceDict.ContainsKey(lang) Then
        '            langVoiceDict(lang) = New List(Of ComboBoxItem)()
        '        End If

        '        ' textBox.Text += $".Add(New ComboBoxItem() With {{.Name = \"{s.Name}\",.Content =\"{String.Format("{0} ({1}) ({2})", s.Value.displayName, s.Value.gender, s.Value.regionName)}\"}})\n";
        '        langVoiceDict(lang).Add(New ComboBoxItem() With {
        '        .Name = s.Name,
        '        .Content = [String].Format("{0} ({1}) ({2})", s.Value.displayName, s.Value.gender, s.Value.regionName)
        '    })
        '    Next
        '    '  textBox.Text= SerializeObject<Dictionary<string, List<ComboBoxItem>>>(langVoiceDict);
        '    ' textBox.Text = JsonConvert.SerializeObject(langVoiceDict);
        '    For Each item In langVoiceDict
        '        Dim rk As String = TryCast(item.Value, List(Of ComboBoxItem))(0).Name
        '        Dim rv As String = TryCast(item.Value, List(Of ComboBoxItem))(0).Content.ToString()


        '        textBox.Text += "langVoiceDict.Add(New Three(""{item.Key}"",""{ rk}"",""{ rv}""))" & vbLf
        '    Next
        'End Using

        'Me.fromComboBox.SelectedIndex = 0
        'Me.toComboBox.SelectedIndex = 0
    End Sub
    Public Function SerializeObject(Of T)(toSerialize As T) As String
        Dim xmlSerializer As New XmlSerializer(toSerialize.[GetType]())

        Using textWriter As New StringWriter()
            xmlSerializer.Serialize(textWriter, toSerialize)
            Return textWriter.ToString()
        End Using
    End Function
    ''' <summary>
    ''' Connect to the Machine Translation Service and Construct the Audio Graph
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub StartButton_Click(sender As Object, e As RoutedEventArgs)
        Await ViewModel.ClearAsync()

        Me.statusText.Text = "يتم الآن الاتصال ..."

        ' The service takes 2 callbacks Action<Result> for text recognition and translation
        ' and Action<AudioFrame> for text to speech response

        Await Me.speechTranlateClient.ConnectAsync("ar-EG", "ar", "ar-EG-Hoda", AddressOf Me.DisplayResultAsync, AddressOf SendAudioOut)

        Me.statusText.Text = "Creating AudioGraph"

        Dim pcmEncoding = AudioEncodingProperties.CreatePcm(16000, 1, 16)

        ' Construct the audio graph
        ' mic -> Machine Translate Service
        ' Machine Translation text to speech output -> speaker
        Dim result = Await AudioGraph.CreateAsync(New AudioGraphSettings(AudioRenderCategory.Speech) With {
        .DesiredRenderDeviceAudioProcessing = AudioProcessing.Raw,
        .AudioRenderCategory = AudioRenderCategory.Speech,
        .EncodingProperties = pcmEncoding
    })

        If result.Status = AudioGraphCreationStatus.Success Then
            Me.graph = result.Graph

            '#Region "input"
            ' mic -> machine translation speech translate
            Dim microphone = Await DeviceInformation.CreateFromIdAsync(DirectCast(micBox(0), DeviceInformation).Id)

            Me.speechTranslateOutputMode = Me.graph.CreateFrameOutputNode(pcmEncoding)
            AddHandler graph.QuantumProcessed, Sub(s, a)
                                                   SendToSpeechTranslate(speechTranslateOutputMode.GetFrame())
                                               End Sub

            Me.speechTranslateOutputMode.Start()

            Dim micInputResult = Await Me.graph.CreateDeviceInputNodeAsync(MediaCategory.Speech, pcmEncoding, microphone)

            If micInputResult.Status = AudioDeviceNodeCreationStatus.Success Then
                micInputResult.DeviceInputNode.AddOutgoingConnection(Me.speechTranslateOutputMode)
                micInputResult.DeviceInputNode.Start()
            Else
                Throw New InvalidOperationException()
            End If
            '#End Region

            ''#Region "output"
            '' machine translation text to speech output -> speaker

            'Dim speakerOutputResult = Await Me.graph.CreateDeviceOutputNodeAsync()

            'If speakerOutputResult.Status = AudioDeviceNodeCreationStatus.Success Then
            '    Me.speakerOutputNode = speakerOutputResult.DeviceOutputNode
            '    Me.speakerOutputNode.Start()
            'Else
            '    Throw New InvalidOperationException()
            'End If

            'Me.textToSpeechOutputNode = Me.graph.CreateFrameInputNode(pcmEncoding)
            'Me.textToSpeechOutputNode.AddOutgoingConnection(Me.speakerOutputNode)
            'Me.textToSpeechOutputNode.Start()
            ''#End Region

            ' start the graph
            Me.graph.Start()
        End If

        Me.statusText.Text = "متصل"

        Me.StartButton.IsEnabled = False
        Me.StopButton.IsEnabled = True
    End Sub

    ''' <summary>
    ''' display the recognition and translation result to the ViewModel
    ''' </summary>
    ''' <param name="result"></param>
    Private Async Sub DisplayResultAsync(result As Result)

        'aa


        Await Me.ViewModel.AddAsync(result)
    End Sub


    ''' <summary>
    ''' Send the audio result to the speaker output node.
    ''' </summary>
    ''' <param name="frame"></param>
    Private Sub SendAudioOut(frame As AudioFrame)
        'Me.textToSpeechOutputNode.AddFrame(frame)
    End Sub

    ''' <summary>
    ''' Send the data from the mic to the speech translate client
    ''' </summary>
    ''' <param name="frame"></param>
    Private Sub SendToSpeechTranslate(frame As AudioFrame)
        Me.speechTranlateClient.SendAudioFrame(frame)
    End Sub


    Dim graph As AudioGraph

    Dim speechTranslateOutputMode As AudioFrameOutputNode

    Dim speakerOutputNode As AudioDeviceOutputNode

    Dim speechTranlateClient As SpeechTranslateClient

    Dim textToSpeechOutputNode As AudioFrameInputNode


    ''' <summary>
    ''' reset the service
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs)
        Me.speechTranlateClient.Close()
        If graph IsNot Nothing Then
            graph?.Stop()
            graph?.Dispose()
            graph = Nothing
        End If


        Me.StartButton.IsEnabled = True
        Me.StopButton.IsEnabled = False
    End Sub



    ''' <summary>
    ''' ViewModel that is bind to the List View
    ''' </summary>
    Public Property ViewModel() As ResultViewModel
        Get
            Return m_ViewModel
        End Get
        Set
            m_ViewModel = Value
        End Set
    End Property
    Private m_ViewModel As ResultViewModel

    'Private Async Sub button_Click(sender As Object, e As RoutedEventArgs) Handles button.Click
    '    Dim speakerOutputResult = Await Me.graph.CreateDeviceOutputNodeAsync()

    '    If speakerOutputResult.Status = AudioDeviceNodeCreationStatus.Success Then
    '        Me.speakerOutputNode = speakerOutputResult.DeviceOutputNode
    '        Me.speakerOutputNode.Start()
    '    Else
    '        Throw New InvalidOperationException()
    '    End If

    '    Dim pcmEncoding = AudioEncodingProperties.CreatePcm(16000, 1, 16)
    '    Me.textToSpeechOutputNode = Me.graph.CreateFrameInputNode(pcmEncoding)
    '    Me.textToSpeechOutputNode.AddOutgoingConnection(Me.speakerOutputNode)
    '    Me.textToSpeechOutputNode.Start()
    'End Sub
    Dim res As ZZAzureServices.AnalyzedCorpus
    Dim a As New ZZAzureServices.HopeServiceClient()
    Private Async Sub button1_Click(sender As Object, e As RoutedEventArgs) Handles button1.Click
        ' Dim c As Result = lstItems.Items(lstItems.Items.Count - 1)

        res = Await a.CorpusAnalyzerAsync("ذهب ميسي إلى البحر")
        ' res = Await a.CorpusAnalyzerAsync("هل تحب أن ترى ماتشات كورة القدم")
        'res = Await a.CorpusAnalyzerAsync(c.Recognition)
        txtCorpus.Text = res.AnalyzedText


        Dim ar() As String = res.AnalyzedText.Split("+")
        For Each i In ar
            If i.Length > 2 Then
                If i.Trim().StartsWith("#") Then
                    Dim word = i.Trim().Replace("#", "").Replace(" ", "")
                    Dim nameEnt = (From k In res.EntitesList Where k.NamedEntityPhrase = word Select k).FirstOrDefault


                    Dim hp As New HyperlinkButton With {
                            .Content = word}

                    AddHandler hp.Click, AddressOf HyperlinkButton_Click


                    hp.Foreground = NamedEntityColor.ChangeColor(nameEnt)
                    hp.Content = hp.Content & " - " & GetArabicType(nameEnt)
                    stCorpus.Children.Add(hp)
                End If

            End If
        Next


    End Sub

    Private Async Sub HyperlinkButton_Click(sender As Object, e As RoutedEventArgs)
        'Try
        Dim hp As HyperlinkButton = sender
            Dim b = Await a.ImageSearchAsync(hp.Content.ToString(), 3)
            'img1.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("/Assets/check.png");
            Dim One1 As New BitmapImage(New Uri(b(0).MediaUrl, UriKind.Absolute))
            Dim One2 As New BitmapImage(New Uri(b(1).MediaUrl, UriKind.Absolute))
            Dim One3 As New BitmapImage(New Uri(b(2).MediaUrl, UriKind.Absolute))

            imgPic1.Source = One1
            imgPic2.Source = One2
            imgPic3.Source = One3
        'Catch ex As Exception

        'End Try
    End Sub
End Class
