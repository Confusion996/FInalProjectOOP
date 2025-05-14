using FinalProjectWinUi.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using FinalProjectWinUi.Models;
using FinalProjectWinUi.Pages;
using FinalProjectWinUi.Services;
using System.Threading.Tasks;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FinalProjectWinUi.Helper
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ApplyPlanForm : Page
    {
        public ApplyPlanForm()
        {
            this.InitializeComponent();
        }
        private void PlanTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = PlanTypeComboBox.SelectedItem as ComboBoxItem;
            string selectedPlanType = selectedItem?.Content.ToString();

            // Show the relevant plan options based on selected type
            if (selectedPlanType == "Traditional Plans")
            {
                TraditionalPlansPanel.Visibility = Visibility.Visible;
                CremationPlansPanel.Visibility = Visibility.Collapsed;
            }
            else if (selectedPlanType == "Cremation Plans")
            {
                TraditionalPlansPanel.Visibility = Visibility.Collapsed;
                CremationPlansPanel.Visibility = Visibility.Visible;
            }
         
        }
        private void UpdatePricing()
        {
            string selectedPlan = GetSelectedPlan();
            if (string.IsNullOrEmpty(selectedPlan)) return;

            if (!PricingData.AllPlanPrices.TryGetValue(selectedPlan, out var plan)) return;

            double contractPrice = plan.ContractPrice;
            double totalPayable = contractPrice;

            string insurance = (InsuranceComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (insurance == "Insurable")
            {
                totalPayable += contractPrice * 0.10; // Add 10% insurance
            }

            double installment = totalPayable;
            string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
         
            if (paymentMethod == "Spot Cash")
            {
                installment = totalPayable; // Pay everything upfront
            }
            else if (paymentMethod == "Terms 5 years")
            {
                string paymentMode = (ModeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (paymentMode == "Monthly") installment = totalPayable / 60; // 5 years = 60 months
                else if (paymentMode == "Quarterly") installment = totalPayable / 20; // 5 years = 20 quarters
                else if (paymentMode == "Semi-Annually") installment = totalPayable / 10; // 5 years = 10 semi-annuals
                else if (paymentMode == "Annual") installment = totalPayable / 5; // 5 years = 5 annual payments
            }

            ContractPriceTextBox.Text = contractPrice.ToString("N2");
            TotalAmountPayableTextBox.Text = totalPayable.ToString("N2");
            InstallmentDueTextBox.Text = installment.ToString("N2");
        }

        private void Input_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePricing();
        }
        
        private string GetSelectedPlan()
        {
            foreach (var panel in new[] { TraditionalPlansPanel, CremationPlansPanel })
            {
                foreach (var child in panel.Children)
                {
                    if (child is RadioButton radio && radio.IsChecked == true)
                        return radio.Content.ToString();
                }
            }
            return "";
        }
        private async void SubmitFormButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedPlan = null;

            if (TraditionalPlansPanel.Visibility == Visibility.Visible)
            {
                if (CasketOption.IsChecked == true) selectedPlan = "Basic Plan";
                else if (CasketProOption.IsChecked == true) selectedPlan = "Standard Plan";
                else if (CasketProMaxOption.IsChecked == true) selectedPlan = "Premium Plan";
                else if (CasketUltraOption.IsChecked == true) selectedPlan = "Deluxe Plan";
            }
            else if (CremationPlansPanel.Visibility == Visibility.Visible)
            {
                if (RareOption.IsChecked == true) selectedPlan = "Simple Cremation";
                else if (MediumRareOption.IsChecked == true) selectedPlan = "Memorial Cremation";
                else if (MediumOption.IsChecked == true) selectedPlan = "Premium Cremation";
            }

            if (string.IsNullOrEmpty(selectedPlan))
            {
                // Show warning dialog
                _ = new ContentDialog
                {
                    Title = "No Plan Selected",
                    Content = "Please select a specific plan.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();

                return;
            }

            // Basic field validation
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ContactNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(PlaceOfBirthTextBox.Text) ||
                string.IsNullOrWhiteSpace(AgeTextBox.Text) ||
                string.IsNullOrWhiteSpace(HeightTextBox.Text) ||
                string.IsNullOrWhiteSpace(WeightTextBox.Text) ||
                SexComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(NationalityTextBox.Text) ||
                CivilStatusComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(OccupationTextBox.Text) ||
                string.IsNullOrWhiteSpace(LotTextBox.Text) ||
                string.IsNullOrWhiteSpace(StreetTextBox.Text) ||
                string.IsNullOrWhiteSpace(BarangayTextBox.Text) ||
                string.IsNullOrWhiteSpace(CityTextBox.Text) ||
                string.IsNullOrWhiteSpace(ProvinceTextBox.Text) ||
                string.IsNullOrWhiteSpace(ZipCodeTextBox.Text) ||
                PlanTypeComboBox.SelectedItem == null ||
                string.IsNullOrEmpty(GetSelectedPlan()) ||
                ModeComboBox.SelectedItem == null ||
                PaymentMethodComboBox.SelectedItem == null ||
                InsuranceComboBox.SelectedItem == null)
            {
                await ShowDialog("Missing Information", "Please fill in all required fields.");
                return;
            }

            // Number and Date validations
            if (!int.TryParse(AgeTextBox.Text, out int age))
            {
                await ShowDialog("Invalid Input", "Age must be a valid number.");
                return;
            }

            if (!double.TryParse(HeightTextBox.Text, out double height))
            {
                await ShowDialog("Invalid Input", "Height must be a valid number.");
                return;
            }

            if (!double.TryParse(WeightTextBox.Text, out double weight))
            {
                await ShowDialog("Invalid Input", "Weight must be a valid number.");
                return;
            }

            if (DateOfBirthPicker.Date == null)
            {
                await ShowDialog("Invalid Date", "Please select a valid date of birth.");
                return;
            }

            if (!decimal.TryParse(ContractPriceTextBox.Text, out decimal contractPrice) ||
                !decimal.TryParse(TotalAmountPayableTextBox.Text, out decimal totalAmount) ||
                !decimal.TryParse(InstallmentDueTextBox.Text, out decimal installment))
            {
                await ShowDialog("Invalid Amounts", "Price or payment values are not valid numbers.");
                return;
            }

            var fullName = FullNameTextBox.Text.Split(',');
            string lastName = fullName.Length > 0 ? fullName[0].Trim() : "";
            string firstName = fullName.Length > 1 ? fullName[1].Trim() : "";
            string middleName = fullName.Length > 2 ? fullName[2].Trim() : "";

            var request = new UserApplyPlan
            {
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                ContactNumber = ContactNumberTextBox.Text,
                DateOfBirth = DateOfBirthPicker.Date.Date,
                PlaceOfBirth = PlaceOfBirthTextBox.Text,
                Age = age,
                Height = height.ToString(),
                Weight = weight.ToString(),
                Sex = ((ComboBoxItem)SexComboBox.SelectedItem).Content.ToString(),

                Nationality = NationalityTextBox.Text,
                CivilStatus = ((ComboBoxItem)CivilStatusComboBox.SelectedItem).Content.ToString(),
                Occupation = OccupationTextBox.Text,

                Lot = LotTextBox.Text,
                Street = StreetTextBox.Text,
                Barangay = BarangayTextBox.Text,
                City = CityTextBox.Text,
                Province = ProvinceTextBox.Text,
                ZipCode = ZipCodeTextBox.Text,

                PlanType = ((ComboBoxItem)PlanTypeComboBox.SelectedItem).Content.ToString(),
                SelectedPlan = GetSelectedPlan(),
                PlanName =selectedPlan,
                ModeOfPayment = ((ComboBoxItem)ModeComboBox.SelectedItem).Content.ToString(),
                PaymentMethod = ((ComboBoxItem)PaymentMethodComboBox.SelectedItem).Content.ToString(),
                Insurance = ((ComboBoxItem)InsuranceComboBox.SelectedItem).Content.ToString(),
                ContractPrice = contractPrice,
                TotalAmountPayable = totalAmount,
                InstallmentDue = installment,
                Balance = totalAmount,
            };

            request.CustomerId = AppState.LoggedInUser.CustomerId;
            request.PaymentStatus = "Unpaid";
            request.Status = "Pending"; // or whatever default you want
            request.DateApplied = DateTime.Now.Date;
            request.ClaimStatus = "Not Claimed";
            try
            {
                var service = new PlanSubmissionService();
                service.SubmitPlan(request);

                await ShowDialog("Success!", "Your plan has been submitted for review.");
                this.Frame.Navigate(typeof(HomePage));
            }
            catch (Exception ex)
            {
                await ShowDialog("Error", $"Failed to submit plan: {ex.Message}");
            }

        }

        // Helper method to show dialog
        private async Task ShowDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

    }
}
