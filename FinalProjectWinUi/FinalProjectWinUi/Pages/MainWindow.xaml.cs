using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System;
using FinalProjectWinUi.Pages;
using Microsoft.UI;
using FinalProjectWinUi.Services;

namespace FinalProjectWinUi
{
    public sealed partial class MainWindow : Window
    {
        private bool isUserLoggedIn = false;

        public MainWindow()
        {
            this.InitializeComponent();
            this.MaximizeWindow();
            
            nvSample.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            nvSample.SelectionChanged += NavView_SelectionChanged;
            contentFrame.Navigate(typeof(HomePage));
        }

        private void MaximizeWindow()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            if (presenter != null)
            {
                presenter.Maximize();
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Reset all items to default style
            foreach (var item in sender.MenuItems)
            {
                if (item is NavigationViewItem navItem)
                {
                    navItem.Style = (Style)Application.Current.Resources["NavigationViewItemStyle"];
                }
            }
            foreach (var item in sender.FooterMenuItems)
            {
                if (item is NavigationViewItem navItem)
                {
                    navItem.Style = (Style)Application.Current.Resources["NavigationViewItemStyle"];
                }
            }

            // Apply selected style to the selected item
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                selectedItem.Style = (Style)Application.Current.Resources["NavigationViewItemSelectionStyle"];

                string selectedTag = selectedItem.Tag?.ToString() ?? string.Empty;

                if (selectedTag == "BuyPage" || selectedTag == "UserAccountPage")
                {
                    if (!AppState.isUserLoggedIn)
                    {                     
                        contentFrame.Navigate(typeof(LoginForm));
                        return;
                    }
                }

                Type pageType = selectedTag switch
                {
                    "HomePage" => typeof(HomePage),
                    "PlansPage" => typeof(PlansPage),
                    "BuyPage" => typeof(BuyPage),
                    "UserAccountPage" => typeof(UserAccountPage),
                    _ => null
                };

                if (pageType != null)
                {
                    contentFrame.Navigate(pageType);
                }
            }
        }

        public void SetUserLoggedIn(bool isLoggedIn)
        {
            isUserLoggedIn = isLoggedIn;
        }
    }
}


