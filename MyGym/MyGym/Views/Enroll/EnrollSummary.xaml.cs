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
    public partial class EnrollSummary : ContentPage
    {
        public EnrollSummary()
        {
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        async protected override void OnAppearing()
        {
            try
            {
                Xamarin.Essentials.Preferences.Set("discountamount", "0");
                Application.Current.Properties["unlimited"] = false;
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
                ChildMobile child = null;
                foreach (ChildMobile c in account.Children)
                {
                    if (c.ChildId == childId)
                    {
                        child = c;
                        break;
                    }
                }
                Application.Current.Properties["includesocks"] = child.IncludeSocks;
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
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                ClassDateTime.Text = string.Format(new CultureInfo(gym.Culture), "{0:ddd} {0:MMM} {0:dd} - ", d) + string.Format(new CultureInfo(gym.Culture), "{0:h:mmt} to {1:h:mmt}", cl.Start, cl.End).ToLower(); ;
                CostSummary.Text = Xamarin.Essentials.Preferences.Get("costsummary", "");
                SocksText.Text = gym.ShowSocksText;
                ChildSocksCheckbox.IsChecked = child.IncludeSocks;
                if (child.SocksPurchased || child.SocksReceived)
                {
                    ChildSocksText.Text = "My Gym Socks - *has purchased or received socks";
                }
                else if (child.Age / 12.0 < gym.ShowSocksAgeMin)
                {
                    ChildSocksText.Text = "My Gym Socks - *does not require socks due to age";
                }
                int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                if (accountStatus == 7)
                {
                    CostSummary.Text += " *after the trial period ends";
                    EnrollTitle.Text = "Start Your Trial - Release of Liability";
                    TrialGrid.IsVisible = true;
                    TrialPeriod.IsVisible = true;
                    TrialPeriodConverts.IsVisible = true;
                    TrialPeriod.Text = string.Format(new CultureInfo(gym.Culture), "Trial Period: {0:d} - {1:d} ", d, d.AddDays((gym.TrialWeeks * 7) - 1));
                    TrialPeriodConverts.Text = string.Format(new CultureInfo(gym.Culture), "Trial Auto Converts on: {0:d}", d.AddDays(gym.TrialWeeks * 7));
                    if (gym.TrialNoPayment == true)
                    {
                        TrialCostLater.IsVisible = true;
                    }
                    EnrollButton.Text = "Start Your Trial";
                }
                else if (accountStatus == 2)
                {
                    TrialGrid.IsVisible = false;
                    Terms.Text = Xamarin.Essentials.Preferences.Get("terms", "");
                    CalculateTotals(child);
                }

                if (gym.OfferUnlimitedOnline == true && gym.UnlimitedFee > 0 && gym.UnlimitedEnroll == true)
                {
                    UnlimitedGrid.IsVisible = true;
                    UnlimitedText.Text = $"Update to Unlimited for {gym.UnlimitedFee:c}";
                    UnlimitedText1.Text = $"Take unlimited My Gym classes each week for an additional {gym.UnlimitedFee:c} per billing cycle.  Restrictions may apply.";
                    if (gym.Unlimited == true)
                    {
                        UnlimitedText2.IsVisible = true;
                        UnlimitedText2.Text = "*My Gym is currently running an Unlimited Classes promotion for all members. Upgrade today to keep your Unlimited status when the promotional period is over. You will not be charged until that time.";
                    }
                }
                else
                {
                    UnlimitedGrid.IsVisible = false;
                }
                CreditContainer.IsVisible = false;
                CreditAvailable.IsVisible = false;
                CreditApplied.IsVisible = false;
                if (accountStatus != 7 && gym.AllowStoreCreditOnWebsite == true && (gym.TrialNoPayment == false || accountStatus != 7) && account.StoreCredit > 0)
                {
                    CreditContainer.IsVisible = true;
                    CreditAvailable.IsVisible = true;
                    CreditApplied.IsVisible = true;
                    CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", account.StoreCredit);
                    CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", 0);
                }
                CalculateTotals(child);
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                await Shell.Current.GoToAsync("//errorpage");
            }
            base.OnAppearing();
        }

        async private void CalculateTotals(ChildMobile child)
        {
            try
            {
                decimal totalClassCost = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("totalclasscost", "0"));
                decimal totalTax = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("totaltax", "0"));
                decimal memberCost = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("membercost", "0"));
                decimal memberTax = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("membertax", "0"));
                decimal total = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("total", "0"));
                decimal discountamount = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("discount", "0"));
                decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);

                bool unlimited = Convert.ToBoolean(Application.Current.Properties["unlimited"]);
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                TotalStr.Text = "";
                Xamarin.Essentials.Preferences.Get("socksprice", "0");
                Xamarin.Essentials.Preferences.Get("sockstax", "0");
                int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                if (accountStatus == 2)
                {
                    TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Tuition: {0:c}\r\n", totalClassCost);
                    if (unlimited == true)
                    {
                        decimal unlimitedFee = gym.Unlimited == false ? gym.UnlimitedFee : 0;
                        total += unlimitedFee;
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Unlimited: {0:c}\r\n", unlimitedFee);
                        CostSummary.Text += $" + unlimited fee of {gym.UnlimitedFee:c}";
                        if (gym.Unlimited == true)
                        {
                            CostSummary.Text += " after promotion ends";
                        }
                    }
                    else
                    {
                        CostSummary.Text = Xamarin.Essentials.Preferences.Get("costsummary", "");
                    }
                    if (gym.ShowSocksOnCheckout == true && gym.SocksPrice > 0 && child.IncludeSocks)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "My Gym Socks: {0:c}\r\n", gym.SocksPrice);
                    }
                    if (memberCost > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Membership: {0:c}\r\n", memberCost);
                    }
                    if (memberTax > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Membership Tax: {0:c}\r\n", memberTax);
                    }
                    if (discountamount > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Promo Discount: {0:c}\r\n", -discountamount);
                    }
                    if (totalTax > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Tuition Tax: {0:c}\r\n", totalTax);
                    }
                    if (unlimited == true && gym.ClassTax > 0 && gym.Unlimited == false)
                    {
                        decimal unlimitedTax = gym.UnlimitedFee * (gym.ClassTax / 100.0M);
                        total += unlimitedTax;
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Unlimited Tax: {0:c}\r\n", unlimitedTax);
                    }
                    if (gym.ShowSocksOnCheckout == true && gym.SocksPrice > 0 && child.IncludeSocks)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "My Gym Socks Tax: {0:c}\r\n", gym.SocksTax);
                    }
                    if (creditApply > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Credit: {0:c}\r\n", -creditApply);
                    }
                    TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Total: {0:c}\r\n", total - creditApply);
                }
                else
                {
                    decimal trialCost = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("trialcost", "0"));
                    decimal trialTax = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("trialtax", "0"));
                    total = trialCost + trialTax;
                    TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Trial Cost: {0:c}\r\n", trialCost);
                    if (unlimited == true)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Unlimited: {0:c}\r\n", 0);
                        CostSummary.Text += $" + unlimited fee of {gym.UnlimitedFee:c}";
                        if (gym.Unlimited == true)
                        {
                            CostSummary.Text += " after promotion ends";
                        }
                    }
                    else
                    {
                        CostSummary.Text = Xamarin.Essentials.Preferences.Get("costsummary", "");
                    }
                    if (gym.ShowSocksOnCheckout == true && gym.SocksPrice > 0 && child.IncludeSocks)
                    {
                        total += gym.SocksPrice;
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "My Gym Socks: {0:c}\r\n", gym.SocksPrice);
                    }
                    if (trialTax > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Trial Tax: {0:c}\r\n", trialTax);
                    }
                    if (unlimited == true && gym.ClassTax > 0 && gym.Unlimited == false)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Unlimited Tax: {0:c}\r\n", 0);
                    }
                    if (gym.ShowSocksOnCheckout == true && gym.SocksPrice > 0 && child.IncludeSocks)
                    {
                        total += gym.SocksTax;
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "My Gym Socks Tax: {0:c}\r\n", gym.SocksTax);
                    }
                    if (discountamount > 0)
                    {
                        TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Promo Discount: {0:c} *applied to first payment\r\n", -discountamount);
                    }
                    TotalStr.Text += string.Format(new CultureInfo(gym.Culture), "Total: {0:c}\r\n", total);
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                await Shell.Current.GoToAsync("//errorpage");
            }
        }

        async private void Credit_Clicked(object sender, EventArgs e)
        {
            try
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
                if (decimal.TryParse(creditApply, out creditApplyAmount) == true && creditApplyAmount >= 0)
                {
                    string ca = CreditAvailable.Text.Replace("Credit Available: ", "").Substring(1);
                    string cp = CreditApply.Text.Replace("-", "");
                    decimal creditAvailableAmount = Convert.ToDecimal(ca);
                    Application.Current.Properties["creditapply"] = creditApplyAmount;
                    Application.Current.Properties["creditavailable"] = creditAvailableAmount;

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
                    await DisplayAlert("Credit Amount Incorrect", "Please enter a numeric value for credit amount.", "Close");
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                await Shell.Current.GoToAsync("//errorpage");
            }
        }

        async private void RunActionCredit(object sender, DoWorkEventArgs e)
        {
            try
            {
                int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", "0"));
                int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", "0"));
                int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", "0"));
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                decimal creditAvailableAmount = Convert.ToDecimal(Application.Current.Properties["creditavailable"]);
                decimal creditApplyAmount = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
                decimal total = accountStatus == 7 ? Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("trialcost", "0")) : Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("totalclasscost", "0"));
                decimal totalTax = accountStatus == 7 ? Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("trialtax", "0")) : Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("totaltax", "0"));
                if (creditApplyAmount > creditAvailableAmount)
                {
                    creditApplyAmount = creditAvailableAmount;
                }
                if (creditApplyAmount > total + totalTax)
                {
                    creditApplyAmount = total + totalTax;
                }
                creditAvailableAmount = creditAvailableAmount - creditApplyAmount;
                Application.Current.Properties["creditapply"] = creditApplyAmount;
                Application.Current.Properties["creditavailable"] = creditAvailableAmount;
                Application.Current.Properties["total"] = total;
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("gymIdTerms", gymId);
                ps.Add("childIdTerms", childId);
                ps.Add("classIdTerms", classId);
                ps.Add("accountStatus", accountStatus);
                ps.Add("storeCredit", creditApplyAmount);
                ps.Add("includeSocks", ((bool)Application.Current.Properties["includesocks"]) == true ? 1 : 0);
                string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/termsenroll", ps));
                string[] a = l.Split('|');
                if (accountStatus == 2)
                {
                    Xamarin.Essentials.Preferences.Set("costsummary", a[0]);
                    Xamarin.Essentials.Preferences.Set("totalclasscost", a[1].ToString());
                    Xamarin.Essentials.Preferences.Set("totaltax", a[2].ToString());
                    Xamarin.Essentials.Preferences.Set("total", a[5].ToString());
                    Terms.Text = a[6].Replace("rnrn", "\r\n\r\n");
                }
                else
                {
                    Xamarin.Essentials.Preferences.Set("costsummary", a[0]);
                    Xamarin.Essentials.Preferences.Set("totalclasscost", a[1].ToString());
                    Xamarin.Essentials.Preferences.Set("totaltax", a[2].ToString());
                    Xamarin.Essentials.Preferences.Set("total", a[3].ToString());
                    Terms.Text = a[6].Replace("rnrn", "\r\n\r\n");
                }
                Xamarin.Essentials.Preferences.Set("terms", Terms.Text);
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                await Shell.Current.GoToAsync("//errorpage");
            }
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
            try
            {
                CostSummary.Text = Xamarin.Essentials.Preferences.Get("costsummary", "");
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
                ChildMobile child = null;
                foreach (ChildMobile c in account.Children)
                {
                    if (c.ChildId == childId)
                    {
                        child = c;
                        break;
                    }
                }
                CalculateTotals(child);
                decimal creditApply = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
                CreditAvailable.Text = string.Format(new CultureInfo(gym.Culture), "Credit Available: {0:c}", account.StoreCredit - creditApply);
                CreditApplied.Text = string.Format(new CultureInfo(gym.Culture), "Credit Applied: {0:c}", creditApply);
                CreditApplied.TextColor = Color.DarkGreen;
                CreditApply.Text = "";
                activityIndicatorCredit.IsVisible = false;
                await DisplayAlert("Credit was Applied to Payment", string.Format(new CultureInfo(gym.Culture), "{0:c} was applied to your payment.", creditApply), "Close");
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                await Shell.Current.GoToAsync("//errorpage");
            }
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("notes", Notes.Text);
            string accountStatusStr = Xamarin.Essentials.Preferences.Get("accountstatus", "0");
            int accountStatus = accountStatusStr == "" || accountStatusStr == "null" ? 0 : Convert.ToInt32(accountStatusStr);
            if (accountStatus == 2)
            {
                Xamarin.Essentials.Preferences.Set("action", "enroll");
            }
            else if (accountStatus == 7)
            {
                Xamarin.Essentials.Preferences.Set("action", "trial");
            }
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//loading");
        }

        void ChildSocksCheckbox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            Application.Current.Properties["includesocks"] = isChecked;
            ChildMobile child = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    child.IncludeSocks = isChecked;
                    break;
                }
            }
            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionSocks;
            b.RunWorkerCompleted += RunWorkerCompletedSocks;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        async private void RunActionSocks(object sender, DoWorkEventArgs e)
        {
            try
            {
                int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", "0"));
                int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", "0"));
                int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
                int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("gymIdTerms", gymId);
                ps.Add("childIdTerms", childId);
                ps.Add("classIdTerms", classId);
                ps.Add("accountStatus", Convert.ToInt32(accountStatus));
                ps.Add("storeCredit", Application.Current.Properties["creditapply"]);
                ps.Add("includeSocks", ((bool)Application.Current.Properties["includesocks"]) == true ? 1 : 0);
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
                }

            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                await Shell.Current.GoToAsync("//errorpage");
            }
        }

        private void RunWorkerCompletedSocks(object sender, RunWorkerCompletedEventArgs e)
        {
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", "0"));
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            ChildMobile child = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            CalculateTotals(child);
        }

        void UnlimitedCheckbox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            Application.Current.Properties["unlimited"] = isChecked;
            ChildMobile child = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            CalculateTotals(child);
        }

    }
}