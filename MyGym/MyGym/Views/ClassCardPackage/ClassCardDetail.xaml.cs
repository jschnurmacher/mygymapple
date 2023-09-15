using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassCardDetail : ContentPage
    {
        public ClassCardDetail()
        {
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            Application.Current.Properties["creditapply"] = 0;
            Application.Current.Properties["creditavailable"] = 0;
            int classCardPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classcardpackageid", ""));
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            ClassCardMobile p = (ClassCardMobile)Application.Current.Properties["classcardpackages"];
            foreach (ClassCardPackageMobile pk in p.ClassCardPackages)
            {
                if (pk.Id == classCardPackageId)
                {
                    ClassCardName.Text = pk.Name;
                    Ages.Text = pk.Ages;
                    CreditsStr.Text = $"{pk.Credits} Classes";
                    ValidDaysStr.Text = $"Card is Valid for {pk.ValidDays} Days";
                    break;
                }
            }

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionLiability;
            b.RunWorkerCompleted += RunWorkerCompletedLiability;
            b.RunWorkerAsync(new DoWorkEventArgs(null));

            BackgroundWorker b1 = new BackgroundWorker();
            b1.WorkerReportsProgress = true;
            b1.WorkerSupportsCancellation = true;
            b1.DoWork += RunActionCalc;
            b1.RunWorkerCompleted += RunWorkerCompletedCalc;
            b1.RunWorkerAsync(new DoWorkEventArgs(null));

            base.OnAppearing();
        }

        private void RunActionLiability(object sender, DoWorkEventArgs e)
        {
            EnrollTitle.IsVisible = false;
            scrollView.IsVisible = false;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdLiability", gym.Id);
            string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/liability", ps));
            Xamarin.Essentials.Preferences.Set("liability", l.Replace("\r\n", "\r\n\r\n"));
        }

        async private void RunWorkerCompletedLiability(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            ReleaseOfLiability.Text = Xamarin.Essentials.Preferences.Get("liability", "");
            EnrollTitle.IsVisible = true;
            scrollView.IsVisible = true;
        }

        private void RunActionCalc(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
            promoCode = promoCode == null ? "" : promoCode;
            decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            ps.Add("gymId", Convert.ToInt32(gym.Id));
            ps.Add("accountId", Convert.ToInt32(account.AccountId));
            ps.Add("cardPackageId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classcardpackageid", "")));
            ps.Add("promoCode", promoCode);
            ps.Add("storeCredit", creditApply);
            string l = UtilMobile.CallApiGetParamsString("/api/gym/cardpackagecalc", ps);
            string[] a = l.Split('|');
            decimal cardcost = Convert.ToDecimal(a[0]);
            decimal subtotal = Convert.ToDecimal(a[1]);
            decimal tax = Convert.ToDecimal(a[2]);
            decimal total = Convert.ToDecimal(a[3]);
            decimal cardmember = Convert.ToDecimal(a[4]);
            decimal cardmembertax = Convert.ToDecimal(a[5]);
            decimal discountamount = Convert.ToDecimal(a[6]);
            string discount = Convert.ToString(a[7]);
            Application.Current.Properties["cardcost"] = cardcost;
            Application.Current.Properties["cardsubtotal"] = subtotal;
            Application.Current.Properties["cardtotal"] = total;
            Application.Current.Properties["cardtax"] = tax;
            Application.Current.Properties["cardmember"] = cardmember;
            Application.Current.Properties["cardmembertax"] = cardmembertax;
            Application.Current.Properties["carddiscountamount"] = discountamount;
            Application.Current.Properties["carddiscount"] = discount;
        }

        async private void RunWorkerCompletedCalc(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            decimal cardcost = Convert.ToDecimal(Application.Current.Properties["cardcost"]);
            decimal subtotal = Convert.ToDecimal(Application.Current.Properties["cardsubtotal"]);
            decimal total = Convert.ToDecimal(Application.Current.Properties["cardtotal"]);
            decimal tax = Convert.ToDecimal(Application.Current.Properties["cardtax"]);
            decimal cardmember = Convert.ToDecimal(Application.Current.Properties["cardmember"]);
            decimal cardmembertax = Convert.ToDecimal(Application.Current.Properties["cardmembertax"]);
            decimal discountamount = Convert.ToDecimal(Application.Current.Properties["carddiscountamount"]);
            string discount = Convert.ToString(Application.Current.Properties["carddiscount"]);
            TotalStr.Text = "";
            TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Cost: {0:c}\r\n", cardcost);
            if (cardmember > 0)
            {
                TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Membership: {0:c}\r\n", cardmember);
            }
            if (discountamount > 0)
            {
                TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Promo Discount: {0:c}\r\n", -discountamount);
            }
            TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Subtotal: {0:c}\r\n", subtotal);
            if (tax > 0)
            {
                TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Tax: {0:c}\r\n", tax);
            }
            if (cardmembertax > 0)
            {
                TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Membership Tax: {0:c}\r\n", cardmember);
            }
            TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Total: {0:c}\r\n", total);
            if (string.IsNullOrEmpty(discount) == false)
            {
                TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "{0}\r\n", discount);
            }
            activityIndicator.IsVisible = false;
            continueButton.IsVisible = true;
            TotalStr.IsVisible = true;
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;
            if (liabilityCheckbox.IsChecked == false)
            {
                InputMissing.IsVisible = true;
                await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new ClassCardBilling());
            }
        }

        private void AgreeButton_Clicked(object sender, EventArgs e)
        {
            liabilityCheckbox.IsChecked = true;
        }
    }
}