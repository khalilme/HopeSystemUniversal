﻿using Intense.Presentation;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using HopeUWP.Pages;
using HopeUWP.Presentation;

namespace HopeUWP
{
    public sealed partial class Shell : UserControl
    {
        public Shell()
        {
            this.InitializeComponent();

            var vm = new ShellViewModel();
            //vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Welcome", PageType = typeof(WelcomePage) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Welcome", PageType = typeof(LettersSign) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Page 1", PageType = typeof(Page1) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Page 2", PageType = typeof(Page2) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "أخبار الصم", PageType = typeof(NewsViewerDirectWeb) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "مكبر النصوص", PageType = typeof(EnlargeTextMain) });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Page 3", PageType = typeof(Page3) });

            vm.BottomItems.Add(new NavigationItem { Icon = "", DisplayName = "Settings", PageType = typeof(SettingsPage) });

            // select the first top item
            vm.SelectedItem = vm.TopItems.First();

            this.ViewModel = vm;
        }

        public ShellViewModel ViewModel { get; private set; }

        public Frame RootFrame
        {
            get
            {
                return this.Frame;
            }
        }
    }
}
