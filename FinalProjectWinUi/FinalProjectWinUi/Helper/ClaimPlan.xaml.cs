using System;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT.Interop;
using Windows.Storage.Pickers;
using Windows.Storage;
using FinalProjectWinUi.Services;
using FinalProjectWinUi.Models;

namespace FinalProjectWinUi.Helper
{
    public sealed partial class ClaimPlan : Page
    {
        private StorageFile validIDFile;
        private StorageFile deathCertFile;

        public ClaimPlan()
        {
            this.InitializeComponent();
            LoadPlanDetails();
        }

        private async void LoadPlanDetails()
        {
            string email = AppState.LoggedInUser.Email;

            if (string.IsNullOrEmpty(email))
            {
                await ShowDialog("Error", "Email not found in AppState.");
                return;
            }

            try
            {
                var query = @"SELECT PlanHolderData.PlanId, 
                                 PlanHolderInformation.Lastname, 
                                 PlanHolderInformation.Firstname, 
                                 PlanHolderInformation.Middlename, 
                                 PlanHolderData.Plantype, 
                                 PlanHolderData.PlanName, 
                                 PlanHolderData.ContractPrice, 
                                 PlanHolderData.Balance, 
                                 PlanHolderData.Status, 
                                 PlanHolderData.PaymentStatus, 
                                 CustomerAccount.Email
                              FROM (CustomerAccount 
                                    INNER JOIN PlanHolderData ON CustomerAccount.CustomerId = PlanHolderData.CustomerId) 
                                    INNER JOIN PlanHolderInformation ON PlanHolderData.PlanId = PlanHolderInformation.PlanId
                              WHERE PlanHolderData.Status = 'Approved' AND CustomerAccount.Email = @Email";

                using var connection = new OleDbConnection(Connection.Conn);
                await connection.OpenAsync();

                using var command = new OleDbCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                var reader = await command.ExecuteReaderAsync();
                var plans = new ObservableCollection<PlanData>();

                while (await reader.ReadAsync())
                {
                    var plan = new PlanData
                    {
                        PlanId = reader["PlanId"].ToString(),
                        PlanholderName = $"{reader["Firstname"]} {reader["Middlename"]} {reader["Lastname"]}",
                        PlanType = reader["Plantype"].ToString(),
                        PlanName = reader["PlanName"].ToString(),
                        ContractPrice = reader["ContractPrice"].ToString(),
                        Balance = reader["Balance"].ToString(),
                        Status = reader["Status"].ToString(),
                        PaymentStatus = reader["PaymentStatus"].ToString(),
                    };
                    plans.Add(plan);
                }

                PlanDetailsGrid.ItemsSource = plans;
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", ex.Message);
            }
        }

        private async void UploadValidID_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(picker, hwnd);

            validIDFile = await picker.PickSingleFileAsync(); // ✅ Corrected assignment
            if (validIDFile != null)
            {
                ValidIdPathTextBlock.Text = validIDFile.Path;
            }
        }

        private async void UploadDeathCert_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
            InitializeWithWindow.Initialize(picker, hwnd);

            deathCertFile = await picker.PickSingleFileAsync(); // ✅ Corrected assignment
            if (deathCertFile != null)
            {
                DeathCertPathTextBlock.Text = deathCertFile.Path;
            }
        }

        private async void SubmitClaim_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ClaimantNameTextBox.Text) ||
        string.IsNullOrWhiteSpace(RelationshipTextBox.Text) ||
        string.IsNullOrWhiteSpace(ContactTextBox.Text) ||
        string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
        ClaimDatePicker.Date == DateTimeOffset.MinValue ||
        validIDFile == null ||
        deathCertFile == null)
            {
                await ShowDialog("Missing Info", "All fields and uploads are required.");
                return;
            }


            try
            {
                var selectedPlan = PlanDetailsGrid.SelectedItem as PlanData;
                if (selectedPlan == null)
                {
                    await ShowDialog("No Plan Selected", "Please select a plan before submitting a claim.");
                    return;
                }

                int planId = int.Parse(selectedPlan.PlanId); // Ensure PlanId is numeric
                int customerId = AppState.LoggedInUser.CustomerId;
                string claimantName = ClaimantNameTextBox.Text;
                string relationship = RelationshipTextBox.Text;
                string contactNumber = ContactTextBox.Text;
                string emailAddress = EmailTextBox.Text;
                DateTime dateOfClaim = ClaimDatePicker.Date.DateTime;
                string claimstatus = "Pending"; // Default status
                string deathCertPath = deathCertFile.Path;
                string validIdPath = validIDFile.Path;

                var query = @"INSERT INTO ClaimPlan 
                      (CustomerId, PlanId, ClaimantName, RelationshipToPlanHolder, ContactNumber, 
                       EmailAddress, DateOfClaim, DeathCertificatePath, ValidIdPath, ClaimStatus) 
                      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using var connection = new OleDbConnection(Connection.Conn);
                await connection.OpenAsync();

                using var command = new OleDbCommand(query, connection);

                // Order matters in OleDb — must match VALUES(?) order above
                command.Parameters.Add("CustomerId", OleDbType.Integer).Value = customerId;
                command.Parameters.Add("PlanId", OleDbType.Integer).Value = planId;
                command.Parameters.Add("ClaimantName", OleDbType.VarChar).Value = claimantName;
                command.Parameters.Add("RelationshipToPlanHolder", OleDbType.VarChar).Value = relationship;
                command.Parameters.Add("ContactNumber", OleDbType.VarChar).Value = contactNumber;
                command.Parameters.Add("EmailAddress", OleDbType.VarChar).Value = emailAddress;
                command.Parameters.Add("DateOfClaim", OleDbType.Date).Value = dateOfClaim;
                command.Parameters.Add("DeathCertificatePath", OleDbType.VarChar).Value = deathCertPath;
                command.Parameters.Add("ValidIdPath", OleDbType.VarChar).Value = validIdPath;
                command.Parameters.Add("ClaimStatus", OleDbType.VarChar).Value = claimstatus;

                await command.ExecuteNonQueryAsync();

                await ShowDialog("Claim Submitted", "Your claim has been submitted successfully for review.");
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", $"An error occurred while submitting your claim: {ex.Message}");
            }
        }


        private async Task ShowDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
