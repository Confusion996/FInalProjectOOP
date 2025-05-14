using FinalProjectWinUi.Models;
using FinalProjectWinUi.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Data.OleDb;

namespace FinalProjectWinUi.Helper
{
    public sealed partial class UpdateInfoPage : Page
    {
        public UpdateInfoPage()
        {
            this.InitializeComponent();
        }

        private async void OnUpdateInfoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string planId = PlanIdBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(planId))
                {
                    var invalidInputDialog = new ContentDialog
                    {
                        Title = "Invalid Input",
                        Content = "Please enter a valid Plan ID.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await invalidInputDialog.ShowAsync();
                    return;
                }

                // Prepare data
                string lastName = string.IsNullOrWhiteSpace(LastNameBox.Text) ? null : LastNameBox.Text;
                string firstName = string.IsNullOrWhiteSpace(FirstNameBox.Text) ? null : FirstNameBox.Text;
                string middleName = string.IsNullOrWhiteSpace(MiddleNameBox.Text) ? null : MiddleNameBox.Text;
                string contactNumber = string.IsNullOrWhiteSpace(ContactNumberBox.Text) ? null : ContactNumberBox.Text;

                // Remove time from DateTime
                DateTime? dateOfBirth = DateOfBirthPicker.Date != null
                    ? DateOfBirthPicker.Date.DateTime.Date // This will remove the time part and keep only the date.
                    : (DateTime?)null;

                string placeOfBirth = string.IsNullOrWhiteSpace(PlaceOfBirthBox.Text) ? null : PlaceOfBirthBox.Text;
                int age = int.TryParse(AgeBox.Text, out int parsedAge) ? parsedAge : 0;
                string sex = (SexBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string height = string.IsNullOrWhiteSpace(HeightBox.Text) ? null : HeightBox.Text;
                string weight = string.IsNullOrWhiteSpace(WeightBox.Text) ? null : WeightBox.Text;

                string lot = string.IsNullOrWhiteSpace(LotNumberBox.Text) ? null : LotNumberBox.Text;
                string street = string.IsNullOrWhiteSpace(StreetBox.Text) ? null : StreetBox.Text;
                string barangay = string.IsNullOrWhiteSpace(BarangayBox.Text) ? null : BarangayBox.Text;
                string city = string.IsNullOrWhiteSpace(CityBox.Text) ? null : CityBox.Text;
                string province = string.IsNullOrWhiteSpace(ProvinceBox.Text) ? null : ProvinceBox.Text;
                string zipCode = string.IsNullOrWhiteSpace(ZipCodeBox.Text) ? null : ZipCodeBox.Text;

                string nationality = string.IsNullOrWhiteSpace(NationalityBox.Text) ? null : NationalityBox.Text;
                string civilStatus = (CivilStatusBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string occupation = string.IsNullOrWhiteSpace(OccupationBox.Text) ? null : OccupationBox.Text;

                // Connection string
                string connectionString = Connection.Conn;

                using (var conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    try
                    {
                        // Update PlanHolderInformation
                        var cmdInfo = new OleDbCommand(@"
                UPDATE PlanHolderInformation SET 
                    Lastname = ?, Firstname = ?, Middlename = ?, ContactNo = ?, 
                    DateOfBirth = ?, PlaceOfBirth = ?, Age = ?, Sex = ?, Height = ?, Weight = ?
                WHERE PlanId = ?", conn);

                        cmdInfo.Parameters.AddWithValue("?", (object)lastName ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", (object)firstName ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", (object)middleName ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", (object)contactNumber ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", (object)dateOfBirth ?? DBNull.Value); // Pass the date-only value
                        cmdInfo.Parameters.AddWithValue("?", (object)placeOfBirth ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", age);
                        cmdInfo.Parameters.AddWithValue("?", (object)sex ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", (object)height ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", (object)weight ?? DBNull.Value);
                        cmdInfo.Parameters.AddWithValue("?", planId);
                        cmdInfo.ExecuteNonQuery();

                        // Update PlanHolderAddress
                        var cmdAddress = new OleDbCommand(@"
                UPDATE PlanHolderAddress SET 
                    Lot = ?, Street = ?, Barangay = ?, City = ?, Province = ?, ZipCode = ?
                WHERE PlanId = ?", conn);

                        cmdAddress.Parameters.AddWithValue("?", (object)lot ?? DBNull.Value);
                        cmdAddress.Parameters.AddWithValue("?", (object)street ?? DBNull.Value);
                        cmdAddress.Parameters.AddWithValue("?", (object)barangay ?? DBNull.Value);
                        cmdAddress.Parameters.AddWithValue("?", (object)city ?? DBNull.Value);
                        cmdAddress.Parameters.AddWithValue("?", (object)province ?? DBNull.Value);
                        cmdAddress.Parameters.AddWithValue("?", (object)zipCode ?? DBNull.Value);
                        cmdAddress.Parameters.AddWithValue("?", planId);
                        cmdAddress.ExecuteNonQuery();

                        // Update PlanHolderCivilStatus
                        var cmdCivil = new OleDbCommand(@"
                UPDATE PlanHolderCivilStatus SET 
                    Nationality = ?, CivilStatus = ?, Occupation = ?
                WHERE PlanId = ?", conn);

                        cmdCivil.Parameters.AddWithValue("?", (object)nationality ?? DBNull.Value);
                        cmdCivil.Parameters.AddWithValue("?", (object)civilStatus ?? DBNull.Value);
                        cmdCivil.Parameters.AddWithValue("?", (object)occupation ?? DBNull.Value);
                        cmdCivil.Parameters.AddWithValue("?", planId);
                        cmdCivil.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log the exception to show details in a dialog
                        string errorMessage = $"Failed to execute query:\n\n{ex.Message}\n\n" +
                                              $"PlanId: {planId}\n" +
                                              $"LastName: {lastName}\n" +
                                              $"FirstName: {firstName}\n" +
                                              $"MiddleName: {middleName}\n" +
                                              $"ContactNumber: {contactNumber}\n" +
                                              $"DateOfBirth: {dateOfBirth}\n" +
                                              $"PlaceOfBirth: {placeOfBirth}\n" +
                                              $"Age: {age}\n" +
                                              $"Sex: {sex}\n" +
                                              $"Height: {height}\n" +
                                              $"Weight: {weight}\n" +
                                              $"Lot: {lot}\n" +
                                              $"Street: {street}\n" +
                                              $"Barangay: {barangay}\n" +
                                              $"City: {city}\n" +
                                              $"Province: {province}\n" +
                                              $"ZipCode: {zipCode}\n" +
                                              $"Nationality: {nationality}\n" +
                                              $"CivilStatus: {civilStatus}\n" +
                                              $"Occupation: {occupation}";

                        var errorDialog = new ContentDialog
                        {
                            Title = "Database Update Error",
                            Content = errorMessage,
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                        return;
                    }
                }

                // Success Dialog
                var successDialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Information updated successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Show a general error dialog
                var generalErrorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to update information: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await generalErrorDialog.ShowAsync();
            }
        }




    }
}
