using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventDetail : ContentPage
    {
        BackgroundWorker b;

        public EventDetail()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            string reset = "";
            if (Xamarin.Essentials.Preferences.ContainsKey("reset"))
            {
                reset = Xamarin.Essentials.Preferences.Get("reset", "");
            }
            if (reset == "1")
            {
                base.OnAppearing();
                return;
            }
            Application.Current.Properties["promo"] = 0;
            Application.Current.Properties["promocode"] = "";
            Xamarin.Essentials.Preferences.Set("discountsummary", "");
            decimal creditApply = 0;
            if (Application.Current.Properties.ContainsKey("creditapply"))
            {
                creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            }

            EnrollTitle.IsVisible = false;
            scrollView.IsVisible = false;

            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            EventName.Text = ev.Display;
            SelectedSessions.Text = "";
            if (ev.SelectedDates != null && ev.SelectedDates.Count > 0)
            {
                foreach (EventDateMobile d in ev.SelectedDates)
                {
                    SelectedSessions.Text += string.Format("{0}\r\n", d.SessionStr);
                }
            }
            if (ev.EventDiscountMobile != null)
            {
                SelectedSessions.Text = ev.EventDiscountMobile.DisplayList;
                EnrollTitle.Text = "Book Camp/Event Sessions";
                continueButton.Text = "Book Camps/Events";
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            bool member = false;
            foreach (ChildMobile ch in account.Children)
            {
                if (ch.Member == true)
                {
                    member = true;
                    break;
                }
            }
            if (member == true || gym.MembershipEnabled == false || gym.MembershipFee <= 0)
            {
                MembershipGrid.IsVisible = false;
            }
            else
            {
                MembershipText.Text = string.Format(new CultureInfo(gym.Culture), "Purchase a My Gym Membership for {0:c} and Save on this and Future Transactions", gym.MembershipFee);
            }
            bool amount = true;
            if (ev != null && ev.EventCost != null && ev.EventCost.CreditsApplied >= ev.EventCost.Sessions)
            {
                amount = false;
            }
            CreditContainer.IsVisible = false;
            CreditAvailable.IsVisible = false;
            CreditApplied.IsVisible = false;
            if (amount == true && gym.AllowStoreCreditOnWebsite == true && account.StoreCredit > 0)
            {
                CreditContainer.IsVisible = true;
                CreditAvailable.IsVisible = true;
                CreditApplied.IsVisible = true;
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", account.StoreCredit);
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
            }
            if (account.Waiver == true || string.IsNullOrEmpty(account.SignatureWaiver) == false || string.IsNullOrEmpty(account.SignatureWaivera) == false || string.IsNullOrEmpty(account.SignatureWaiverb) == false)
            {
                signatureView.IsVisible = false;
                signatureLabel.IsVisible = false;
            }
            base.OnAppearing();
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            try
            {
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("gymIdLiability", gym.Id);
                string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/liability", ps));
                Xamarin.Essentials.Preferences.Set("liability", l.Replace("\r\n", "\r\n\r\n"));
            }
            catch { }
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
            ReleaseOfLiability.Text = Xamarin.Essentials.Preferences.Get("liability", "");
            EnrollTitle.IsVisible = true;
            scrollView.IsVisible = true;

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionCampCost;
            b.RunWorkerCompleted += RunWorkerCompletedCampCost;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionCampCost(object sender, DoWorkEventArgs e)
        {
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            string childrenIds = "";
            int numSessions = 0;
            if (ev.SelectedDates != null && ev.SelectedDates.Count > 0 && ev.EventDiscountMobile == null)
            {
                numSessions = ev.SelectedDates.Count;
                List<int> childrenIdList = new List<int>();
                foreach (EventDateMobile d in ev.SelectedDates)
                {
                    if (childrenIdList.Contains(d.ChildId) == false)
                    {
                        childrenIdList.Add(d.ChildId);
                    }
                }
                foreach (int i in childrenIdList)
                {
                    if (childrenIds != "")
                    {
                        childrenIds += ",";
                    }
                    childrenIds += i.ToString();
                }
            }
            else if (ev.EventDiscountMobile != null)
            {
                int i = ev.EventDiscountMobile.DisplayList.IndexOf(" ");
                numSessions = Convert.ToInt32(ev.EventDiscountMobile.DisplayList.Substring(0, i));
            }
            string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            ps.Add("accountId", accountId);
            ps.Add("childrenIds", childrenIds);
            ps.Add("classTemplateId", ev.EventId);
            ps.Add("numSessions", numSessions);
            ps.Add("promoCode", promoCode);
            ps.Add("package", ev.EventDiscountMobile == null ? 0 : ev.EventDiscountMobile.Id);
            ps.Add("membership", Xamarin.Essentials.Preferences.Get("membership", "") == "1" ? 1 : 0);
            ev.EventCost = (EventCost)UtilMobile.CallApiGetParams<EventCost>("/api/gym/readcampcost", ps);
            Application.Current.Properties["promoresult"] = ev.EventCost.DiscountSummary;
        }

        async private void RunWorkerCompletedCampCost(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            CostSummary.Text = "";
            CostSummary.Text += $"{ev.EventCost.Desc}\r\n";
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (ev.EventCost.Total > 0)
            {
                CostSummary.Text = UtilMobile.CostSummaryEvent(false);
            }
            else
            {
                continueButton.Text = "Schedule Camps/Events";
                PromoCodeContainer.IsVisible = false;
            }
            promoButton.IsVisible = true;
            activityIndicatorPromo.IsVisible = false;
            string s = (string)Application.Current.Properties["promoresult"];
            int promo = (int)Application.Current.Properties["promo"];
            if (promo == 1)
            {
                if (s.StartsWith("Code not accepted") == false)
                {
                    PromoCodeInvalid.IsVisible = false;
                    PromoCodeContainer.IsVisible = false;
                    PromoCodeAccepted.IsVisible = true;
                    PromoCodeAccepted.Text = s + " Your discount will be applied at checkout.";
                    Xamarin.Essentials.Preferences.Set("discountsummary", s);
                }
                else
                {
                    PromoCodeAccepted.IsVisible = false;
                    PromoCodeContainer.IsVisible = true;
                    PromoCodeInvalid.IsVisible = true;
                    PromoCodeInvalid.Text = s;
                    Xamarin.Essentials.Preferences.Set("discountsummary", "");
                }
                EnrollTitle.IsVisible = true;
                scrollView.IsVisible = true;
                CostSummary.Text = "";
                CostSummary.Text += $"{ev.EventCost.Desc}\r\n";
                if (ev.EventCost.Total > 0)
                {
                    CostSummary.Text = UtilMobile.CostSummaryEvent(false);
                }
                else
                {
                    continueButton.Text = "Schedule Camps/Events";
                }
                Application.Current.Properties["promo"] = 0;
            }
        }

        private void Promo_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["promocode"] = PromoCode.Text;
            Application.Current.Properties["promo"] = 1;
            promoButton.IsVisible = false;
            activityIndicatorPromo.IsVisible = true;
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionCampCost;
            b.RunWorkerCompleted += RunWorkerCompletedCampCost;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        void MembershipButton_Clicked(System.Object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            activityIndicatorMembership.IsVisible = true;
            MembershipText.IsVisible = false;
            membershipCheckbox.IsVisible = false;
            BackgroundWorker b1;
            b1 = new BackgroundWorker();
            b1.WorkerReportsProgress = true;
            b1.WorkerSupportsCancellation = true;
            b1.DoWork += RunActionMembership;
            b1.RunWorkerCompleted += RunWorkerCompletedMembership;
            b1.RunWorkerAsync(new DoWorkEventArgs(null));
            RadCheckBox b = (RadCheckBox)sender;
            if (Convert.ToBoolean(b.IsChecked))
            {
                Xamarin.Essentials.Preferences.Set("membership", "1");
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("membership", "");
            }
        }

        private void RunActionMembership(object sender, DoWorkEventArgs e)
        {
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
        }

        async private void RunWorkerCompletedMembership(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            CostSummary.Text = "";
            CostSummary.Text += $"{ev.EventCost.Desc}\r\n";
            if (ev.EventCost.Total > 0)
            {
                CostSummary.Text = UtilMobile.CostSummaryEvent(false);
            }
            activityIndicatorMembership.IsVisible = false;
            MembershipText.IsVisible = true;
            membershipCheckbox.IsVisible = true;

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionCampCost;
            b.RunWorkerCompleted += RunWorkerCompletedCampCost;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
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

            string creditApply = CreditApply.Text.Replace("-", "");
            decimal creditApplyAmount = 0;
            if (decimal.TryParse(creditApply, out creditApplyAmount) == true && creditApplyAmount > 0)
            {
                EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
                if (creditApplyAmount > ev.EventCost.Total)
                {
                    creditApplyAmount = ev.EventCost.Total;
                }
                Application.Current.Properties["creditapply"] = creditApplyAmount;
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
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
        }

        async private void RunWorkerCompletedCredit(object sender, RunWorkerCompletedEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            decimal creditApplyAmount = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
            decimal creditAvailableAmount = Convert.ToDecimal(account.StoreCredit - creditApplyAmount);
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            CostSummary.Text = "";
            CostSummary.Text += $"{ev.EventCost.Desc}\r\n";
            if (ev.EventCost.Total > 0)
            {
                CostSummary.Text = UtilMobile.CostSummaryEvent(false);
            }
            if (creditApplyAmount > 0)
            {
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", creditAvailableAmount);
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", creditApplyAmount);
                CreditApplied.TextColor = Color.DarkGreen;
                CreditApply.Text = "";
            }
            if (creditAvailableAmount <= 0)
            {
                CreditContainer.IsVisible = false;
            }
            activityIndicatorCredit.IsVisible = false;
            await DisplayAlert("Credit was Applied to Payment", string.Format(new CultureInfo(gym.Culture), "{0:c} was applied to your payment.", creditApplyAmount), "Close");

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionCampCost;
            b.RunWorkerCompleted += RunWorkerCompletedCampCost;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            bool waiver = false;
            if (account.Waiver == false && string.IsNullOrEmpty(account.SignatureWaiver) && string.IsNullOrEmpty(account.SignatureWaivera) && string.IsNullOrEmpty(account.SignatureWaiverb))
            {
                if (signatureView.IsBlank == false)
                {
                    var s = signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, (int)s.Length);
                    string sig = Convert.ToBase64String(b);
                    Xamarin.Essentials.Preferences.Set("signature", "data:image/png;base64," + sig);
                }
                else
                {
                    Xamarin.Essentials.Preferences.Set("signature", "");
                }
                waiver = true;
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("signature", "");
            }
            if (liabilityCheckbox.IsChecked == false)
            {
                await DisplayAlert("Agree to Liablility", "Please Agree to Liability Waiver", "Close");
            }
            else if (signatureView.IsBlank == true && waiver == true)
            {
                await DisplayAlert("Agree to Liablility", "Please sign with your finger or mouse to agree Liability Waiver", "Close");
            }
            else
            {
                EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
                if (ev.EventCost.Total <= 0)
                {
                    Xamarin.Essentials.Preferences.Set("action", "enrollcamp");
                    Xamarin.Essentials.Preferences.Set("actionaux", "enrollcamp");
                    await Shell.Current.Navigation.PopToRootAsync();
                    await Shell.Current.GoToAsync("//loading");
                }
                else
                {
                    await Shell.Current.Navigation.PushAsync(new EventBilling());
                }
            }
        }

        private void AgreeButton_Clicked(object sender, EventArgs e)
        {
            liabilityCheckbox.IsChecked = true;
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
            if (credit != "-1000")
            {
                await DisplayAlert("Gift Code Applied", summary + " You can now use the credit.", "Close");
                Xamarin.Essentials.Preferences.Set("giftcode", GiftCode.Text);
                GiftCodeContainer.IsVisible = false;
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                account.StoreCredit += Convert.ToDecimal(credit);
                CreditContainer.IsVisible = true;
                CreditAvailable.IsVisible = true;
                CreditApplied.IsVisible = true;
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", account.StoreCredit);
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
            }
            else
            {
                await DisplayAlert("Gift Code Invalid", summary + " Please check your gift code and re-enter.", "Close");
                Xamarin.Essentials.Preferences.Set("giftcode", "");
            }
            giftButton.IsVisible = true;
            activityIndicatorGift.IsVisible = false;
        }
    }
}