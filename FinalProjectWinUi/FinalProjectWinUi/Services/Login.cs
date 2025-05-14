using FinalProjectWinUi.Models;
using System;
using System.Data.OleDb;

namespace FinalProjectWinUi.Services
{
    public class Login
    {
        public CustomerAccount ValidateLogin(string email, string password)
        {
            string query = "SELECT * FROM CustomerAccount WHERE Email = ? AND Password = ?";

            try
            {
                using (OleDbConnection conn = new OleDbConnection(Connection.Conn))
                {
                    conn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", email);
                        cmd.Parameters.AddWithValue("?", password);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Assuming your table has columns: Id, Name, Email, Password
                                return new CustomerAccount
                                {
                                    CustomerId = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3),
                                    Password = reader.GetString(4),                                  
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login error: " + ex.Message);
            }

            return null;
        }


    }

}
