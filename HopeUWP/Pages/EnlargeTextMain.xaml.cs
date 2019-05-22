using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HopeUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EnlargeTextMain : Page
    {
        public EnlargeTextMain()
        {
            this.InitializeComponent();
        }




        public string replacer(string s)
        {
            return s.Replace(" ", "_");
        }


        private void lstWords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstWords.SelectedIndex == -1)
            {
                return;
            }
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            EnlargeSaver s = (EnlargeSaver)lstWords.SelectedItem;
            if (roamingSettings.Values.ContainsKey("EnlargeSaver"))
            {
                List<EnlargeSaver> li = roamingSettings.Values["EnlargeSaver"] as List<EnlargeSaver>;

                var l = (from k in li where k.Name == s.Name select k).FirstOrDefault();
                if (l != null)
                {
                    l.Times += 1;
                }
                roamingSettings.Values.Remove("EnlargeSaver");
                roamingSettings.Values["EnlargeSaver"] = li;


                Frame.Navigate(typeof(LargeTextView), ((EnlargeSaver)lstWords.SelectedItem).Name);

                lstWords.SelectedIndex = -1;
            }



        }

        public void FillList()
        {
            up1.Visibility = Visibility.Visible;
            up1.IsIndeterminate = true;
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("EnlargeSaver"))
            {
                var li = roamingSettings.Values["EnlargeSaver"] as List<EnlargeSaver>;
                lstWords.Items.Clear();


                if (li != null)
                {
                    foreach (var c in li.OrderByDescending(n => n.Times))
                    {
                        lstWords.Items.Add(c);
                    }


                    try
                    {

                        foreach (var wo in li.Where(n => string.IsNullOrEmpty(n.SoundFile)))
                        {
                            dynamic file = IsolatedStorageFile.GetUserStoreForApplication();

                            dynamic client = WebRequest.CreateHttp("http://www.bing.com");
                            //AddHandler client.OpenReadCompleted, Sub(oo As Object, ee As OpenReadCompletedEventArgs)
                            //                                         Using stream As New IsolatedStorageFileStream(wo.Name & ".mp3", System.IO.FileMode.Create, file)
                            //                                             Dim buffer As Byte() = New Byte(1023) {}
                            //                                             Try
                            //                                                 While ee.Result.Read(buffer, 0, buffer.Length) > 0
                            //                                                     stream.Write(buffer, 0, buffer.Length)
                            //                                                 End While
                            //                                             Catch ex As Exception

                            //                                             End Try

                            //                                         End Using

                            //                                         wo.SoundFile = wo.Name & ".mp3"

                            //                                     End Sub
                            //client.OpenReadAsync(New Uri(String.Format("http://translate.google.com/translate_tts?ie=UTF-8&q={0}&tl=ar", wo.Name)))

                        }

                        //Dim settings As IO.IsolatedStorage.IsolatedStorageSettings = IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings
                        //If Not settings.Contains("EnlargeSaver") Then
                        //    settings.Add("EnlargeSaver", li)
                        //Else
                        //    settings.Remove("EnlargeSaver")
                        //    settings.Add("EnlargeSaver", li)
                        //End If
                        //settings.Save()



                    }
                    catch (Exception ex)
                    {
                        up1.Visibility = Visibility.Collapsed;
                        up1.IsIndeterminate = false;
                    }

                }

            }
            up1.IsIndeterminate = false;
            up1.Visibility = Visibility.Collapsed;

        }



        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    try
        //    {
        //        FillList();

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    base.OnNavigatedTo(e);
        //}

        //Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();


        private void EnlargeAdd()
        {
            EnlargeSaver s = new EnlargeSaver
            {
                //Language = loader.GetString("ResourceLanguage").Split('-')[0],
                Name = txt_string.Text,
                Times = 1
            };
            EnlargeAdd(s);
        }


        private void EnlargeAdd(EnlargeSaver s)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (!roamingSettings.Values.ContainsKey("EnlargeSaver"))
            {
                List<EnlargeSaver> li = new List<EnlargeSaver>();
                li.Add(s);
                roamingSettings.Values["EnlargeSaver"] = li;
            }
            else
            {
                List<EnlargeSaver> li = roamingSettings.Values["EnlargeSaver"] as List<EnlargeSaver>;

                var l = (from k in li where k.Name == txt_string.Text select k).FirstOrDefault();
                if (l != null)
                {
                    l.Times += 1;
                }
                else
                {
                    li.Add(s);
                }
                roamingSettings.Values.Remove("EnlargeSaver");
                roamingSettings.Values["EnlargeSaver"] = li;
            }

        }


        private void btn_Enlarge_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            if ((roamingSettings.Values.ContainsKey("VideoCount")))
            {
                roamingSettings.Values["VideoCount"] = 1;
            }
            else
            {
                roamingSettings.Values["VideoCount"] = int.Parse(roamingSettings.Values["VideoCount"].ToString()).ToString() + 1;
            }
            if (int.Parse(roamingSettings.Values["VideoCount"].ToString()) > 5)
            {
                if (Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses["FullVideo"].IsActive)
                {

                    try
                    {
                        EnlargeAdd();




                    }
                    catch (Exception ex)
                    {
                    }


                    Frame.Navigate(typeof(LargeTextView), txt_string.Text);
                }
                else
                {
                    //     App.ShowInAppBuyFull()
                }
            }
            else
            {

                try
                {
                    EnlargeAdd();




                }
                catch (Exception ex)
                {
                }


                Frame.Navigate(typeof(LargeTextView), txt_string.Text);
            }



        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

                if (roamingSettings.Values.ContainsKey("EnlargeSaver"))
                {
                    roamingSettings.Values.Remove("EnlargeSaver");
                    lstWords.Items.Clear();

                    try
                    {
                        var file = IsolatedStorageFile.GetUserStoreForApplication();
                        foreach (var f in file.GetFileNames())
                        {
                            file.DeleteFile(f);
                        }
                    }
                    catch (Exception ex)
                    {
                        //' TODO
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private async void AppBarUpload_Click(object sender, RoutedEventArgs e)
        {
            //if (ProximityDevice.GetDefault() != null)
            //    ProximityDevice device = ProximityDevice.GetDefault();

            //if (device != null)
            //{
            //    Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            //    if ((roamingSettings.Values.ContainsKey("EnlargeSaver")))
            //    {
            //        var li = roamingSettings.Values["EnlargeSaver"] as List<EnlargeSaver>;
            //        device.PublishMessage("Windows.SampleMessageType", KCommon.IsolatedStorageHelper.Serialize(li));
            //        MessageDialog dialog = new MessageDialog(loader.GetString("Done"));
            //        await dialog.ShowAsync();
            //    }
            //    else
            //    {

            //        MessageDialog dialog = new MessageDialog(loader.GetString("EmptyList"));
            //        await dialog.ShowAsync();
            //    }
            //}
            //else
            //{
            //    MessageDialog dialog = new MessageDialog(loader.GetString("NFCNotSupported"));
            //    await dialog.ShowAsync();
            //}
        }
        //    Private Async Sub btn_Download_Click(sender As Object, e As RoutedEventArgs) Handles btn_Download.Click

        //  If ProximityDevice.GetDefault() IsNot Nothing Then
        //      Dim device As ProximityDevice = ProximityDevice.GetDefault()
        //        If device IsNot Nothing Then


        //            device.SubscribeForMessage("Windows.SampleMessageType", Async Sub(ee, oo)
        //                                                                        'ee.DeviceId, EnlargeAdd

        //                                                                        Dim liEx = KCommon.IsolatedStorageHelper.Deserialize(Of List(Of EnlargeSaver))(oo.DataAsString)
        //                                                                        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
        //                                                                        If Not roamingSettings.Values.ContainsKey("EnlargeSaver") Then
        //                                                                            roamingSettings.Values["EnlargeSaver") = liEx
        //                                                                        Else
        //                                                                            Dim li As List(Of EnlargeSaver) = roamingSettings.Values["EnlargeSaver")

        //                                                                            For Each c In liEx
        //                                                                                Dim l = (From k In li Where k.Name = c.Name).FirstOrDefault
        //                                                                                If l IsNot Nothing Then
        //                                                                                    l.Times += 1
        //                                                                                Else
        //                                                                                    li.Add(c)
        //                                                                                End If
        //                                                                            Next
        //                                                                            roamingSettings.Values.Remove("EnlargeSaver")
        //                                                                            roamingSettings.Values["EnlargeSaver") = li
        //                                                                        End If

        //                                                                        Dim ms As New Popup
        //                                                                        Dim dialog As New MessageDialog(loader.GetString("Done"))
        //                                                                        Await dialog.ShowAsync()
        //                                                                    End Sub)






        //        End If
        //    Else
        //        Dim dialog As New MessageDialog(loader.GetString("NFCNotSupported"))
        //        Await dialog.ShowAsync()
        //    End If


        //End Sub
    }
    public class EnlargeSaver
    {
        public string Name { get; set; }
        public int Times { get; set; }
        public double Size { get; set; }
        public string SoundFile { get; set; }
        public string Language { get; set; }
    }
}
