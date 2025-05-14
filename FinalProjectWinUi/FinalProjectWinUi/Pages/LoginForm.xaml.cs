using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FinalProjectWinUi.Services;
using FinalProjectWinUi.Models;
using System;

namespace FinalProjectWinUi.Pages
{
    public sealed partial class LoginForm : Page
    {
        private bool isDialogOpen = false;

        public LoginForm()
        {
            this.InitializeComponent();
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SIgnupForm));
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDialogOpen) return;

            string email = LoginEmail.Text;
            string password = LoginPass.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
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

            var loginService = new Login();
            var user = loginService.ValidateLogin(email, password);

            if (user == null)
            {
                isDialogOpen = true;
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Invalid email or password.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                await errorDialog.ShowAsync();
                isDialogOpen = false;
            }
            else
            {
                AppState.isUserLoggedIn = true;
                AppState.LoggedInUser = user;

                Frame.Navigate(typeof(BuyPage));
            }
        }

    }
}