using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace AdminApplication.Services
{
    public static class PlanDataService
    {
        private static readonly string connectionString =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        // Get the total sales within a specific date range
        public static async Task<double> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.Run(() =>
            {
                double total = 0;
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // Create the command with placeholders
                    var cmd = new OleDbCommand("SELECT COUNT([PaymentId]) FROM [Payments] WHERE [Date] BETWEEN ? AND ?", conn);

                    // Format the date explicitly for MS Access
                    cmd.Parameters.Add(new OleDbParameter("?", OleDbType.Date) { Value = startDate.ToString("MM/dd/yyyy") });
                    cmd.Parameters.Add(new OleDbParameter("?", OleDbType.Date) { Value = endDate.ToString("MM/dd/yyyy") });

                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        total = Convert.ToDouble(result);
                }
                return total;
            });
        }

        // Get the total payments within a specific date range
        public static async Task<double> GetTotalPaymentsAsync(DateTime startDate, DateTime endDate)
        {
            return await Task.Run(() =>
            {
                double total = 0;
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new OleDbCommand("SELECT SUM(Amount) FROM Payments WHERE [Date] BETWEEN ? AND ?", conn);

                    // Format the dates explicitly to MM/DD/YYYY for MS Access compatibility
                    cmd.Parameters.Add(new OleDbParameter("?", OleDbType.Date) { Value = startDate.ToString("MM/dd/yyyy") });
                    cmd.Parameters.Add(new OleDbParameter("?", OleDbType.Date) { Value = endDate.ToString("MM/dd/yyyy") });

                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        total = Convert.ToDouble(result);
                }
                return total;
            });
        }


        // Get the total outstanding balances (No date range)
        public static async Task<double> GetOutstandingBalancesAsync()
        {
            return await Task.Run(() =>
            {
                double total = 0;
                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new OleDbCommand("SELECT SUM(Balance) FROM PlanHolderData", conn);
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                        total = Convert.ToDouble(result);
                }
                return total;
            });
        }

        // ✅ New: Get total sales for a specific plan type or plan name
        public static async Task<Dictionary<string, double>> GetSalesByPlanNamesAsync(DateTime startDate, DateTime endDate)
        {
            var salesData = new Dictionary<string, double>();

            // Define the plan names you're interested in
            string[] planNames = {
        "Basic Plan", "Standard Plan", "Premium Plan",
        "Deluxe Plan", "Simple Cremation", "Memorial Cremation", "Premium Cremation"
    };

            // Open a connection to the database
            using (var conn = new OleDbConnection(connectionString))
            {
                await conn.OpenAsync(); // Use async opening of the connection

                foreach (var planName in planNames)
                {
                    // Build the query for each plan
                    string query = @"
                SELECT SUM(p.Amount) 
                FROM Payments p 
                INNER JOIN PlanHolderData d ON p.PlanId = d.PlanId 
                WHERE d.PlanName = ? AND p.Date BETWEEN ? AND ?";

                    // Set up the command
                    using (var cmd = new OleDbCommand(query, conn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("?", planName);  // PlanType
                        cmd.Parameters.AddWithValue("?", startDate); // Start date
                        cmd.Parameters.AddWithValue("?", endDate);   // End date

                        // Execute the query and get the result
                        var result = await cmd.ExecuteScalarAsync();

                        // Handle the result and store it in the dictionary
                        double total = result != DBNull.Value ? Convert.ToDouble(result) : 0;
                        salesData[planName] = total;
                    }
                }
            }

            // Return the dictionary containing the total sales for each plan
            return salesData;
        }


    }
}
