using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class NewsViewerDirectWeb : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Uri uri = new Uri("http://www.aldeaf.com/forumdisplay.php?f=59",UriKind.Absolute);
            try
            {
                LoadingProcessProgressRing.IsActive = true;
                LoadingProcessProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;

                DisplayWebView.Navigate(uri);
            }
            catch (Exception ex)
            {
                //NotifyUser(ex.ToString());
            }

            base.OnNavigatedTo(e);
        }
        private void DisplayWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
        {
            LoadingProcessProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LoadingProcessProgressRing.IsActive = false;
            DisplayWebView.Visibility = Visibility.Visible;

        }
        public NewsViewerDirectWeb()
        {
            this.InitializeComponent();
        }
    }
}
