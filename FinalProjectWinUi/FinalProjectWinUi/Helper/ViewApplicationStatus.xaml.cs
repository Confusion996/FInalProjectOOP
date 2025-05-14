using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using FinalProjectWinUi.Models;
using FinalProjectWinUi.Services;

namespace FinalProjectWinUi.Helper
{
    public sealed partial class ViewApplicationStatus : Page
    {
        private string _connectionString = Connection.Conn; // Replace this
        private string _userEmail = AppState.LoggedInUser.Email; // Assuming you're using a static AppState

        public ViewApplicationStatus()
        {
            this.InitializeComponent();
            LoadPlanApplications();
            LoadClaimApplications();
        }

        private void LoadPlanApplications()
        {
            try
            {
                using (var connection = new OleDbConnection(_connectionString))
                {
                    var command = new OleDbCommand(
                        @"SELECT PlanHolderData.PlanId, PlanHolderData.Plantype, PlanHolderData.PlanName, 
                          PlanHolderData.ContractPrice, PlanHolderData.ModeOfPayment, PlanHolderData.Status, 
                          CustomerAccount.Email
                  FROM PlanHolderData 
                  INNER JOIN CustomerAccount ON PlanHolderData.CustomerId = CustomerAccount.CustomerId 
                  WHERE CustomerAccount.Email = ?", connection);

                    // Bind the parameter to the query
                    command.Parameters.AddWithValue("?", _userEmail);

                    var adapter = new OleDbDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the data to the grid
                    PlanApplicationsGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog("Failed to load plan applications: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Error loading plan applications: " + ex.Message);
            }
        }


        private void LoadClaimApplications()
        {
            try
            {
                using (var connection = new OleDbConnection(_connectionString))
                {
                    var command = new OleDbCommand(@"
                SELECT ClaimPlan.ClaimId, ClaimPlan.PlanId, ClaimPlan.ClaimantName, 
                       ClaimPlan.RelationshipToPlanHolder, ClaimPlan.ContactNumber, 
                       ClaimPlan.EmailAddress, ClaimPlan.DateOfClaim, ClaimPlan.ClaimStatus
                FROM (((CustomerAccount
                INNER JOIN PlanHolderData ON CustomerAccount.CustomerId = PlanHolderData.CustomerId)
                INNER JOIN ClaimPlan ON PlanHolderData.PlanId = ClaimPlan.PlanId AND CustomerAccount.CustomerId = ClaimPlan.CustomerId)
                )
                WHERE CustomerAccount.Email = ?", connection);

                    command.Parameters.AddWithValue("?", _userEmail);

                    var adapter = new OleDbDataAdapter(command);
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ClaimApplicationsGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog("Failed to load claim applications: " + ex.Message);
            }
        }



        private async void ShowErrorDialog(string errorMessage)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = errorMessage,
                CloseButtonText = "OK"
            };

            // Ensure the XamlRoot is set to the current page's XamlRoot
            errorDialog.XamlRoot = this.XamlRoot;

            await errorDialog.ShowAsync();
        }

    }
}
