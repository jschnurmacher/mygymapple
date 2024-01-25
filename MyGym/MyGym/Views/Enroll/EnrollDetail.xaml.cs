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
    public partial class EnrollDetail : ContentPage
    {
        public EnrollDetail()
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
            Xamarin.Essentials.Preferences.Set("membercost", "0");
            Xamarin.Essentials.Preferences.Set("membertax", "0");

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));

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
            TrialPeriod.IsVisible = false;
            TrialPeriodConverts.IsVisible = false;
            TrialCost.IsVisible = false;
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            if (accountStatus == 7)
            {
                EnrollTitle.Text = "Start Your Trial - Release of Liability";
                TrialPeriod.IsVisible = true;
                TrialPeriodConverts.IsVisible = true;
                TrialCost.IsVisible = true;
                TrialPeriod.Text = string.Format(new CultureInfo(gym.Culture), "Trial Period: {0:d} - {1:d} ", d, d.AddDays((gym.TrialWeeks * 7) - 1));
                TrialPeriodConverts.Text = string.Format(new CultureInfo(gym.Culture), "Trial Auto Converts on: {0:d}", d.AddDays(gym.TrialWeeks * 7));
                TrialCost.Text = string.Format("{0} week(s) for {1:c}{2}", gym.TrialWeeks, Math.Round(Convert.ToDecimal(gym.TrialCost), 2), gym.ClassTax > 0 ? " + tax" : "");
            }
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            EnrollTitle.IsVisible = false;
            scrollView.IsVisible = false;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdLiability", gym.Id);
            string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/liability", ps));
            Xamarin.Essentials.Preferences.Set("liability", l.Replace("\r\n", "\r\n\r\n"));
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
                await Shell.Current.Navigation.PushAsync(new EnrollTerms());
            }
        }

        private void AgreeButton_Clicked(object sender, EventArgs e)
        {
            liabilityCheckbox.IsChecked = true;
        }
    }
}