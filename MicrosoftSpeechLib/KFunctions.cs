using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Media.Render;

namespace MicrosoftSpeechLib
{
    public class KSpeechFunctions
    {

        public KSpeechFunctions(string AzureDataMarketClientId, string AzureDataMarketClientSecret)
        {

            this.ViewModel = new ResultViewModel();
            this.speechTranlateClient = new SpeechTranslateClient(
                AzureDataMarketClientId,
                AzureDataMarketClientSecret);
        }

        //public async Task<Dictionary<string, string>> JsonSpeechListAsync()
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        var response = await httpClient.GetAsync("https://dev.microsofttranslator.com/languages?api-version=1.0&scope=text,tts,speech");
        //        response.EnsureSuccessStatusCode();
        //        var jsonString = await response.Content.ReadAsStringAsync();
        //        dynamic jsonObject = JObject.Parse(jsonString);
        //        Dictionary<string, string> dic = new Dictionary<string, string>();

        //        foreach (var s in jsonObject.speech)
        //            dic.Add(s.Name, s.Value.name);
      
        //        return jsonObject;
        //    }
        //}

        /// <summary>
        /// display the recognition and translation result to the ViewModel
        /// </summary>
        /// <param name="result"></param>
        private async void DisplayResultAsync(Result result)
        {
            await this.ViewModel.AddAsync(result);
        }

        /// <summary>
        /// Send the audio result to the speaker output node.
        /// </summary>
        /// <param name="frame"></param>
        private void SendAudioOut(AudioFrame frame)
        {
            this.textToSpeechOutputNode.AddFrame(frame);
        }

        /// <summary>
        /// Send the data from the mic to the speech translate client
        /// </summary>
        /// <param name="frame"></param>
        private void SendToSpeechTranslate(AudioFrame frame)
        {
            this.speechTranlateClient.SendAudioFrame(frame);
        }

        AudioGraph graph;
        AudioFrameOutputNode speechTranslateOutputMode;
        AudioDeviceOutputNode speakerOutputNode;
        SpeechTranslateClient speechTranlateClient;
        
        AudioFrameInputNode textToSpeechOutputNode;

        /// <summary>
        /// reset the service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton(ref Windows.UI.Xaml.Controls.Button StartButton, ref Windows.UI.Xaml.Controls.Button StopButton)
        {
            this.speechTranlateClient.Close();

            if (this.graph != null)
            {
                this.graph?.Stop();
                this.graph?.Dispose();
                this.graph = null;
            }

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        /// <summary>
        /// ViewModel that is bind to the List View
        /// </summary>
        public ResultViewModel ViewModel { get; set; }


        AudioEncodingProperties pcmEncoding = AudioEncodingProperties.CreatePcm(16000, 1, 16);
        public async void InputAsync(DeviceInformation inp)
        {
            // mic -> machine translation speech translate
            var microphone = await DeviceInformation.CreateFromIdAsync((inp).Id);

            this.speechTranslateOutputMode = this.graph.CreateFrameOutputNode(pcmEncoding);
            this.graph.QuantumProcessed += (s, a) => this.SendToSpeechTranslate(this.speechTranslateOutputMode.GetFrame());

            this.speechTranslateOutputMode.Start();

            var micInputResult = await this.graph.CreateDeviceInputNodeAsync(MediaCategory.Speech, pcmEncoding, microphone);

            if (micInputResult.Status == AudioDeviceNodeCreationStatus.Success)
            {
                micInputResult.DeviceInputNode.AddOutgoingConnection(this.speechTranslateOutputMode);
                micInputResult.DeviceInputNode.Start();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async void StartAsync(string fromValue, string toValue, string voiceValue)
        {
            await speechTranlateClient.ConnectAsync(fromValue, toValue, voiceValue, DisplayResultAsync, SendAudioOut);
        }
        public async void OutputAsync()
        {
            // machine translation text to speech output -> speaker

            var speakerOutputResult = await this.graph.CreateDeviceOutputNodeAsync();

            if (speakerOutputResult.Status == AudioDeviceNodeCreationStatus.Success)
            {
                this.speakerOutputNode = speakerOutputResult.DeviceOutputNode;
                this.speakerOutputNode.Start();
            }
            else
            {
                throw new InvalidOperationException();
            }

            this.textToSpeechOutputNode = this.graph.CreateFrameInputNode(pcmEncoding);
            this.textToSpeechOutputNode.AddOutgoingConnection(this.speakerOutputNode);
            this.textToSpeechOutputNode.Start();

        }

        public async void ClearAsync()
        {
            await ViewModel.ClearAsync();
        }

        public async void St(DeviceInformation micBox)
        {
            var result = await AudioGraph.CreateAsync(
           new AudioGraphSettings(AudioRenderCategory.Speech)
           {
               DesiredRenderDeviceAudioProcessing = AudioProcessing.Raw,
               AudioRenderCategory = AudioRenderCategory.Speech,
               EncodingProperties = pcmEncoding
           });
            if (result.Status == AudioGraphCreationStatus.Success)
            { 
                graph = result.Graph;

                InputAsync((DeviceInformation)micBox);

            OutputAsync();
            graph.Start();
        }
        }



    }
}
