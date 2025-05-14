using FinalProjectWinUi.Models;
using FinalProjectWinUi.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Data;
using System.Data.OleDb;

namespace FinalProjectWinUi.Helper
{
    public sealed partial class TransactionHistory : Page
    {
        private string userEmail = AppState.LoggedInUser.Email;
        private OleDbConnection connection;
        private DataTable PlanDataTable;
        public TransactionHistory()
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
            SELECT PlanHolderData.PlanId, 
                   PlanHolderData.Plantype, 
                   PlanHolderData.PlanName, 
                   PlanHolderData.PaymentStatus, 
                   PlanHolderData.ContractPrice, 
                   PlanHolderData.Balance, 
                   PlanHolderData.DateApplied
            FROM CustomerAccount
            INNER JOIN PlanHolderData ON CustomerAccount.CustomerId = PlanHolderData.CustomerId
            WHERE CustomerAccount.Email = ? AND PlanHolderData.Status = 'Approved';";

                OleDbCommand cmd = new OleDbCommand(query, connection);
                cmd.Parameters.AddWithValue("?", userEmail);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                PlanDataTable = new DataTable();
                adapter.Fill(PlanDataTable);

                // Log the number of rows
                System.Diagnostics.Debug.WriteLine($"Rows returned: {PlanDataTable.Rows.Count}");

                PlanDataGrid.ItemsSource = PlanDataTable.DefaultView;

                // Update status message
                StatusTextBlock.Text = PlanDataTable.Rows.Count == 0
                    ? "No approved plans found."
                    : "Select a plan to pay.";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
            }
            catch (Exception ex)
            {
                // Update status message for errors
                StatusTextBlock.Text = $"Failed to load plans: {ex.Message}";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
            finally
            {
                connection.Close();
            }
        }


        private void ViewTransactionHistory_Click(object sender, RoutedEventArgs e)
        {
            if (PlanDataGrid.SelectedItem is DataRowView selectedRow)
            {
                try
                {
                    // Extract PlanId from the selected row
                    int planId = Convert.ToInt32(selectedRow["PlanId"]);

                    // Fetch transactions by PlanId
                    LoadTransactionHistory(planId);
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Error loading transaction history: {ex.Message}";
                    StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                }
            }
            else
            {
                StatusTextBlock.Text = "Please select a plan first.";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
            }
        }

        private void LoadTransactionHistory(int planId)
        {
            try
            {
                // Initialize the OleDbConnection for direct database access
                using (var connection = new OleDbConnection(Connection.Conn))
                {
                    connection.Open();
                    string query = @"
                SELECT Payments.PaymentId, 
                       Payments.PlanId, 
                       Payments.Amount, 
                       Payments.Date, 
                       Payments.Method, 
                       Payments.ProofOfPayment
                FROM (CustomerAccount 
                INNER JOIN Payments ON CustomerAccount.CustomerId = Payments.CustomerId) 
                INNER JOIN PlanHolderData ON PlanHolderData.PlanId = Payments.PlanId 
                                              AND CustomerAccount.CustomerId = PlanHolderData.CustomerId
                WHERE Payments.PlanId = ?
                ORDER BY Payments.Date DESC";

                    OleDbCommand cmd = new OleDbCommand(query, connection);
                    cmd.Parameters.AddWithValue("?", planId);

                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataTable transactionTable = new DataTable();
                    adapter.Fill(transactionTable);
                    TransactionDataGrid.ItemsSource = transactionTable.DefaultView;

                    // Check if the DataTable is empty
                    if (transactionTable.Rows.Count == 0)
                    {
                        StatusTextBlock.Text = "No transactions found for the selected plan.";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                    }
                    else
                    {
                        StatusTextBlock.Text = "Transactions loaded.";
                        StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                    }
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading transaction history: {ex.Message}";
                StatusTextBlock.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
            }
        }
    }
}
