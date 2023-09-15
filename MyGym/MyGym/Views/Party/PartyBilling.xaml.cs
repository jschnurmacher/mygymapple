using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyBilling : ContentPage
    {
        public PartyBilling()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            int addBilling = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("addbilling", "0"));
            if (addBilling == 1)
            {
                Xamarin.Essentials.Preferences.Set("addbilling", "0");
                await Shell.Current.Navigation.PopAsync();
                return;
            }
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
            listView.HeightRequest = account.Billing.Count * 120;
            base.OnAppearing(); base.OnAppearing();
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
            Xamarin.Essentials.Preferences.Set("addbilling", "1");
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
            int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", "-2"));
            if (billingId < -1)
            {
                await DisplayAlert("Billing Not Selected", "Please add billing information to continue", "Close");
            }
            else
            {
                var o = ListView.SelectedItemProperty;
                if (o == null)
                {
                    await DisplayAlert("Billing Not Selected", "Please add or select billing information to continue", "Close");
                }
                else
                {
                    await Shell.Current.Navigation.PushAsync(new PartySummary());

                }
            }
        }
    }
}