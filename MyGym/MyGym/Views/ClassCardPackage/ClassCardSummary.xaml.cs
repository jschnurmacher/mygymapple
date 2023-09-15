using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassCardSummary : ContentPage
    {
        public ClassCardSummary()
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
            Application.Current.Properties["creditapply"] = 0;
            CreditContainer.IsVisible = false;
            CreditAvailable.IsVisible = false;
            CreditApplied.IsVisible = false;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            if (gym.AllowStoreCreditOnWebsite == true && account.StoreCredit > 0)
            {
                CreditContainer.IsVisible = true;
                CreditAvailable.IsVisible = true;
                CreditApplied.IsVisible = true;
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", account.StoreCredit);
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
            }
            CalculateTotals();
            /*
            decimal discountamount = Convert.ToDecimal(Application.Current.Properties["carddiscountamount"]);
            if (discountamount > 0)
            {
                PromoCodeContainer.IsVisible = false;
            }*/
            base.OnAppearing();
        }

        private void Promo_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["promocode"] = PromoCode.Text;
            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionPromo;
            b.RunWorkerCompleted += RunWorkerCompletedPromo;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            activityIndicator.IsVisible = true;
        }

        private void RunActionPromo(object sender, DoWorkEventArgs e)
        {
            CalculateCardPackageCalc();
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
            CalculateTotals();
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            decimal creditAvailable = account.StoreCredit - creditApply;
            CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", creditAvailable);
            CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", creditApply);
            CreditApplied.TextColor = Color.DarkGreen;
            CreditApply.Text = "";
            TotalStr.IsVisible = true;
            activityIndicator.IsVisible = false;
            PromoCode.Text = "";
            string discount = Convert.ToString(Application.Current.Properties["carddiscount"]);
            if (discount.ToLower().Contains("is not valid") == false)
            {
                PromoCodeContainer.IsVisible = false;
            }
            await DisplayAlert("Promo was Applied", string.Format(new CultureInfo(gym.Culture), "{0}", discount), "Close");
        }

        private void CalculateCardPackageCalc()
        {
            decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
            promoCode = promoCode == null ? "" : promoCode;
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

        private void CalculateTotals()
        {
            int classCardPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classcardpackageid", ""));
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            ClassCardMobile p = (ClassCardMobile)Application.Current.Properties["classcardpackages"];
            decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            foreach (ClassCardPackageMobile pk in p.ClassCardPackages)
            {
                if (pk.Id == classCardPackageId)
                {
                    decimal cardcost = Convert.ToDecimal(Application.Current.Properties["cardcost"]);
                    decimal subtotal = Convert.ToDecimal(Application.Current.Properties["cardsubtotal"]);
                    decimal total = Convert.ToDecimal(Application.Current.Properties["cardtotal"]);
                    decimal tax = Convert.ToDecimal(Application.Current.Properties["cardtax"]);
                    decimal cardmember = Convert.ToDecimal(Application.Current.Properties["cardmember"]);
                    decimal cardmembertax = Convert.ToDecimal(Application.Current.Properties["cardmembertax"]);
                    decimal discountamount = Convert.ToDecimal(Application.Current.Properties["carddiscountamount"]);
                    string discount = Convert.ToString(Application.Current.Properties["carddiscount"]);
                    ClassCardName.Text = pk.Name;
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
                    if (creditApply > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Credit: {0:c}\r\n", -creditApply);
                    }
                    TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Total: {0:c}\r\n", total);
                    if (string.IsNullOrEmpty(discount) == false)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "*{0}\r\n", discount);
                    }
                    Ages.Text = pk.Ages;
                    CreditsStr.Text = $"{pk.Credits} Classes";
                    ValidDaysStr.Text = $"Card is Valid for {pk.ValidDays} Days";
                    break;
                }
            }
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
            await DisplayAlert("Gift Code Applied", summary, "Close");
            if (credit != "-1000")
            {
                Xamarin.Essentials.Preferences.Set("giftcode", GiftCode.Text);
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                account.StoreCredit += Convert.ToDecimal(credit);
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", Convert.ToDecimal(account.StoreCredit));
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("giftcode", "");
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
            CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", Convert.ToDecimal(account.StoreCredit));
            CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
            string creditApply = CreditApply.Text.Replace("-", "");
            decimal creditApplyAmount = 0;
            decimal creditAvailableAmount = account.StoreCredit;
            Application.Current.Properties["creditapply"] = 0;
            if (decimal.TryParse(creditApply, out creditApplyAmount) == true && creditApplyAmount > 0)
            {
                TotalStr.IsVisible = false;
                activityIndicator.IsVisible = true;
                decimal total = Convert.ToDecimal(Application.Current.Properties["cardtotal"]);
                if (creditApplyAmount > creditAvailableAmount)
                {
                    creditApplyAmount = creditAvailableAmount;
                }
                if (creditApplyAmount > total)
                {
                    creditApplyAmount = total;
                }
                Application.Current.Properties["creditapply"] = creditApplyAmount;
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
            CalculateCardPackageCalc();
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
            CalculateTotals();
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            decimal creditAvailable = account.StoreCredit - creditApply;
            CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", creditAvailable);
            CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", creditApply);
            CreditApplied.TextColor = Color.DarkGreen;
            CreditApply.Text = "";
            TotalStr.IsVisible = true;
            activityIndicator.IsVisible = false;
            await DisplayAlert("Credit was Applied to Payment", string.Format(new CultureInfo(gym.Culture), "{0:c} was applied to your payment.", creditApply), "Close");
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("notes", Notes.Text);
            Xamarin.Essentials.Preferences.Set("action", "classcard");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}