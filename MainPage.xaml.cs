using System.Globalization;

namespace TipMate
{
    public partial class MainPage : ContentPage
    {
        private readonly CultureInfo _culture = CultureInfo.CurrentCulture;

        private bool _isMultiplePeople;
        public bool IsMultiplePeople
        {
            get => _isMultiplePeople;
            set
            {
                if (_isMultiplePeople != value)
                {
                    _isMultiplePeople = value;
                    OnPropertyChanged();  // Notify the UI to update
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            TipEntry.Text = "15";
            SplitEntry.Text = "1";
            TipSlider.Value = 15;
            SplitSlider.Value = 1;
            BillEntry.TextChanged += (_, _) => UpdateResult();
            TotalRoundSwitch.Toggled += (_, _) => UpdateResult();
            IsMultiplePeople = false;
            UpdateResult();
        }

        private void OnTipSliderChanged(object sender, ValueChangedEventArgs e)
        {
            TipEntry.Text = Math.Round(e.NewValue).ToString(_culture);
            UpdateResult();
        }

        private void OnTipEntryChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int tip))
            {
                TipSlider.Value = tip;
                UpdateResult();
            }
        }

        private void OnSplitSliderChanged(object sender, ValueChangedEventArgs e)
        {
            SplitEntry.Text = Math.Round(e.NewValue).ToString(_culture);

            UpdateResult();
        }

        private void OnSplitEntryChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int split))
            {
                SplitSlider.Value = split;

                // Update the IsMultiplePeople property based on the split value
                IsMultiplePeople = split > 2;

                UpdateResult();
            }
        }

        private void OnRoundTipToggled(object sender, ToggledEventArgs e)
        {
            HapticsHelper.Vibrate(75);
            UpdateResult();
        }

        private void OnRoundTotalToggled(object sender, ToggledEventArgs e)
        {
            HapticsHelper.Vibrate(75);
            UpdateResult();
        }

        private void OnEntryUnfocused(object sender, FocusEventArgs e)
        {
            HapticsHelper.Vibrate(20);
        }

        private void UpdateResult()
        {
            // Parse bill input
            if (!double.TryParse(BillEntry.Text, NumberStyles.Any, _culture, out double bill))
            {
                return;
            }

            // Parse tip percent
            int tipPercent = int.TryParse(TipEntry.Text, out int parsedTip) ? parsedTip : 0;

            // Parse split count with fallback to 1
            int split = int.TryParse(SplitEntry.Text, out int parsedSplit) ? Math.Max(1, parsedSplit) : 1;

            // Calcuate tip and totals
            double tipAmount = bill * tipPercent / 100;
            if (TipRoundSwitch.IsToggled)
            {
                tipAmount = Math.Ceiling(tipAmount);
            }
            double total = bill + tipAmount;
            double roundedTotal = TotalRoundSwitch.IsToggled ? Math.Ceiling(total) : total;
            double perPerson = roundedTotal / split;

            // Update label with formatted currency
            TipAmountLabel.Text = string.Format(_culture, "{0:C}", tipAmount);
            TotalAmountLabel.Text = string.Format(_culture, "{0:C}", roundedTotal);
            ResultLabel.Text = string.Format(_culture, "{0:C}", perPerson);
        }

        private async void OnShareClicked(object sender, EventArgs e)
        {
            if (!double.TryParse(BillEntry.Text, NumberStyles.Any, _culture, out double bill))
            {
                return;
            }

            HapticsHelper.Vibrate(200);

            int tipPercent = int.TryParse(TipEntry.Text, out int parsedTip) ? parsedTip : 0;
            int split = int.TryParse(SplitEntry.Text, out int parsedSplit) ? Math.Max(1, parsedSplit) : 1;

            double tipAmount = bill * tipPercent / 100;
            if (TipRoundSwitch.IsToggled)
            {
                tipAmount = Math.Ceiling(tipAmount);
            }
            double total = bill + tipAmount;
            double roundedTotal = TotalRoundSwitch.IsToggled ? Math.Ceiling(total) : total;
            double perPerson = roundedTotal / split;

            string message = $"Bill: {bill.ToString("C", _culture)}\n" +
                     $"Tip: {tipAmount.ToString("C", _culture)}%\n" +
                     $"Split: {split} people\n" +
                     $"Total: {roundedTotal.ToString("C", _culture)}\n" +
                     $"Each pays: {perPerson.ToString("C", _culture)}";
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = message,
                Title = "TipMate Summary"
            });
        }
    }
}