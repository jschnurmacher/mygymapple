using mygymmobiledata;
using System;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassCardBilling : ContentPage
    {
        public ClassCardBilling()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (AccountBillingMobile b in account.Billing)
            {
                b.BillingName = $"{b.First} {b.Last}";
                b.BillingFullAddress = b.BillingAddress;
                if (b.BillingApt != "")
                {
                    b.BillingFullAddress += " " + b.BillingApt;
                }
                b.BillingCityStateZip = $"{b.BillingCity}, {b.BillingState} {b.BillingZip}";
            }
            if (account.Billing.Count > 0)
            {
                account.Billing[0].Checked = true;
                Xamarin.Essentials.Preferences.Set("billingid", "-1");
            }
            listView.ItemsSource = account.Billing;
            listView.HeightRequest = account.Billing.Count * 100;
            base.OnAppearing();
        }

        async private void AddBilling_Tapped(object sender, EventArgs e)
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            if (account.Billing.Count == 0)
            {
                Xamarin.Essentials.Preferences.Set("billingid", "-1");
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("billingid", "0");
            }
            await Shell.Current.Navigation.PushAsync(new AccountBillingEdit());
        }

        private RadCheckBox checkboxLast = null;
        private void billingCheckbox_IsCheckedChanged(object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            RadCheckBox checkbox = (RadCheckBox)sender;
            AccountBillingMobile billing = (AccountBillingMobile)checkbox.BindingContext;
            if (checkbox.IsChecked == true)
            {
                Xamarin.Essentials.Preferences.Set("billingid", billing.Id.ToString());
                if (checkboxLast != null && checkboxLast != checkbox)
                {
                    checkboxLast.IsChecked = false;
                }
                checkboxLast = checkbox;
            }
        }

        private void Billing_Tapped(object sender, EventArgs e)
        {
            Grid o = (Grid)sender;
            foreach (Element t in o.Children)
            {
                if (t.GetType().ToString().Contains("CheckBox"))
                {
                    ((RadCheckBox)t).IsChecked = true;
                    break;
                }
            }
        }

        async private void Continue_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new ClassCardSummary());
        }
    }
}