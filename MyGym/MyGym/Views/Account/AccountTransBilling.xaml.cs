using System;
using System.Collections.Generic;
using System.ComponentModel;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountTransBilling : ContentPage
    {
        public AccountTransBilling()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int x = 0;
            foreach (AccountBillingMobile b in account.Billing)
            {
                b.BillingName = $"{b.First} {b.Last}";
                b.BillingFullAddress = b.BillingAddress;
                if (b.BillingApt != "")
                {
                    b.BillingFullAddress += " " + b.BillingApt;
                }
                b.BillingCityStateZip = $"{b.BillingCity}, {b.BillingState} {b.BillingZip}";
                if (x == 0)
                {
                    b.Delete = false;
                    b.MakePrimary = false;
                }
                else
                {
                    b.Delete = true;
                    b.MakePrimary = true;
                }
                x++;
            }
            listView.ItemsSource = account.Billing;
            listView.HeightRequest = account.Billing.Count * 100;
            base.OnAppearing();
        }

        async void EditBilling_Tapped(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("billingid", ((TappedEventArgs)e).Parameter.ToString());
            await Shell.Current.Navigation.PushAsync(new AccountBillingEdit());
        }

        async void DeleteBilling_Tapped(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("billingid", ((TappedEventArgs)e).Parameter.ToString());
            var answer = await DisplayAlert("Delete Billing", "Are you sure?", "Yes", "No");
            if (answer)
            {
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionDelete;
                b.RunWorkerCompleted += RunWorkerCompletedDelete;
                b.RunWorkerAsync("");
            }
        }

        private void RunActionDelete(object sender, DoWorkEventArgs e)
        {
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "0"));
            int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", "-1"));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps = new Dictionary<string, object>();
            ps.Add("accountId", Convert.ToInt32(accountId));
            ps.Add("billingId", Convert.ToInt32(billingId));
            Application.Current.Properties["account"] = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/deletebilling", ps);
        }

        async private void RunWorkerCompletedDelete(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            await Shell.Current.Navigation.PopAsync();
            await Shell.Current.Navigation.PushAsync(new AccountTransBilling());
        }

        async void MakePrimaryBilling_Tapped(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("billingid", ((TappedEventArgs)e).Parameter.ToString());
            Xamarin.Essentials.Preferences.Set("billingid", ((TappedEventArgs)e).Parameter.ToString());
            var answer = await DisplayAlert("Convert To Primary", "Convert card to primary. Card will be used for any future recurring payments if you are enrolled. Are you sure?", "Yes", "No");
            if (answer)
            {
                BackgroundWorker b1 = new BackgroundWorker();
                b1.WorkerReportsProgress = true;
                b1.WorkerSupportsCancellation = true;
                b1.DoWork += RunActionMakePrimary;
                b1.RunWorkerCompleted += RunWorkerCompletedMakePrimary;
                b1.RunWorkerAsync("");
            }
        }

        async private void RunActionMakePrimary(object sender, DoWorkEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "0"));
            int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", "-1"));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps = new Dictionary<string, object>();
            ps.Add("accountId", Convert.ToInt32(accountId));
            ps.Add("billingId", Convert.ToInt32(billingId));
            Application.Current.Properties["account"] = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/convertbilling", ps);
        }

        async private void RunWorkerCompletedMakePrimary(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            await Shell.Current.Navigation.PopAsync();
            await Shell.Current.Navigation.PushAsync(new AccountTransBilling());
        }

        async void AddBilling_Tapped(System.Object sender, System.EventArgs e)
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
    }
}
