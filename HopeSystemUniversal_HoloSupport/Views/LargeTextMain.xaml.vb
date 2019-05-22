Imports System.IO.IsolatedStorage
Imports Windows.Networking.Proximity
Imports System.IO
Imports Windows.ApplicationModel.Store
Imports Store = Windows.ApplicationModel.Store
Imports Windows.UI.Popups
Imports System.Net

Public NotInheritable Class LargeTextMain
    Inherits Page


    Function replacer(s As String) As String
        Return s.Replace(" ", "_")
    End Function

    Private Sub lstWords_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lstWords.SelectionChanged

        If lstWords.SelectedIndex = -1 Then
            Return
        End If
        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
        Dim s As EnlargeSaver = CType(lstWords.SelectedItem, EnlargeSaver)
        If roamingSettings.Values.ContainsKey("EnlargeSaver") Then
            Dim li As List(Of EnlargeSaver) = roamingSettings.Values("EnlargeSaver")

            Dim l = (From k In li Where k.Name = s.Name).FirstOrDefault
            If l IsNot Nothing Then
                l.Times += 1
            End If
            roamingSettings.Values.Remove("EnlargeSaver")
            roamingSettings.Values("EnlargeSaver") = li


            Frame.Navigate(GetType(LargeTextView), CType(lstWords.SelectedItem, EnlargeSaver).Name)

            lstWords.SelectedIndex = -1
        End If



    End Sub

    Sub FillList()
        up1.Visibility = Visibility.Visible
        up1.IsIndeterminate = True
        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
        If roamingSettings.Values.ContainsKey("EnlargeSaver") Then
            Dim li = TryCast(roamingSettings.Values("EnlargeSaver"), List(Of EnlargeSaver))
            lstWords.Items.Clear()


            If li IsNot Nothing Then
                For Each c In li.OrderByDescending(Function(n) n.Times)
                    lstWords.Items.Add(c)
                Next

                Try

                    For Each wo In li.Where(Function(n) n.SoundFile = "")

                        Dim file = IsolatedStorageFile.GetUserStoreForApplication()

                        Dim client = WebRequest.CreateHttp("http://www.bing.com")
                        'AddHandler client.OpenReadCompleted, Sub(oo As Object, ee As OpenReadCompletedEventArgs)
                        '                                         Using stream As New IsolatedStorageFileStream(wo.Name & ".mp3", System.IO.FileMode.Create, file)
                        '                                             Dim buffer As Byte() = New Byte(1023) {}
                        '                                             Try
                        '                                                 While ee.Result.Read(buffer, 0, buffer.Length) > 0
                        '                                                     stream.Write(buffer, 0, buffer.Length)
                        '                                                 End While
                        '                                             Catch ex As Exception

                        '                                             End Try

                        '                                         End Using

                        '                                         wo.SoundFile = wo.Name & ".mp3"

                        '                                     End Sub
                        'client.OpenReadAsync(New Uri(String.Format("http://translate.google.com/translate_tts?ie=UTF-8&q={0}&tl=ar", wo.Name)))

                    Next

                    'Dim settings As IO.IsolatedStorage.IsolatedStorageSettings = IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings
                    'If Not settings.Contains("EnlargeSaver") Then
                    '    settings.Add("EnlargeSaver", li)
                    'Else
                    '    settings.Remove("EnlargeSaver")
                    '    settings.Add("EnlargeSaver", li)
                    'End If
                    'settings.Save()



                Catch ex As Exception
                    up1.Visibility = Visibility.Collapsed
                    up1.IsIndeterminate = False
                End Try

            End If

        End If
        up1.IsIndeterminate = False
        up1.Visibility = Visibility.Collapsed

    End Sub



    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Try
            FillList()
        Catch ex As Exception

        End Try
        MyBase.OnNavigatedTo(e)
    End Sub
    Dim loader As New Windows.ApplicationModel.Resources.ResourceLoader()


    Private Sub EnlargeAdd()
        Dim s As New EnlargeSaver With {.Language = loader.GetString("ResourceLanguage").Split("-")(0), .Name = txt_string.Text, .Times = 1}
        EnlargeAdd(s)
    End Sub

    Private Sub EnlargeAdd(s As EnlargeSaver)
        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
        If Not roamingSettings.Values.ContainsKey("EnlargeSaver") Then
            Dim li As New List(Of EnlargeSaver)()
            li.Add(s)
            roamingSettings.Values("EnlargeSaver") = li
        Else
            Dim li As List(Of EnlargeSaver) = roamingSettings.Values("EnlargeSaver")

            Dim l = (From k In li Where k.Name = txt_string.Text).FirstOrDefault
            If l IsNot Nothing Then
                l.Times += 1
            Else
                li.Add(s)
            End If
            roamingSettings.Values.Remove("EnlargeSaver")
            roamingSettings.Values("EnlargeSaver") = li
        End If

    End Sub

    Private Sub btn_Enlarge_Click(sender As Object, e As RoutedEventArgs) Handles btn_Enlarge.Click
        Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings

        If (roamingSettings.Values.ContainsKey("VideoCount")) Then
            roamingSettings.Values("VideoCount") = 1
        Else
            roamingSettings.Values("VideoCount") = Integer.Parse(roamingSettings.Values("VideoCount")).ToString() + 1
        End If
        If Integer.Parse(roamingSettings.Values("VideoCount").ToString()) > 5 Then
            If Store.CurrentApp.LicenseInformation.ProductLicenses("FullVideo").IsActive Then
                Try

                    EnlargeAdd()



                Catch ex As Exception

                End Try


                Frame.Navigate(GetType(LargeTextView), txt_string.Text)
            Else
                '     App.ShowInAppBuyFull()
            End If
        Else
            Try

                EnlargeAdd()



            Catch ex As Exception

            End Try


            Frame.Navigate(GetType(LargeTextView), txt_string.Text)
        End If



    End Sub

    Private Sub btn_Delete_Click(sender As Object, e As RoutedEventArgs) Handles btn_Delete.Click
        Try

            Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings

            If roamingSettings.Values.ContainsKey("EnlargeSaver") Then
                roamingSettings.Values.Remove("EnlargeSaver")
                lstWords.Items.Clear()

                Try
                    Dim file = IsolatedStorageFile.GetUserStoreForApplication()
                    For Each f In file.GetFileNames
                        file.DeleteFile(f)
                    Next
                Catch ex As Exception
                    '' TODO
                End Try
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Async Sub AppBarUpload_Click(sender As Object, e As RoutedEventArgs) Handles btn_Upload.Click
        If ProximityDevice.GetDefault() IsNot Nothing Then
            Dim device As ProximityDevice = ProximityDevice.GetDefault()
            If device IsNot Nothing Then

                Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings

                If (roamingSettings.Values.ContainsKey("EnlargeSaver")) Then
                    Dim li = TryCast(roamingSettings.Values("EnlargeSaver"), List(Of EnlargeSaver))
                    device.PublishMessage("Windows.SampleMessageType", KCommon.IsolatedStorageHelper.Serialize(li))
                    Dim dialog As New MessageDialog(loader.GetString("Done"))
                    Await dialog.ShowAsync()
                Else
                    Dim dialog As New MessageDialog(loader.GetString("EmptyList"))
                    Await dialog.ShowAsync()
                End If



            End If
        Else
            Dim dialog As New MessageDialog(loader.GetString("NFCNotSupported"))
            Await dialog.ShowAsync()
        End If
    End Sub
    Private Async Sub btn_Download_Click(sender As Object, e As RoutedEventArgs) Handles btn_Download.Click
        If ProximityDevice.GetDefault() IsNot Nothing Then
            Dim device As ProximityDevice = ProximityDevice.GetDefault()
            If device IsNot Nothing Then


                device.SubscribeForMessage("Windows.SampleMessageType", Async Sub(ee, oo)
                                                                            'ee.DeviceId, EnlargeAdd

                                                                            Dim liEx = KCommon.IsolatedStorageHelper.Deserialize(Of List(Of EnlargeSaver))(oo.DataAsString)
                                                                            Dim roamingSettings As Windows.Storage.ApplicationDataContainer = Windows.Storage.ApplicationData.Current.RoamingSettings
                                                                            If Not roamingSettings.Values.ContainsKey("EnlargeSaver") Then
                                                                                roamingSettings.Values("EnlargeSaver") = liEx
                                                                            Else
                                                                                Dim li As List(Of EnlargeSaver) = roamingSettings.Values("EnlargeSaver")

                                                                                For Each c In liEx
                                                                                    Dim l = (From k In li Where k.Name = c.Name).FirstOrDefault
                                                                                    If l IsNot Nothing Then
                                                                                        l.Times += 1
                                                                                    Else
                                                                                        li.Add(c)
                                                                                    End If
                                                                                Next
                                                                                roamingSettings.Values.Remove("EnlargeSaver")
                                                                                roamingSettings.Values("EnlargeSaver") = li
                                                                            End If

                                                                            Dim ms As New Popup
                                                                            Dim dialog As New MessageDialog(loader.GetString("Done"))
                                                                            Await dialog.ShowAsync()
                                                                        End Sub)






            End If
        Else
            Dim dialog As New MessageDialog(loader.GetString("NFCNotSupported"))
            Await dialog.ShowAsync()
        End If


    End Sub
End Class


Public Class EnlargeSaver
    Public Property Name As String
    Public Property Times As Integer
    Public Property Size As Double = 88
    Public Property SoundFile As String = ""
    Public Property Language As String
End Class