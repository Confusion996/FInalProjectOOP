using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AdminApplication.Services;
using AdminApplication.Models;
using System;
using AdminApplication.Helpers;

namespace AdminApplication.Pages
{
    public sealed partial class LoginPage : Page
    {
        private bool isDialogOpen = false;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignupPage)); // Change to your signup page
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDialogOpen) return;

            string username = LoginUsername.Text;
            string password = LoginPass.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                isDialogOpen = true;
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please enter both email and password.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await errorDialog.ShowAsync();
                isDialogOpen = false;
                return;
            }

            var adminService = new login();
            var admin = adminService.ValidateAdminLogin(username, password);

            if (admin == null)
            {
                isDialogOpen = true;
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Invalid username or password.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await errorDialog.ShowAsync();
                isDialogOpen = false;
            }
            else
            {
                AppState.IsAdminLoggedIn = true;
                AppState.LoggedInAdmin = admin;

                Frame.Navigate(typeof(Report)); // Replace with your main page after login
            }
        }
    }
}
