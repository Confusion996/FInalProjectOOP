using FinalProjectWinUi.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace FinalProjectWinUi.Helper
{
    public sealed partial class PlanPayment : Page
    {
        private OleDbConnection connection;
        private DataTable approvedPlansTable;
        private string userEmail = AppState.LoggedInUser.Email;

        public PlanPayment()
        {
            this.InitializeComponent();
            connection = new OleDbConnection(Connection.Conn);
            LoadApprovedPlans();
            QuestPDF.Settings.License = LicenseType.Community;
        }

        private void LoadApprovedPlans()
        {
            try
            {
                connection.Open();
                string query = @"
                    SELECT PlanHolderData.PlanId, PlanHolderData.Plantype, PlanHolderData.Balance, PlanHolderData.PlanName, PlanHolderData.InstallmentDue, PlanHolderData.PaymentStatus, PlanHolderData.ContractPrice
                    FROM (((CustomerAccount INNER JOIN PlanHolderData ON CustomerAccount.CustomerId = PlanHolderData.CustomerId) INNER JOIN PlanHolderAddress ON PlanHolderData.PlanId = PlanHolderAddress.PlanId) INNER JOIN PlanHolderCivilStatus ON PlanHolderData.PlanId = PlanHolderCivilStatus.PlanId) INNER JOIN PlanHolderInformation ON PlanHolderData.PlanId = PlanHolderInformation.PlanId
                    WHERE CustomerAccount.Email = ? AND PlanHolderData.Status = 'Approved';";

                OleDbCommand cmd = new OleDbCommand(query, connection);
                cmd.Parameters.AddWithValue("?", userEmail);

                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                approvedPlansTable = new DataTable();
                adapter.Fill(approvedPlansTable);
                ApprovedPlansDataGrid.ItemsSource = approvedPlansTable.DefaultView;

                StatusTextBlock.Text = approvedPlansTable.Rows.Count == 0
                    ? "No approved plans found."
                    : "Select a plan to pay.";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
            finally
            {
                connection.Close();
            }
        }

        private async void SubmitPayment_Click(object sender, RoutedEventArgs e)
        {
            if (ApprovedPlansDataGrid.SelectedItem is DataRowView selectedRow)
            {
                try
                {
                    if (!decimal.TryParse(PaymentAmountTextBox.Text, out decimal paymentAmount))
                    {
                        StatusTextBlock.Text = "Invalid payment amount format";
                        return;
                    }

                    if (PaymentMethodComboBox.SelectedItem is not ComboBoxItem methodItem)
                    {
                        StatusTextBlock.Text = "Payment method not selected";
                        return;
                    }

                    string paymentMethod = methodItem.Content.ToString();
                    decimal currentBalance = Convert.ToDecimal(selectedRow["Balance"]);
                    int planId = Convert.ToInt32(selectedRow["PlanId"]);
                    int customerId = AppState.LoggedInUser.CustomerId;

                    // Check if payment amount is greater than current balance
                    if (paymentAmount > currentBalance)
                    {
                        StatusTextBlock.Text = "Payment amount exceeds the current balance.";
                        return;
                    }

                    connection.Open();
                    using (OleDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert payment record
                            string insertQuery = @"INSERT INTO Payments 
                        (PlanId, CustomerId, Amount, [Date], Method) 
                        VALUES (?, ?, ?, ?, ?)";

                            using (OleDbCommand insertCmd = new OleDbCommand(insertQuery, connection, transaction))
                            {
                                insertCmd.Parameters.Add("@PlanId", OleDbType.Integer).Value = planId;
                                insertCmd.Parameters.Add("@CustomerId", OleDbType.Integer).Value = customerId;
                                insertCmd.Parameters.Add("@Amount", OleDbType.Currency).Value = paymentAmount;
                                insertCmd.Parameters.Add("@Date", OleDbType.Date).Value = DateTime.Now;
                                insertCmd.Parameters.Add("@Method", OleDbType.VarChar).Value = paymentMethod;
                                insertCmd.ExecuteNonQuery();
                            }

                            // Get PaymentId
                            int paymentId;
                            using (OleDbCommand getIdCmd = new OleDbCommand("SELECT @@IDENTITY", connection, transaction))
                            {
                                paymentId = Convert.ToInt32(getIdCmd.ExecuteScalar());
                            }

                            // Automatically define the receipt file path
                            string receiptDirectory = @"C:\Users\A.H\Documents\Receipt";
                            Directory.CreateDirectory(receiptDirectory); // Ensure directory exists

                            string receiptPath = Path.Combine(receiptDirectory, $"Payment_{paymentId}.pdf");


                            // Generate the receipt
                            GenerateReceipt(
                                paymentId: paymentId,
                                planId: planId,
                                customerId: customerId,
                                amount: paymentAmount,
                                date: DateTime.Now,
                                method: paymentMethod,
                                outputPath: receiptPath
                            );

                            // Update Payments with file path
                            string updateQuery = @"UPDATE Payments SET ProofOfPayment = ? WHERE PaymentId = ?";
                            using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection, transaction))
                            {
                                updateCmd.Parameters.Add("@ProofOfPayment", OleDbType.VarChar).Value = receiptPath;
                                updateCmd.Parameters.Add("@PaymentId", OleDbType.Integer).Value = paymentId;
                                updateCmd.ExecuteNonQuery();
                            }

                            // Update plan balance
                            decimal newBalance = currentBalance - paymentAmount;
                            string status = newBalance == 0 ? "Fully Paid" : (paymentAmount >= (Convert.ToDecimal(selectedRow["ContractPrice"]) / 2)) ? "Halfway Paid" : "Approved";

                            string balanceQuery = @"UPDATE PlanHolderData 
                                            SET Balance = ?, 
                                                Status = ? 
                                            WHERE PlanId = ?";
                            using (OleDbCommand balanceCmd = new OleDbCommand(balanceQuery, connection, transaction))
                            {
                                balanceCmd.Parameters.Add("@Balance", OleDbType.Currency).Value = newBalance;
                                balanceCmd.Parameters.Add("@Status", OleDbType.VarChar).Value = status;
                                balanceCmd.Parameters.Add("@PlanId", OleDbType.Integer).Value = planId;
                                balanceCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            // Show transaction complete message
                            ContentDialog dialog = new ContentDialog
                            {
                                Title = "Transaction Finished",
                                Content = $"Receipt saved at:\n{receiptPath}",
                                PrimaryButtonText = "Open Receipt",
                                CloseButtonText = "Close",
                                XamlRoot = this.Content.XamlRoot
                            };

                            var result = await dialog.ShowAsync();

                            if (result == ContentDialogResult.Primary)
                            {
                                await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(receiptPath));
                            }

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            StatusTextBlock.Text = $"Transaction failed: {ex.Message}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Error: {ex.Message}";
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                    LoadApprovedPlans();
                    PaymentAmountTextBox.Text = "";
                }
            }
            else
            {
                StatusTextBlock.Text = "Please select a plan first.";
            }
        }

        private void GenerateReceipt(int paymentId, int planId, int customerId, decimal amount, DateTime date, string method, string outputPath)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header
                    page.Header()
                        .Element(header =>
                        {
                            header.AlignCenter()
                                  .Text("Payment Receipt")
                                  .Bold().FontSize(18);
                        });

                    // Content
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text($"Payment ID: {paymentId}").FontSize(12);
                            column.Item().Text($"Plan ID: {planId}").FontSize(12);
                            column.Item().Text($"Customer ID: {customerId}").FontSize(12);
                            column.Item().Text($"Amount Paid: {amount:C}").FontSize(12);
                            column.Item().Text($"Date: {date:yyyy-MM-dd HH:mm}").FontSize(12);
                            column.Item().Text($"Payment Method: {method}").FontSize(12);
                        });

                    // Footer
                    page.Footer()
                        .Element(footer =>
                        {
                            footer.AlignCenter()
                                  .Text("Thank you for your payment!")
                                  .Italic().FontSize(10);
                        });
                });
            }).GeneratePdf(outputPath);
        }

        private void ApprovedPlansDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApprovedPlansDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if (decimal.TryParse(selectedRow["InstallmentDue"]?.ToString(), out decimal dueAmount))
                {
                    PaymentAmountTextBox.Text = dueAmount.ToString("F2");
                }
            }
        }
    }
}