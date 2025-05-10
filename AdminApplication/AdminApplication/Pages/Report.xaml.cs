using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using AdminApplication.Services;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.WinUI;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;

namespace AdminApplication.Pages
{
    public sealed partial class Report : Page
    {
        public Report()
        {
            this.InitializeComponent();
        }

        private async void OnGetReportClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get selected date range from ComboBox
                string selectedRange = ((ComboBoxItem)DateRangeComboBox.SelectedItem)?.Content.ToString();
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Today;

                switch (selectedRange)
                {
                    case "Weekly":
                        startDate = DateTime.Today.AddDays(-7);
                        break;
                    case "Monthly":
                        startDate = DateTime.Today.AddMonths(-1);
                        break;
                    case "Yearly":
                        startDate = DateTime.Today.AddYears(-1);
                        break;
                    default:
                        startDate = DateTime.Today.AddDays(-7);
                        break;
                }

                // Get summary report data
                double totalSales = await PlanDataService.GetTotalSalesAsync(startDate, endDate);
                double totalPayments = await PlanDataService.GetTotalPaymentsAsync(startDate, endDate);
                double outstandingBalance = await PlanDataService.GetOutstandingBalancesAsync();

                TotalSalesText.Text = $"{totalSales:N0}";
                TotalPaymentsText.Text = $"₱{totalPayments:N2}";
                OutstandingBalancesText.Text = $"₱{outstandingBalance:N2}";

                // 📊 Sales by PlanName using Dictionary<string, double>
                Dictionary<string, double> planSales = await PlanDataService.GetSalesByPlanNamesAsync(startDate, endDate);

                PlanSalesChart.Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = planSales.Values.ToList(),
                        Name = "Plan Sales",
                        TooltipLabelFormatter = point =>
                            $"{planSales.Keys.ElementAt(point.Index)}: ₱{point.PrimaryValue:N2}"
                    }
                };

                PlanSalesChart.XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = planSales.Keys.ToArray(),
                        Name = "Plan",
                        LabelsPaint = new SolidColorPaint(SkiaSharp.SKColors.White)
                    }
                };

                PlanSalesChart.YAxes = new Axis[]
                {
                    new Axis
                    {
                        Name = "₱ Sales",
                        Labeler = value => $"₱{value:N0}",
                        LabelsPaint = new SolidColorPaint(SkiaSharp.SKColors.White)
                    }
                };
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Error Loading Report",
                    Content = $"An error occurred: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
    }
}
