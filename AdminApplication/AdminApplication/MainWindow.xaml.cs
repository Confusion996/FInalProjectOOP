using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using AdminApplication.Pages;
using AdminApplication.Services;
using AdminApplication.Helpers;
using Microsoft.UI;

namespace AdminApplication
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            (appWindow.Presenter as OverlappedPresenter)?.Maximize();

            // Always show login page first if not logged in
            if (!AppState.IsAdminLoggedIn)
            {
                contentFrame.Navigate(typeof(LoginPage));
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                // Redirect to login if not logged in
                if (!AppState.IsAdminLoggedIn)
                {
                    contentFrame.Navigate(typeof(LoginPage));
                    return;
                }

                string selectedTag = selectedItem.Tag?.ToString() ?? string.Empty;

                Type pageType = selectedTag switch
                {
                    "Applications" => typeof(ApplicationManagement),
                    "Payments" => typeof(Payments),
                    "Requests" => typeof(Requests),
                    "Reports" => typeof(Report),
                    _ => null
                };

                if (pageType != null)
                {
                    contentFrame.Navigate(pageType);
                }
            }
        }
    }
}
