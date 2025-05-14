using System;
using System.Data.OleDb;
using FinalProjectWinUi.Models;

namespace FinalProjectWinUi.Services
{
    public class UpdateInfo
    {
        public void UpdateUserInfo(UserApplyPlan request)
        {
            using (var connection = new OleDbConnection(Connection.Conn))
            {
                connection.Open();

                // Update PlanHolderInformation
                using (var cmd = new OleDbCommand(@"
                    UPDATE PlanHolderInformation 
                    SET [Lastname] = ?, 
                        [Firstname] = ?, 
                        [Middlename] = ?, 
                        [ContactNo] = ?, 
                        [DateOfBirth] = ?, 
                        [PlaceOfBirth] = ?, 
                        [Age] = ?, 
                        [Sex] = ?, 
                        [Height] = ?, 
                        [Weight] = ?
                    WHERE [PlanId] = ?", connection))
                {
                    cmd.Parameters.AddWithValue("?", request.LastName);        // Short Text
                    cmd.Parameters.AddWithValue("?", request.FirstName);       // Short Text
                    cmd.Parameters.AddWithValue("?", request.MiddleName);      // Short Text
                    cmd.Parameters.AddWithValue("?", request.ContactNumber);   // Short Text
                    cmd.Parameters.AddWithValue("?", request.DateOfBirth);     // Date/Time
                    cmd.Parameters.AddWithValue("?", request.PlaceOfBirth);    // Short Text
                    cmd.Parameters.AddWithValue("?", request.Age);             // Number
                    cmd.Parameters.AddWithValue("?", request.Sex);             // Short Text
                    cmd.Parameters.AddWithValue("?", request.Height);          // Short Text
                    cmd.Parameters.AddWithValue("?", request.Weight);          // Short Text

                    cmd.Parameters.AddWithValue("?", request.PlanId);          // Number (should be an int/long)
                    cmd.ExecuteNonQuery();
                }

                // Update PlanHolderAddress
                using (var cmd = new OleDbCommand(@"
                    UPDATE PlanHolderAddress 
                    SET [Lot] = ?, 
                        [Street] = ?, 
                        [Barangay] = ?, 
                        [City] = ?, 
                        [Province] = ?, 
                        [ZipCode] = ? 
                    WHERE [PlanId] = ?", connection))
                {
                    cmd.Parameters.AddWithValue("?", request.Lot);             // Short Text
                    cmd.Parameters.AddWithValue("?", request.Street);          // Short Text
                    cmd.Parameters.AddWithValue("?", request.Barangay);        // Short Text
                    cmd.Parameters.AddWithValue("?", request.City);            // Short Text
                    cmd.Parameters.AddWithValue("?", request.Province);        // Short Text
                    cmd.Parameters.AddWithValue("?", request.ZipCode);         // Short Text
                    cmd.Parameters.AddWithValue("?", request.PlanId);          // Number (int/long)
                    cmd.ExecuteNonQuery();
                }

                // Update PlanHolderCivilStatus
                using (var cmd = new OleDbCommand(@"
                    UPDATE PlanHolderCivilStatus 
                    SET [Nationality] = ?, 
                        [CivilStatus] = ?, 
                        [Occupation] = ?
                    WHERE [PlanId] = ?", connection))
                {
                    cmd.Parameters.AddWithValue("?", request.Nationality); // Short Text
                    cmd.Parameters.AddWithValue("?", request.CivilStatus); // Short Text
                    cmd.Parameters.AddWithValue("?", request.Occupation);  // Short Text
                    cmd.Parameters.AddWithValue("?", request.PlanId);      // Number (int/long)
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
