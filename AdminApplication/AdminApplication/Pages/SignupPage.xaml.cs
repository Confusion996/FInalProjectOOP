using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using AdminApplication.Services;
using AdminApplication.Models;

namespace AdminApplication.Pages
{
    public sealed partial class SignupPage : Page
    {
        public SignupPage()
        {
            this.InitializeComponent();
        }

        private async void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ErrorText.Text = "All fields are required. Please fill in all boxes.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (password != confirmPassword)
            {
                ErrorText.Text = "Passwords do not match. Please try again.";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                var newAdmin = new AdminAccount
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Password = password
                };

                bool result = await AdminAuthService.SignUpAsync(newAdmin);
                if (result)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Admin account created successfully!",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await dialog.ShowAsync();
                    Frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    ErrorText.Text = "Signup failed. Username might already exist.";
                    ErrorText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error: {ex.Message}";
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }
    }
}
