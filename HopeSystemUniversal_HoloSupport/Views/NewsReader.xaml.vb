' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Imports System.Net.NetworkInformation
Imports Windows.Foundation.Metadata
Imports Windows.Storage
Imports Windows.System
Imports Windows.UI
Imports Windows.UI.Popups
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class NewsReader
    Inherits Page



    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        ImageProgressBar.Visibility = Visibility.Visible
        ImageProgressBar.IsIndeterminate = True




        'If ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView") Then
        '    Dim titleBar = ApplicationView.GetForCurrentView().TitleBar
        '    If titleBar IsNot Nothing Then
        '        titleBar.ButtonBackgroundColor = Colors.DarkBlue
        '        titleBar.ButtonForegroundColor = Colors.White
        '        titleBar.BackgroundColor = Colors.Blue
        '        titleBar.ForegroundColor = Colors.White
        '    End If
        'End If

        ''Mobile customization
        'If ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar") Then
        '    Dim statusBar__1 = StatusBar.GetForCurrentView()
        '    If statusBar__1 IsNot Nothing Then
        '        statusBar__1.BackgroundOpacity = 1
        '        statusBar__1.BackgroundColor = Colors.DarkBlue
        '        statusBar__1.ForegroundColor = Colors.White
        '    End If
        'End If
















        Dim loader As New Windows.ApplicationModel.Resources.ResourceLoader()

        If NetworkInterface.GetIsNetworkAvailable() = False Then
            Dim dialog As New MessageDialog(loader.GetString("NoInternet"))
            Await dialog.ShowAsync()
            ImageProgressBar.Visibility = Visibility.Collapsed
            ImageProgressBar.IsIndeterminate = False
        Else

            Dim helperRSS As New RSShelperClass


            'Dim storageFile As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///News/" &
            '                                                                                         loader.GetString("ResourceLanguage").Split("-")(0) & ".xml"))

            Dim storageFile As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(New Uri("ms-appx:///News/en.xml"))



            Using stream = Await storageFile.OpenStreamForReadAsync()
                Dim XPath As XElement = XElement.Load(stream)
                Dim m = (From k In XPath.Descendants("Cat")
                         Select New AChannel With
                         {
                             .Name = k.Attribute("Name").Value,
                             .Url = k.Attribute("URL").Value
                         }).ToList()

                helperRSS.Go(ListBoxRssFeed, m.FirstOrDefault().Url)

                'For Each c In m

                '    Dim m2 = (From k In XPath.Descendants("Cat") Where k.Attribute("Name").Value = appBarMenuCountrieswordslist.Text
                '                                                                 Select
                '                                                                     Url = k.Attribute("URL").Value
                '                                                           ).FirstOrDefault()

                '    helperRSS.Go(ListBoxRssFeed, m2)
                'Next
            End Using






            ImageProgressBar.Visibility = Visibility.Collapsed
            ImageProgressBar.IsIndeterminate = False

        End If















    End Sub



    Private Async Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim rss As Windows.Web.Syndication.SyndicationItem = ListBoxRssFeed.SelectedItem

        Await Launcher.LaunchUriAsync(New Uri(rss.Id))
    End Sub



End Class
