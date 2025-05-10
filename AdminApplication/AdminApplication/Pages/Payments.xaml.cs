using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace AdminApplication.Pages
{
    public sealed partial class Payments : Page
    {
        private readonly string connectionString =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        public Payments()
        {
            this.InitializeComponent();
            this.Loaded += Payments_Loaded;
        }

        private void Payments_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPayments();
        }

        private void LoadPayments()
        {
            try
            {
                using OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();

                string query = @"
                    SELECT 
                        Payments.PaymentId,
                        Payments.PlanId,
                        Payments.Date,
                        Payments.Method,
                        Payments.ProofOfPayment,
                        CustomerAccount.Email
                    FROM 
                        (Payments 
                        INNER JOIN PlanHolderData ON Payments.PlanId = PlanHolderData.PlanId)
                        INNER JOIN CustomerAccount ON PlanHolderData.CustomerId = CustomerAccount.CustomerId;
                ";

                using OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                PaymentsDataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error Loading Payments",
                    Content = $"An error occurred: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
            }
        }

        private async void ViewReceipt_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentsDataGrid.SelectedItem is DataRowView row)
            {
                string receiptPath = row["ProofOfPayment"]?.ToString();
                if (!string.IsNullOrWhiteSpace(receiptPath) && File.Exists(receiptPath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = receiptPath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Cannot Open File",
                            Content = $"Error opening receipt: {ex.Message}",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
                else
                {
                    ContentDialog notFoundDialog = new ContentDialog
                    {
                        Title = "Receipt Not Found",
                        Content = "The selected receipt file could not be found on the system.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await notFoundDialog.ShowAsync();
                }
            }
        }
    }
}
