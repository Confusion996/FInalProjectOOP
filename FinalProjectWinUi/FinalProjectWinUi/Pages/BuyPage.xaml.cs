using FinalProjectWinUi.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

namespace FinalProjectWinUi.Pages
{
    public sealed partial class BuyPage : Page
    {
        public BuyPage()
        {
            this.InitializeComponent();
        }

        private void BuyNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string selectedTag = selectedItem.Tag?.ToString() ?? string.Empty;
                NavigateToPage(selectedTag);
            }
        }

        private void Card_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Border border && border.Tag is string selectedTag)
            {
                NavigateToPage(selectedTag);
            }
        }

        private void NavigateToPage(string tag)
        {
            Type pageType = tag switch
            {
                "ApplyPlan" => typeof(ApplyPlanForm),
                "PayPlan" => typeof(PlanPayment),
                "ClaimPlan" => typeof(ClaimPlan),
                "ViewPlan" => typeof(ViewPlan),
                "TransactionHistory" => typeof(TransactionHistory),
                "UpdateInfo" => typeof(UpdateInfoPage),
                "ViewStatus" => typeof(ViewApplicationStatus),
                _ => null
            };

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }
}
