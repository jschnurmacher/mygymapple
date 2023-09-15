using System;
using System.Collections.Generic;
using System.ComponentModel;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartyDate : ContentPage
    {
        BackgroundWorker b;

        public PartyDate()
        {
            InitializeComponent();
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
        }

        protected override void OnAppearing()
        {
            months.IsVisible = false;
            dates.IsVisible = false;
            times.IsVisible = false;
            activityIndicator.IsVisible = true;
            base.OnAppearing();
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
            string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", Convert.ToInt32(gymId));
            ps.Add("accountId", Convert.ToInt32(accountId));
            PartyDatesMobile party = (PartyDatesMobile)UtilMobile.CallApiGetParams<PartyDatesMobile>($"/api/gym/readpartydates", ps);
            Application.Current.Properties["partydates"] = party;
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
            PartyDatesMobile partyDates = (PartyDatesMobile)Application.Current.Properties["partydates"];
            months.ItemsSource = partyDates.Months;
            activityIndicator.IsVisible = false;
            months.IsVisible = true;
            dates.IsVisible = true;
            times.IsVisible = true;
        }

        void months_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            PartyMonthMobile m = (PartyMonthMobile)months.SelectedItem;
            PartyDatesMobile partyDates = (PartyDatesMobile)Application.Current.Properties["partydates"];
            foreach (PartyMonthMobile pm in partyDates.Months)
            {
                if (pm.MonthInt == m.MonthInt)
                {
                    Application.Current.Properties["partyselectedmonth"] = pm;
                    dates.IsEnabled = true;
                    dates.SelectedItem = null;
                    dates.ItemsSource = pm.Dates;
                    times.IsEnabled = false;
                    continueButton.IsVisible = false;
                    break;
                }
            }
        }

        void dates_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            if (dates.SelectedItem == null)
            {
                return;
            }
            PartyMonthMobile m = (PartyMonthMobile)Application.Current.Properties["partyselectedmonth"];
            PartyDateMobile d = (PartyDateMobile)dates.SelectedItem;
            PartyDatesMobile partyDates = (PartyDatesMobile)Application.Current.Properties["partydates"];
            foreach (PartyMonthMobile pm in partyDates.Months)
            {
                if (pm.MonthInt == m.MonthInt)
                {
                    foreach (PartyDateMobile pd in pm.Dates)
                    {
                        if (pd.DateDate.Date == d.DateDate.Date)
                        {
                            times.IsEnabled = true;
                            times.ItemsSource = pd.Times;
                            times.SelectedItem = null;
                            continueButton.IsVisible = false;
                            break;
                        }
                    }
                }
            }
        }

        void times_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            if (times.SelectedItem == null)
            {
                return;
            }
            PartyTimeMobile t = (PartyTimeMobile)times.SelectedItem;
            Application.Current.Properties["partyselectedtime"] = t;
            continueButton.IsVisible = true;
        }

        async void ContinueButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new PartyChildren());
        }
    }
}
