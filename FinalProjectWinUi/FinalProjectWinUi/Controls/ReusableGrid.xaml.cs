using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;

namespace FinalProjectWinUi.Controls
{
    public sealed partial class ReusableGrid : UserControl
    {
        public ReusableGrid()
        {
            this.InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ReusableGrid), new PropertyMetadata(""));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ReusableGrid), new PropertyMetadata(""));

        public double Price
        {
            get => (double)GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }

        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(double), typeof(ReusableGrid), new PropertyMetadata(0.0));

        public string FormattedPrice => $"₱{Price:F2}";

        public Uri IconUri
        {
            get => (Uri)GetValue(IconUriProperty);
            set => SetValue(IconUriProperty, value);
        }

        public static readonly DependencyProperty IconUriProperty =
            DependencyProperty.Register("IconUri", typeof(Uri), typeof(ReusableGrid), new PropertyMetadata(null));
    }
}
