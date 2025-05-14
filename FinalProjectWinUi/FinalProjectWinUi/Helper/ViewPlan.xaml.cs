using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Data;
using System.Data.OleDb;
using FinalProjectWinUi.Services;

namespace FinalProjectWinUi.Helper
{
    public sealed partial class ViewPlan : Page
    {
        private OleDbConnection connection;
        private DataTable userPlansTable;
        private string userEmail = AppState.LoggedInUser.Email;

        public ViewPlan()
        {
            this.InitializeComponent();
            connection = new OleDbConnection(Connection.Conn);
            LoadUserPlans();
        }

        private void LoadUserPlans()
        {
            try
            {
                connection.Open();
                string query = @"
SELECT 
    PlanHolderData.PlanId, 
    PlanHolderData.Plantype, 
    PlanHolderData.PlanName, 
    PlanHolderData.DateApplied, 
    PlanHolderData.Status, 
    PlanHolderData.ContractPrice, 
    PlanHolderData.Balance, 
    PlanHolderInformation.Lastname, 
    PlanHolderInformation.Firstname, 
    PlanHolderInformation.Middlename
FROM 
    (((CustomerAccount 
    INNER JOIN PlanHolderData ON CustomerAccount.CustomerId = PlanHolderData.CustomerId) 
    INNER JOIN PlanHolderAddress ON PlanHolderData.PlanId = PlanHolderAddress.PlanId) 
    INNER JOIN PlanHolderCivilStatus ON PlanHolderData.PlanId = PlanHolderCivilStatus.PlanId) 
    INNER JOIN PlanHolderInformation ON PlanHolderData.PlanId = PlanHolderInformation.PlanId
WHERE 
    CustomerAccount.Email = ?;";

                OleDbCommand cmd = new OleDbCommand(query, connection);
                cmd.Parameters.AddWithValue("?", userEmail);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                userPlansTable = new DataTable();
                adapter.Fill(userPlansTable);

                // Add PlanholderName column
                userPlansTable.Columns.Add("PlanholderName", typeof(string));
                foreach (DataRow row in userPlansTable.Rows)
                {
                    string firstname = row["Firstname"].ToString();
                    string middlename = row["Middlename"].ToString();
                    string lastname = row["Lastname"].ToString();
                    row["PlanholderName"] = $"{firstname} {middlename} {lastname}".Trim();
                }

                MyPlansDataGrid.ItemsSource = userPlansTable.DefaultView;

                StatusTextBlock.Text = userPlansTable.Rows.Count == 0
                    ? "No plans found for your account."
                    : "Your plans have been loaded.";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading plans: {ex.Message}";
            }
            finally
            {
                connection.Close();
            }
        }


        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            var planId = (sender as Button)?.Tag?.ToString();
            // TODO: Load and show full plan details for planId
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var planId = (sender as Button)?.Tag?.ToString();
            // TODO: Generate and save PDF contract for planId
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            var planId = (sender as Button)?.Tag?.ToString();
            // TODO: Open transfer request dialog for planId
        }

    }
}
