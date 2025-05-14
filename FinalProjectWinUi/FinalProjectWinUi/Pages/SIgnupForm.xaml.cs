using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Data.OleDb;
using FinalProjectWinUi.Models;
using FinalProjectWinUi.Services;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FinalProjectWinUi.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SIgnupForm : Page
    {
        public SIgnupForm()
        {
            this.InitializeComponent();
        }
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {          
            Frame.Navigate(typeof(LoginForm));
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if any of the input fields are empty
            if (string.IsNullOrWhiteSpace(FirstName.Text) ||
                string.IsNullOrWhiteSpace(LastName.Text) ||
                string.IsNullOrWhiteSpace(Email.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "All fields are required. Please fill in all the boxes.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Passwords do not match. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
                return;
            }

            var customer = new CustomerAccount(
                FirstName.Text,
                LastName.Text,
                Email.Text,
                PasswordBox.Password
                );

            var service = new SignUp();
            if(!service.RegisterCustomer(customer))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Email already exists. Please use a different email.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
                return;
            }

            ContentDialog successDialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Account created successfully!",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
            _ = successDialog.ShowAsync();
            this.Frame.Navigate(typeof(LoginForm));
        }
    }
}
