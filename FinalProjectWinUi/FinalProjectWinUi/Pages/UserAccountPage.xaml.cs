using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FinalProjectWinUi.Services;
using System;

namespace FinalProjectWinUi.Pages
{
    public sealed partial class UserAccountPage : Page
    {
        public UserAccountPage()
        {
            this.InitializeComponent();

            // Populate name and email from AppState
            if (AppState.LoggedInUser != null)
            {
                NameText.Text = $"Name: {AppState.LoggedInUser.FirstName} {AppState.LoggedInUser.LastName}";
                EmailText.Text = $"Email: {AppState.LoggedInUser.Email}";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppState.isUserLoggedIn = false;
            AppState.LoggedInUser = null;

            // Navigate back to login page
            Frame.Navigate(typeof(LoginForm));
        }
        private async void ShowDialog(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot // This is required in WinUI 3
            };

            await dialog.ShowAsync();
        }

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            var user = AppState.LoggedInUser;

            if (user == null)
            {
                ShowDialog("Error", "No user is logged in.");
                return;
            }

            if (string.IsNullOrWhiteSpace(oldPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowDialog("Validation Error", "Please fill out all password fields.");
                return;
            }

            if (oldPassword != user.Password)
            {
                ShowDialog("Incorrect Password", "The old password is incorrect.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowDialog("Mismatch", "New password and confirmation do not match.");
                return;
            }

            if (newPassword.Length < 6)
            {
                ShowDialog("Weak Password", "Password must be at least 6 characters long.");
                return;
            }

            // Perform update in database
            bool success = await SignUp.UpdateUserPasswordAsync(user.Email, newPassword);
            if (success)
            {
                user.Password = newPassword; // update local session
                ShowDialog("Success", "Password changed successfully.");
                OldPasswordBox.Password = "";
                NewPasswordBox.Password = "";
                ConfirmPasswordBox.Password = "";
            }
            else
            {
                ShowDialog("Error", "Failed to update password in the database.");
            }
        }

    }
}
