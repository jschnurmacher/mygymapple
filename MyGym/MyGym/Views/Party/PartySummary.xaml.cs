using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartySummary : ContentPage
    {
        public PartySummary()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            List<ChildMobile> selectedChildren = (List<ChildMobile>)Application.Current.Properties["selectedchildren"];
            if (selectedChildren.Count == 0)
            {
                Shell.Current.Navigation.PopToRootAsync();
                return;
            }
            base.OnAppearing();
            Application.Current.Properties["creditapply"] = 0;
            Application.Current.Properties["creditavailable"] = account.StoreCredit;
            Application.Current.Properties["deposit"] = 0;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int partyPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", ""));
            PartyMobile p = (PartyMobile)Application.Current.Properties["party"];
            foreach (PartyPackageMobile pk in p.PartyPackages)
            {
                if (pk.Id == partyPackageId)
                {
                    PartyPackage.Text = $"Package: {pk.Name}";
                    break;
                }
            }
            PartyTimeMobile pt = (PartyTimeMobile)Application.Current.Properties["partyselectedtime"];
            string partyHalfHour = Xamarin.Essentials.Preferences.Get("partyhalfhour", "");
            if (partyHalfHour == "1")
            {
                pt.End = pt.End.AddMinutes(30);
            }
            pt.Time = PartyTime.Text = string.Format(new CultureInfo(gym.Culture), "Date: {0:d} {0:h:mmtt}-{1:h:mmtt}", pt.Start, pt.End);
            PartyDeposit.Text = string.Format(new CultureInfo(gym.Culture), "Deposit: {0:c}", gym.BirthdayDeposit);
            PartyChild.Text = "Children:\r\n";
            foreach (ChildMobile c in selectedChildren)
            {
                PartyChild.Text += c.First + "\r\n";
            }
            List<int> addOns = (List<int>)Application.Current.Properties["selectedaddons"];
            if (addOns.Contains(0))
            {
                PartyUpgrades.IsVisible = false;
            }
            else
            {
                PartyOptionsMobile s = (PartyOptionsMobile)Application.Current.Properties["partyaddons"];
                PartyUpgrades.Text = "Party Upgrades:\r\n";
                foreach (PartyOptionMobile po in s.PartyOptions)
                {
                    if (addOns.Contains(po.Id))
                    {
                        PartyUpgrades.Text += po.Name + "\r\n";
                    }
                }
            }
            CreditContainer.IsVisible = false;
            CreditAvailable.IsVisible = false;
            CreditApplied.IsVisible = false;
            if (gym.AllowStoreCreditOnWebsite == true && account.StoreCredit > 0)
            {
                CreditContainer.IsVisible = true;
                CreditAvailable.IsVisible = true;
                CreditApplied.IsVisible = true;
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", account.StoreCredit);
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
            }
        }

        private void Promo_Clicked(object sender, EventArgs e)
        {
            promoButton.IsVisible = false;
            activityIndicator.IsVisible = true;
            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionPromo;
            b.RunWorkerCompleted += RunWorkerCompletedPromo;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionPromo(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("promoCode", PromoCode.Text);
            Application.Current.Properties["promoresult"] = UtilMobile.CallApiGetParamsString($"/api/gym/promoparty", ps);
        }

        async private void RunWorkerCompletedPromo(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            string s = (string)Application.Current.Properties["promoresult"];
            string[] a = s.Split('|');
            string summary = a[1];
            PromoCodeResult.IsVisible = true;
            PromoCodeResult.Text = summary;
            if (summary.StartsWith("Code not accepted") == false)
            {
                Application.Current.Properties["promocode"] = PromoCode.Text;
                PromoCodeResult.TextColor = Color.DarkGreen;
            }
            else
            {
                Application.Current.Properties["promocode"] = "";
                PromoCodeResult.TextColor = Color.IndianRed;
                PromoCodeContainer.IsVisible = false;
            }
            promoButton.IsVisible = true;
            activityIndicator.IsVisible = false;
        }

        private void Gift_Clicked(object sender, EventArgs e)
        {
            giftButton.IsVisible = false;
            activityIndicatorGift.IsVisible = true;
            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionGift;
            b.RunWorkerCompleted += RunWorkerCompletedGift;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionGift(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            ps.Add("giftCode", GiftCode.Text);
            Application.Current.Properties["giftresult"] = UtilMobile.CallApiGetParamsString($"/api/gym/giftcert", ps);
        }

        async private void RunWorkerCompletedGift(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            string s = (string)Application.Current.Properties["giftresult"];
            string[] a = s.Split('|');
            string summary = a[0];
            string credit = a[1];
            GiftCodeResult.IsVisible = true;
            GiftCodeResult.Text = summary;
            if (credit != "-1000")
            {
                Xamarin.Essentials.Preferences.Set("giftcode", GiftCode.Text);
                GiftCodeResult.TextColor = Color.DarkGreen;
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                CreditContainer.IsVisible = true;
                CreditAvailable.IsVisible = true;
                CreditApplied.IsVisible = true;
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", Convert.ToDecimal(credit));
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
                GiftCodeContainer.IsVisible = false;
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                account.StoreCredit = Convert.ToDecimal(credit);
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("giftcode", "");
                GiftCodeResult.TextColor = Color.IndianRed;
            }
            giftButton.IsVisible = true;
            activityIndicatorGift.IsVisible = false;
        }

        private void Credit_Clicked(object sender, EventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            Application.Current.Properties["creditapply"] = 0;
            Application.Current.Properties["creditavailable"] = 0;
            Application.Current.Properties["deposit"] = 0;
            CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", Convert.ToDecimal(account.StoreCredit));
            CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
            PartyDeposit.Text = string.Format(new CultureInfo(gym.Culture), "Deposit: {0:c}", gym.BirthdayDeposit);

            string creditApply = CreditApply.Text.Replace("-", "");
            decimal creditApplyAmount = 0;
            if (decimal.TryParse(creditApply, out creditApplyAmount) == true && creditApplyAmount > 0)
            {
                activityIndicatorCredit.IsVisible = true;
                BackgroundWorker b;
                b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionCredit;
                b.RunWorkerCompleted += RunWorkerCompletedCredit;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
            else
            {
                DisplayAlert("Credit Amount Incorrect", "Please enter a numeric value for credit amount.", "Close");
            }
        }

        private void RunActionCredit(object sender, DoWorkEventArgs e)
        {
            string ca = CreditAvailable.Text.Replace("Credit Available: ", "").Substring(1);
            string cp = CreditApply.Text.Replace("-", "");
            string dp = PartyDeposit.Text.Replace("Deposit: ", "").Substring(1);
            decimal creditAvailableAmount = Convert.ToDecimal(ca);
            decimal creditApplyAmount = Convert.ToDecimal(cp);
            decimal deposit = Convert.ToDecimal(dp);
            if (creditApplyAmount > creditAvailableAmount)
            {
                creditApplyAmount = creditAvailableAmount;
            }
            if (creditApplyAmount > deposit)
            {
                creditApplyAmount = deposit;
            }
            creditAvailableAmount = creditAvailableAmount - creditApplyAmount;
            Application.Current.Properties["creditapply"] = creditApplyAmount;
            Application.Current.Properties["creditavailable"] = creditAvailableAmount;
            Application.Current.Properties["deposit"] = deposit;
        }

        async private void RunWorkerCompletedCredit(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            decimal creditAvailableAmount = (decimal)Application.Current.Properties["creditavailable"];
            decimal creditApplyAmount = (decimal)Application.Current.Properties["creditapply"];
            decimal deposit = (decimal)Application.Current.Properties["deposit"];
            CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", creditAvailableAmount);
            CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", creditApplyAmount);
            CreditApplied.TextColor = Color.DarkGreen;
            PartyDeposit.Text = string.Format(new CultureInfo(gym.Culture), "Deposit: {0:c}", deposit - creditApplyAmount);
            activityIndicatorCredit.IsVisible = false;
            CreditApply.Text = "";
            await DisplayAlert("Credit was Applied to Payment", string.Format(new CultureInfo(gym.Culture), "{0:c} was applied to your payment.", creditApplyAmount), "Close");
        }

        async void continueButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "bookparty");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}
