using HopeUWP.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace HopeUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        public Page3()
        {
            this.InitializeComponent();
            this.DataContext = new PicturesViewModel();
        }
    }
}
