using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using mygymmobiledata;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace MyGym
{
    public partial class EventSummary : ContentPage
    {
        public EventSummary()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            string reset = Xamarin.Essentials.Preferences.Get("reset", "");
            if (reset == "1")
            {
                base.OnAppearing();
                return;
            }
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));

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
                EnrollTitle.Text = "Purchase Camp/Event Sessions";
                continueButton.Text = "Purchase Camps/Events";
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.ShowSocksOnCheckout && gym.SocksPrice > 0)
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                List<ChildMobile> childs = new List<ChildMobile>();
                foreach (ChildMobile c in account.Children)
                {
                    bool include = false;
                    if (ev.SelectedDates == null || ev.SelectedDates.Count == 0)
                    {
                        include = true;
                    }
                    else
                    {
                        foreach (EventDateMobile d in ev.SelectedDates)
                        {
                            if (d.ChildId == c.ChildId)
                            {
                                include = true;
                                break;
                            }
                        }
                    }
                    if (include)
                    {
                        c.SocksText = $"My Gym Socks for {c.First}";
                        c.SocksReceivedOrPurchasedText = "";
                        if (c.SocksPurchased || c.SocksReceived)
                        {
                            c.SocksReceivedOrPurchasedText = "*purchased or received in the past";
                        }
                        if (DateTime.Now.Subtract(c.DOB).TotalDays < gym.ShowSocksAgeMin * 365)
                        {
                            c.SocksReceivedOrPurchasedText = "*socks not required due to age";
                        }
                        ChildMobile cm = new ChildMobile();
                        UtilMobile.CopyPropertyValues(c, cm);
                        childs.Add(cm);
                    }
                }
                listView.ItemsSource = childs;
                listView.HeightRequest = childs.Count * 54;
                SocksText.Text = gym.ShowSocksText;
            }
            else
            {
                listView.IsVisible = false;
                SocksText.IsVisible = false;
            }
            CostSummary.Text = UtilMobile.CostSummaryEvent(true);
            string s = Xamarin.Essentials.Preferences.Get("discountsummary", "");
            if (s != "")
            {
                s = s.Replace(" Your discount will be applied at checkout.", "");
                CostSummary.Text += "\n" + s;
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

        async void ScheduleCampsEvents_Clicked(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("reset", "1");
            Xamarin.Essentials.Preferences.Set("action", "enrollcamp");
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            ObservableCollection<EventDateMobile> selectedDates = new ObservableCollection<EventDateMobile>();
            foreach (EventDateMobile d in ev.SelectedDates)
            {
                EventDateMobile a = new EventDateMobile();
                UtilMobile.CopyPropertyValues(d, a);
                selectedDates.Add(a);
            }
            ev.SelectedDates = selectedDates;
            await Shell.Current.GoToAsync("//loading");
        }

        void ChildSocksCheckbox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            CheckBox b = (CheckBox)sender;
            int childId = Convert.ToInt32(b.ClassId);
            bool isChecked = ((CheckBox)sender).IsChecked;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    c.IncludeSocks = isChecked;
                    break;
                }
            }
            CostSummary.Text = UtilMobile.CostSummaryEvent(true);
            string s = Xamarin.Essentials.Preferences.Get("discountsummary", "");
            if (s != "")
            {
                s = s.Replace(" Your discount will be applied at checkout.", "");
                CostSummary.Text += "\n" + s;
            }
        }
    }
}
