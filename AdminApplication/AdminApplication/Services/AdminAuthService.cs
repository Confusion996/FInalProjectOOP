using AdminApplication.Models;
using System;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace AdminApplication.Services
{
    public static class AdminAuthService
    {
        private static string connectionString =
            "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        public static async Task<bool> SignUpAsync(AdminAccount admin)
        {
            using var conn = new OleDbConnection(connectionString);
            await conn.OpenAsync();

            // Check if username already exists
            string checkQuery = "SELECT COUNT(*) FROM AdminAccount WHERE Username = ?";
            using (var checkCmd = new OleDbCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("?", admin.Username);
                int exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                if (exists > 0)
                    return false; // Username already exists
            }

            // Insert new admin record
            string insertQuery = @"
        INSERT INTO AdminAccount (FirstName, LastName, Username, [Password]) 
        VALUES (?, ?, ?, ?)";
            using var insertCmd = new OleDbCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("?", admin.FirstName);
            insertCmd.Parameters.AddWithValue("?", admin.LastName);
            insertCmd.Parameters.AddWithValue("?", admin.Username);
            insertCmd.Parameters.AddWithValue("?", admin.Password);

            int rowsInserted = await insertCmd.ExecuteNonQueryAsync();
            return rowsInserted > 0;
        }


    }
    public class login 
    {
        private readonly string connectionString =
           "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";
        public AdminAccount ValidateAdminLogin(string username, string password)
        {
            string query = "SELECT * FROM AdminAccount WHERE Username = ? AND Password = ?";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", username);
                        cmd.Parameters.AddWithValue("?", password);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new AdminAccount
                                {
                                    AdminId = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Username = reader.GetString(3),
                                    Password = reader.GetString(4),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Admin login error: " + ex.Message);
            }

            return null;
        }

    }

}
