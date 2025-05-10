using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Data;
using System.Data.OleDb;

namespace AdminApplication.Pages
{
    public sealed partial class ApplicationManagement : Page
    {
        private readonly string connectionString =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        public ApplicationManagement()
        {
            this.InitializeComponent();
            LoadPlans();
            PlansDataGrid.SelectionChanged += PlansDataGrid_SelectionChanged;
        }

        private int? selectedPlanId = null;

        private void LoadPlans()
        {
            try
            {
                using OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT 
                        PlanHolderData.PlanId, 
                        PlanHolderData.Plantype, 
                        PlanHolderData.PlanName,
                        PlanHolderData.ContractPrice, 
                        PlanHolderData.ModeOfPayment, 
                        PlanHolderData.Status, 
                        CustomerAccount.Email
                    FROM 
                        (((CustomerAccount 
                        INNER JOIN PlanHolderData ON CustomerAccount.CustomerId = PlanHolderData.CustomerId) 
                        INNER JOIN PlanHolderAddress ON PlanHolderData.PlanId = PlanHolderAddress.PlanId) 
                        INNER JOIN PlanHolderCivilStatus ON PlanHolderData.PlanId = PlanHolderCivilStatus.PlanId) 
                        INNER JOIN PlanHolderInformation ON PlanHolderData.PlanId = PlanHolderInformation.PlanId;
                ";

                using OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                PlansDataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Error loading plans: {ex.Message}";
            }
        }

        private void PlansDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlansDataGrid.SelectedItem is DataRowView row)
            {
                selectedPlanId = Convert.ToInt32(row["PlanId"]);
            }
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Approved");
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus("Rejected");
        }

        private void UpdateStatus(string newStatus)
        {
            if (selectedPlanId == null)
            {
                StatusMessageTextBlock.Text = "Please select a plan from the list.";
                return;
            }

            try
            {
                using OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();

                string updateQuery = "UPDATE PlanHolderData SET Status = ? WHERE PlanID = ?";
                using OleDbCommand cmd = new OleDbCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("?", newStatus);
                cmd.Parameters.AddWithValue("?", selectedPlanId.Value);

                int rowsAffected = cmd.ExecuteNonQuery();

                StatusMessageTextBlock.Text = rowsAffected > 0
                    ? $"Plan ID {selectedPlanId} status updated to '{newStatus}'."
                    : "Failed to update status.";

                LoadPlans();
                selectedPlanId = null;
            }
            catch (Exception ex)
            {
                StatusMessageTextBlock.Text = $"Error updating status: {ex.Message}";
            }
        }
    }
}
