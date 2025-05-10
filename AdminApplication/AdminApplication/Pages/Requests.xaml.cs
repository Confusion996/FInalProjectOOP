using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Data;
using System.Data.OleDb;
using Windows.Storage;
using Windows.System;

namespace AdminApplication.Pages
{
    public sealed partial class Requests : Page
    {
        private readonly string connectionString =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        private int? selectedClaimId = null;

        public Requests()
        {
            this.InitializeComponent();
            LoadClaims();
            ClaimsDataGrid.SelectionChanged += ClaimsDataGrid_SelectionChanged;
        }

        // Load all claims from the ClaimPlan table
        private void LoadClaims()
        {
            try
            {
                using var conn = new OleDbConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM ClaimPlan";

                using var adapter = new OleDbDataAdapter(query, conn);
                var dt = new DataTable();
                adapter.Fill(dt);
                ClaimsDataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Error loading claims: {ex.Message}";
            }
        }

        // Handle row selection in DataGrid
        private void ClaimsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClaimsDataGrid.SelectedItem is DataRowView row)
            {
                selectedClaimId = Convert.ToInt32(row["ClaimId"]);
            }
        }

        // Approve and reject button event handlers
        private void Approve_Click(object sender, RoutedEventArgs e) => UpdateClaimStatus("Approved");

        private void Reject_Click(object sender, RoutedEventArgs e) => UpdateClaimStatus("Rejected");

        // ✅ FIXED: Update correct column name ClaimStatus
        private void UpdateClaimStatus(string newStatus)
        {
            if (selectedClaimId == null)
            {
                StatusMessageTextBlock.Text = "Please select a claim from the list.";
                return;
            }

            try
            {
                using var conn = new OleDbConnection(connectionString);
                conn.Open();

                string updateQuery = "UPDATE ClaimPlan SET ClaimStatus = ? WHERE ClaimId = ?";
                using var cmd = new OleDbCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("?", newStatus);
                cmd.Parameters.AddWithValue("?", selectedClaimId.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                StatusMessageTextBlock.Text = rowsAffected > 0
                    ? $"Claim ID {selectedClaimId} status updated to '{newStatus}'."
                    : "Failed to update status.";

                LoadClaims();
                selectedClaimId = null;
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Error updating status: {ex.Message}";
            }
        }

        // Button: View Death Certificate
        private async void ViewDeathCertificate_Click(object sender, RoutedEventArgs e)
        {
            var selectedClaim = GetSelectedClaim();
            if (selectedClaim == null)
            {
                StatusMessageTextBlock.Text = "Please select a claim first.";
                return;
            }

            string path = selectedClaim["DeathCertificatePath"]?.ToString();
            if (!string.IsNullOrEmpty(path))
                await OpenFile(path);
            else
                StatusMessageTextBlock.Text = "No death certificate file path found.";
        }

        // Button: View Valid ID
        private async void ViewValidID_Click(object sender, RoutedEventArgs e)
        {
            var selectedClaim = GetSelectedClaim();
            if (selectedClaim == null)
            {
                StatusMessageTextBlock.Text = "Please select a claim first.";
                return;
            }

            string path = selectedClaim["ValidIDPath"]?.ToString();
            if (!string.IsNullOrEmpty(path))
                await OpenFile(path);
            else
                StatusMessageTextBlock.Text = "No valid ID file path found.";
        }

        // Helper: Open file in default viewer
        private async System.Threading.Tasks.Task OpenFile(string filePath)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                await Launcher.LaunchFileAsync(file);
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Error opening file: {ex.Message}";
            }
        }

        // Helper: Get selected row from DataGrid
        private DataRowView GetSelectedClaim()
        {
            return ClaimsDataGrid.SelectedItem as DataRowView;
        }
    }
}
