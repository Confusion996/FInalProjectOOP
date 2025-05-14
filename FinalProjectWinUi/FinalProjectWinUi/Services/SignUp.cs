using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProjectWinUi.Models;
using System.Data.OleDb;
namespace FinalProjectWinUi.Services
{
    public class SignUp
    {
        private string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        public bool IsEmailTaken(string email)
        {
            using var conn = new OleDbConnection(Connection.Conn);
            conn.Open();

            var cmd = new OleDbCommand("SELECT COUNT(*) FROM CustomerAccount WHERE Email = ?", conn);
            cmd.Parameters.AddWithValue("?", email);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public bool RegisterCustomer(CustomerAccount customer)
        {
            if (IsEmailTaken(customer.Email)) return false;

            using var conn = new OleDbConnection(Connection.Conn);
            conn.Open();

            var cmd = new OleDbCommand("INSERT INTO [CustomerAccount] ([FirstName], [LastName], [Email], [Password]) VALUES (?, ?, ?, ?)", conn);

            cmd.Parameters.AddWithValue("?", customer.FirstName);
            cmd.Parameters.AddWithValue("?", customer.LastName);
            cmd.Parameters.AddWithValue("?", customer.Email);
            cmd.Parameters.AddWithValue("?", customer.Password);

            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }
        public static async Task<bool> UpdateUserPasswordAsync(string email, string newPassword)
        {
            string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";
            using var conn = new OleDbConnection(connectionString);
            await conn.OpenAsync();

            string query = "UPDATE CustomerAccount SET [Password] = ? WHERE [Email] = ?";
            using var command = new OleDbCommand(query, conn);
            command.Parameters.AddWithValue("?", newPassword);
            command.Parameters.AddWithValue("?", email);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

    }
}
