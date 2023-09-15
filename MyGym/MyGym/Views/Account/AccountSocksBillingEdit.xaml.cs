using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Barcode;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountSocksBillingEdit : ContentPage
    {
        public AccountSocksBillingEdit()
        {
            InitializeComponent();
        }

        private async void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            List<CustomListItemMobile> l = new List<CustomListItemMobile>();
            l.Add(new CustomListItemMobile { Text = "01", Value = "01" });
            l.Add(new CustomListItemMobile { Text = "02", Value = "02" });
            l.Add(new CustomListItemMobile { Text = "03", Value = "03" });
            l.Add(new CustomListItemMobile { Text = "04", Value = "04" });
            l.Add(new CustomListItemMobile { Text = "05", Value = "05" });
            l.Add(new CustomListItemMobile { Text = "06", Value = "06" });
            l.Add(new CustomListItemMobile { Text = "07", Value = "07" });
            l.Add(new CustomListItemMobile { Text = "08", Value = "08" });
            l.Add(new CustomListItemMobile { Text = "09", Value = "09" });
            l.Add(new CustomListItemMobile { Text = "10", Value = "10" });
            l.Add(new CustomListItemMobile { Text = "11", Value = "11" });
            l.Add(new CustomListItemMobile { Text = "12", Value = "12" });
            CCExpMonth.ItemsSource = l;

            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.Country == "Canada")
            {
                State.Placeholder = "-Province-";
                Zip.WatermarkText = "-Postal-";
            }
            else
            {
                State.Placeholder = "-State-";
                Zip.WatermarkText = "-Zip-";
            }
            List<CustomListItemMobile> mb = new List<CustomListItemMobile>();
            for (int x = DateTime.Now.Year; x != DateTime.Now.Year + 12; x++)
            {
                mb.Add(new CustomListItemMobile { Text = x.ToString(), Value = x.ToString() });
            }
            CCExpYear.ItemsSource = mb;
            State.ItemsSource = UtilMobile.GetStates(gym.Country == "Canada", true);
            int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", ""));
            if (billingId >= -1)
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                AccountBillingMobile bb = null;
                foreach (AccountBillingMobile b in account.Billing)
                {
                    if (b.Id == billingId)
                    {
                        bb = b;
                        break;
                    }
                }
                if (bb == null)
                {
                    bb = new AccountBillingMobile();
                    UtilMobile.CopyPropertyValues(account, bb);
                }
                CardName.Text = $"{bb.BillingName}";
                Email.Text = string.IsNullOrEmpty(bb.BillingEmail) ? account.Email : bb.BillingEmail;
                Address.Text = string.IsNullOrEmpty(bb.BillingAddress) ? account.Address : bb.BillingAddress;
                City.Text = string.IsNullOrEmpty(bb.BillingCity) ? account.City : bb.BillingCity;
                string state = string.IsNullOrEmpty(bb.BillingState) ? account.State : bb.BillingState;
                State.SelectedItem = ((List<CustomListItemMobile>)State.ItemsSource).Find(m => m.Value == state);
                Zip.Text = string.IsNullOrEmpty(bb.BillingState) ? account.Zip : bb.BillingZip;
                string month = bb.CCExpMonth.ToString();
                if (bb.CCExpMonth < 10)
                {
                    month = "0" + month;
                }
                CCExpMonth.SelectedItem = ((List<CustomListItemMobile>)CCExpMonth.ItemsSource).Find(m => m.Value == month);
                CCExpYear.SelectedItem = ((List<CustomListItemMobile>)CCExpYear.ItemsSource).Find(m => m.Value == bb.CCExpYear.ToString());
            }
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            string cardName = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(CardName.Text));
            string email = Email.Text == null ? "" : Email.Text.ToLower();
            string address = Address.Text == null ? "" : UtilMobile.ProperCase(Address.Text);
            string city = City.Text == null ? "" : City.Text;
            string state = State.SelectedItem != null ? ((CustomListItemMobile)State.SelectedItem).Value : "";
            string zip = Zip.Text == null ? "" : Zip.Text;
            string ccNum = CCNum.Text == null ? "" : CCNum.Text;
            string ccExpMonth = CCExpMonth.SelectedItem != null ? ((CustomListItemMobile)CCExpMonth.SelectedItem).Value : "";
            string ccExpYear = CCExpYear.SelectedItem != null ? ((CustomListItemMobile)CCExpYear.SelectedItem).Value : "";
            string ccvn = CCVN.Text == null ? "" : CCVN.Text;
            if (cardName == "" || address == "" || city == "" || state == "" || zip == "" || ccNum == "" || ccExpMonth == "" || ccExpYear == "" || ccvn == "")
            {
                Xamarin.Essentials.Preferences.Set("result", "inputmissing");
            }
            else if (UtilMobile.ValidEmail(Email.Text) == false)
            {
                Xamarin.Essentials.Preferences.Set("result", "invalidemail");
            }
            else if (ccNum != "4444333322221111" && ccNum != "1111222233334444" && UtilMobile.ValidCC(ccNum, ccExpMonth + "/" + ccExpYear, ccvn) == false)
            {
                Xamarin.Essentials.Preferences.Set("result", "invalidcc");
            }
            else if (UtilMobile.ContainsAlpha(ccExpMonth) == true || UtilMobile.ContainsAlpha(ccExpYear) == true)
            {
                Xamarin.Essentials.Preferences.Set("result", "invalidccexp");
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("result", "");
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                Dictionary<string, object> ps = new Dictionary<string, object>();
                int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", ""));
                ps.Add("gymId", gym.Id);
                ps.Add("accountId", account.AccountId);
                ps.Add("billingId", billingId);
                ps.Add("cardName", cardName);
                ps.Add("email", email);
                ps.Add("address", address);
                ps.Add("apt", Apt.Text);
                ps.Add("city", city);
                ps.Add("state", state);
                ps.Add("zip", zip);
                ps.Add("ccNum", ccNum);
                ps.Add("ccExpMonth", ccExpMonth);
                ps.Add("ccExpYear", ccExpYear);
                ps.Add("ccvn", ccvn);
                AccountMobile a = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/updatebilling", ps);
                Application.Current.Properties["account"] = a;
            }
        }

        async private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }

            string result = Xamarin.Essentials.Preferences.Get("result", "");
            if (result == "")
            {
                await Shell.Current.Navigation.PopAsync();
            }
            else if (result == "inputmissing")
            {
                await DisplayAlert("Input Missing", "Please enter all values", "Close");
            }
            else if (result == "invalidemail")
            {
                await DisplayAlert("Invalid Email", "Please enter a valid email address", "Close");
            }
            else if (result == "invalidcc")
            {
                await DisplayAlert("Invalid Credit Card", "Please enter a valid credit card number", "Close");
            }
            else if (result == "invalidccexp")
            {
                await DisplayAlert("Invalid Credit Card Expiration", "Please enter a valid credit card expiration", "Close");
            }
            activityIndicator.IsVisible = false;
            submitButton.IsVisible = true;
            cancelButton.IsVisible = true;
        }

        private async void BackToBilling_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        private async void CancelButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        void SubmitButton_Clicked(System.Object sender, System.EventArgs e)
        {
            activityIndicator.IsVisible = true;
            submitButton.IsVisible = false;
            cancelButton.IsVisible = false;

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }
    }
}