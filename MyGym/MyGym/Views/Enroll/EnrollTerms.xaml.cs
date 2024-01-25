using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Telerik.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollTerms : ContentPage
    {
        public EnrollTerms()
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
            Xamarin.Essentials.Preferences.Set("discount", "0");
            Xamarin.Essentials.Preferences.Set("discountsummary", "");
            Application.Current.Properties["creditapply"] = 0;
            Application.Current.Properties["creditavailable"] = 0;

            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            if (accountStatus == 7)
            {
                EnrollTitle.Text = "Start Your Trial Experience";
                if (gym.TrialNoPayment == true)
                {
                    PromoCodeContainer.IsVisible = false;
                    GiftCodeContainer.IsVisible = false;
                }
                else
                {
                    PromoCodeContainer.IsVisible = true;
                    GiftCodeContainer.IsVisible = true;
                }
            }

            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            ChildMobile child = null;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            ScheduleViewMobile classes = (ScheduleViewMobile)Application.Current.Properties["classes"];
            int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
            int classTemplateId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classtemplateid", ""));
            ClassView_ResultMobile cl = null;
            foreach (ClassListView_ResultMobile v in classes.ClassList)
            {
                if (v.ClassTemplateId == classTemplateId)
                {
                    foreach (ClassView_ResultMobile c in v.Classes)
                    {
                        if (c.Id == classId)
                        {
                            cl = c;
                            break;
                        }
                    }
                    break;
                }
            }
            DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
            ClassName.Text = child.First + " - " + cl.Display;
            ClassDateTime.Text = string.Format(new CultureInfo(gym.Culture), "{0:ddd} {0:MMM} {0:dd} - ", d) + string.Format(new CultureInfo(gym.Culture), "{0:h:mmt} to {1:h:mmt}", cl.Start, cl.End).ToLower(); ;

            if (accountStatus == 7)
            {
                GiftCodeContainer.IsVisible = false;
            }

            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionTerms;
            b.RunWorkerCompleted += RunWorkerCompletedTerms;
            b.RunWorkerAsync(new DoWorkEventArgs(null));

            base.OnAppearing();
        }

        private void RunActionTerms(object sender, DoWorkEventArgs e)
        {
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            if (accountStatus == 7)
            {
                EnrollTitle.Text = "Start Your Trial Experience";
            }
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            ChildMobile child = null;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            ScheduleViewMobile classes = (ScheduleViewMobile)Application.Current.Properties["classes"];
            int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
            int classTemplateId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classtemplateid", ""));
            ClassView_ResultMobile cl = null;
            foreach (ClassListView_ResultMobile v in classes.ClassList)
            {
                if (v.ClassTemplateId == classTemplateId)
                {
                    foreach (ClassView_ResultMobile c in v.Classes)
                    {
                        if (c.Id == classId)
                        {
                            cl = c;
                            break;
                        }
                    }
                    break;
                }
            }
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdTerms", gymId);
            ps.Add("childIdTerms", childId);
            ps.Add("classIdTerms", classId);
            ps.Add("accountStatus", Convert.ToInt32(accountStatus));
            ps.Add("storeCredit", 0);
            ps.Add("includeSocks", 0);
            string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/termsenroll", ps));
            string[] a = l.Split('|');
            if (accountStatus == 2)
            {
                Xamarin.Essentials.Preferences.Set("costsummary", a[0]);
                Xamarin.Essentials.Preferences.Set("totalclasscost", a[1].ToString());
                Xamarin.Essentials.Preferences.Set("totaltax", a[2].ToString());
                Xamarin.Essentials.Preferences.Set("membercost", a[3].ToString());
                Xamarin.Essentials.Preferences.Set("membertax", a[4].ToString());
                Xamarin.Essentials.Preferences.Set("total", a[5].ToString());
                Xamarin.Essentials.Preferences.Set("terms", a[6].Replace("rnrn", "\r\n\r\n"));
                Xamarin.Essentials.Preferences.Set("discount", a[7].ToString());
                Xamarin.Essentials.Preferences.Set("discountsummary", a[8].ToString());
            }
            else if (accountStatus == 7)
            {
                Xamarin.Essentials.Preferences.Set("costsummary", a[0]);
                Xamarin.Essentials.Preferences.Set("trialcost", a[1].ToString());
                Xamarin.Essentials.Preferences.Set("trialtax", a[2].ToString());
                Xamarin.Essentials.Preferences.Set("total", a[3].ToString());
                Xamarin.Essentials.Preferences.Set("totalclasscost", a[4].ToString());
                Xamarin.Essentials.Preferences.Set("totaltax", a[5].ToString());
                Xamarin.Essentials.Preferences.Set("terms", a[6].Replace("rnrn", "\r\n\r\n"));
                Xamarin.Essentials.Preferences.Set("discount", a[7].ToString());
                Xamarin.Essentials.Preferences.Set("discountsummary", a[8].ToString());
            }
        }

        async private void RunWorkerCompletedTerms(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            CostSummary.Text = Xamarin.Essentials.Preferences.Get("costsummary", "");
            Terms.Text = Xamarin.Essentials.Preferences.Get("terms", "");
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            if (accountStatus == 7)
            {
                EnrollTitle.Text = "Start Your Trial - Release of Liability";
                CostSummary.Text += " *after the trial period ends";
                TrialPeriod.IsVisible = true;
                TrialPeriodConverts.IsVisible = true;
                TrialCost.IsVisible = true;
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                TrialCost.Text = string.Format("{0} week(s) for {1:c}{2}", gym.TrialWeeks, Math.Round(Convert.ToDecimal(gym.TrialCost), 2), gym.ClassTax > 0 ? " + tax" : "");
                DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
                TrialPeriod.Text = string.Format(new CultureInfo(gym.Culture), "Trial Period: {0:d} - {1:d} ", d, d.AddDays((gym.TrialWeeks * 7) - 1));
                TrialPeriodConverts.Text = string.Format(new CultureInfo(gym.Culture), "Trial Auto Converts on: {0:d}", d.AddDays(gym.TrialWeeks * 7));
            }
            else
            {
                TrialPeriod.IsVisible = false;
                TrialPeriodConverts.IsVisible = false;
                TrialCost.IsVisible = false;
            }
        }

        private void AgreeButton_Clicked(object sender, EventArgs e)
        {
            paymentCheckbox.IsChecked = true;
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;
            if (paymentCheckbox.IsChecked == false || signatureView.IsBlank == true)
            {
                InputMissing.IsVisible = true;
                await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
            }
            else
            {
                var s = signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string sig = Convert.ToBase64String(b);
                Xamarin.Essentials.Preferences.Set("signature", "data:image/png;base64," + sig);
                await Shell.Current.Navigation.PushAsync(new EnrollBilling());
            }
        }

        private void Promo_Clicked(object sender, EventArgs e)
        {
            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionPromo;
            b.RunWorkerCompleted += RunWorkerCompletedPromo;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            activityIndicator.IsVisible = true;
            activityIndicator.IsEnabled = true;
            activityIndicator.IsRunning = true;
        }

        private void RunActionPromo(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
            DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdPromo", gymId);
            ps.Add("accountId", accountId);
            ps.Add("childId", childId);
            ps.Add("classId", classId);
            ps.Add("classDate", d.ToShortDateString());
            ps.Add("promoCode", PromoCode.Text);
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            ps.Add("trial", accountStatus == 7 ? 1 : 0);
            Application.Current.Properties["promoresult"] = UtilMobile.CallApiGetParamsString($"/api/gym/promoclass", ps);
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
            Xamarin.Essentials.Preferences.Set("totalclasscost", a[0].ToString());
            Xamarin.Essentials.Preferences.Set("totaltax", a[1].ToString());
            Xamarin.Essentials.Preferences.Set("membercost", a[2].ToString());
            Xamarin.Essentials.Preferences.Set("membertax", a[3].ToString());
            Xamarin.Essentials.Preferences.Set("total", a[4].ToString());
            Xamarin.Essentials.Preferences.Set("discount", a[5].ToString());
            Xamarin.Essentials.Preferences.Set("discountsummary", a[6].ToString());
            string summary = a[6];
            if (summary.StartsWith("Code not accepted") == false)
            {
                PromoCodeInvalid.IsVisible = false;
                PromoCodeContainer.IsVisible = false;
                PromoCodeAccepted.IsVisible = true;
                int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                if (accountStatus == 2)
                {
                    PromoCodeAccepted.Text = summary + " Discount will be applied at checkout.";
                }
                else
                {
                    PromoCodeAccepted.Text = summary + " Discount will be applied on your first payment.";
                }
                Application.Current.Properties["promocode"] = PromoCode.Text;
                Xamarin.Essentials.Preferences.Set("discountsummary", summary);
            }
            else
            {
                PromoCodeAccepted.IsVisible = false;
                PromoCodeContainer.IsVisible = true;
                PromoCodeInvalid.IsVisible = true;
                PromoCodeInvalid.Text = summary;
                Application.Current.Properties["promocode"] = "";
                Xamarin.Essentials.Preferences.Set("discountsummary", "");
            }
            EnrollTitle.IsVisible = true;
            scrollView.IsVisible = true;
            activityIndicator.IsVisible = false;
            activityIndicator.IsEnabled = false;
            activityIndicator.IsRunning = false;
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
                await DisplayAlert("Gift Code Applied", summary + " You can use the credit at checkout in the next steps.", "Close");
                Xamarin.Essentials.Preferences.Set("giftcode", GiftCode.Text);
                GiftCodeContainer.IsVisible = false;
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                account.StoreCredit += Convert.ToDecimal(credit);
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