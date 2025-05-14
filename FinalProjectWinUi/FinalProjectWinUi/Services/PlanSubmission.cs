using FinalProjectWinUi.Models;
using System;
using System.Data.OleDb;

namespace FinalProjectWinUi.Services
{
    public class PlanSubmissionService
    {
        private readonly string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\A.H\\source\\repos\\FinalProjectWInUI\\FinalProjectWinUi\\Database\\CustomerDatabase.accdb";

        public void SubmitPlan(UserApplyPlan request)
        {
            using (var connection = new OleDbConnection(Connection.Conn))
            {
                connection.Open();

                // Insert into PlanHolderData
                var planDataCmd = new OleDbCommand("INSERT INTO PlanHolderData (PlanType,PlanName, ContractPrice, ModeOfPayment, PaymentMethod, InstallmentDue, Status, Balance, CustomerId, PaymentStatus, DateApplied, ClaimStatus) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", connection);
                planDataCmd.Parameters.AddWithValue("?", request.PlanType);
                planDataCmd.Parameters.AddWithValue("?", request.PlanName);
                planDataCmd.Parameters.AddWithValue("?", request.ContractPrice);
                planDataCmd.Parameters.AddWithValue("?", request.ModeOfPayment);
                planDataCmd.Parameters.AddWithValue("?", request.PaymentMethod);
                planDataCmd.Parameters.AddWithValue("?", request.InstallmentDue);
                planDataCmd.Parameters.AddWithValue("?", request.Status);
                planDataCmd.Parameters.AddWithValue("?", request.Balance);
                planDataCmd.Parameters.AddWithValue("?", request.CustomerId);
                planDataCmd.Parameters.AddWithValue("?", request.PaymentStatus);
                planDataCmd.Parameters.AddWithValue("?", request.DateApplied);
                planDataCmd.Parameters.AddWithValue("?", request.ClaimStatus);
                planDataCmd.ExecuteNonQuery();

                // Get the last inserted PlanId
                planDataCmd = new OleDbCommand("SELECT @@IDENTITY", connection);
                int planId = Convert.ToInt32(planDataCmd.ExecuteScalar());

                // Insert into PlanHolderInformation
                var infoCmd = new OleDbCommand(@"INSERT INTO PlanHolderInformation 
                    (PlanId, LastName, FirstName, MiddleName, ContactNo, DateOfBirth, PlaceOfBirth, Age, Sex, Height, Weight) 
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", connection);

                infoCmd.Parameters.AddWithValue("?", planId);
                infoCmd.Parameters.AddWithValue("?", request.LastName);
                infoCmd.Parameters.AddWithValue("?", request.FirstName);
                infoCmd.Parameters.AddWithValue("?", request.MiddleName);
                infoCmd.Parameters.AddWithValue("?", request.ContactNumber);
                infoCmd.Parameters.AddWithValue("?", request.DateOfBirth);
                infoCmd.Parameters.AddWithValue("?", request.PlaceOfBirth);
                infoCmd.Parameters.AddWithValue("?", request.Age);
                infoCmd.Parameters.AddWithValue("?", request.Sex);
                infoCmd.Parameters.AddWithValue("?", request.Height);
                infoCmd.Parameters.AddWithValue("?", request.Weight);
                infoCmd.ExecuteNonQuery();

                // Insert into PlanHolderAddress
                var addressCmd = new OleDbCommand("INSERT INTO PlanHolderAddress (PlanId, Lot, Street, Barangay, City, Province, ZipCode) VALUES (?, ?, ?, ?, ?, ?, ?)", connection);
                addressCmd.Parameters.AddWithValue("?", planId);
                addressCmd.Parameters.AddWithValue("?", request.Lot);
                addressCmd.Parameters.AddWithValue("?", request.Street);
                addressCmd.Parameters.AddWithValue("?", request.Barangay);
                addressCmd.Parameters.AddWithValue("?", request.City);
                addressCmd.Parameters.AddWithValue("?", request.Province);
                addressCmd.Parameters.AddWithValue("?", request.ZipCode);
                addressCmd.ExecuteNonQuery();

                // Insert into PlanHolderCivilStatus
                var civilCmd = new OleDbCommand("INSERT INTO PlanHolderCivilStatus (PlanId, Nationality, CivilStatus, Occupation) VALUES (?, ?, ?, ?)", connection);
                civilCmd.Parameters.AddWithValue("?", planId);
                civilCmd.Parameters.AddWithValue("?", request.Nationality);
                civilCmd.Parameters.AddWithValue("?", request.CivilStatus);
                civilCmd.Parameters.AddWithValue("?", request.Occupation);
                civilCmd.ExecuteNonQuery();

                // You can also insert to CustomerPlans if needed.
            }
        }
    }
}
