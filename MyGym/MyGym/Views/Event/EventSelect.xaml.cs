using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using mygymmobiledata;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using System.ComponentModel;

namespace MyGym
{
    public partial class EventSelect : ContentPage
    {
        private static int SelectedDateHeight = 36;
        public EventSelect()
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
            Application.Current.Properties["creditapply"] = 0;
            Application.Current.Properties["creditavailable"] = 0;

            base.OnAppearing();

            activityIndicatorContinue.IsVisible = false;
            continueButton.IsVisible = true;
            Application.Current.Properties["selectedchildren"] = new List<ChildMobile>();
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            CampName.Text = ev.Display;
            AgeDesc.Text = ev.AgeDesc;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            List<ChildMobile> children = new List<ChildMobile>();
            foreach (ChildMobile c in account.Children)
            {
                bool isEnrolled = false;
                foreach (EnrollMobile em in c.Enrolls)
                {
                    if (em.Type == "Enrollment")
                    {
                        isEnrolled = true;
                    }
                }
                ChildMobile m = new ChildMobile();
                UtilMobile.CopyPropertyValues(c, m);
                int yrs = Convert.ToInt32(Math.Floor(DateTime.Now.Subtract(c.DOB).Days / 365.0M));
                int mos = Convert.ToInt32(Math.Floor((DateTime.Now.Subtract(c.DOB).Days % 365) / 30.0M));
                int age = (yrs * 12) + mos;
                m.ChildEnabled = ((age >= ev.StartAge - 1 && age <= ev.EndAge + 1) || (ev.StartAge == 0 && ev.EndAge == 0)) && (ev.MembersOnly == false || c.Member == true) && (ev.EnrolledOnly == false || isEnrolled);
                m.ChildNotEnabled = !((age >= ev.StartAge - 1 && age <= ev.EndAge + 1) || (ev.StartAge == 0 && ev.EndAge == 0));
                if (m.ChildNotEnabled == false)
                {
                    m.ChildNotEnabledMember = !(ev.MembersOnly == false || c.Member == true);
                    if (m.ChildNotEnabledMember == false)
                    {
                        m.ChildNotEnabledEnrolled = !(ev.EnrolledOnly == false || isEnrolled);
                    }
                }
                children.Add(m);
            }
            childrenList.ItemsSource = children;
            childrenList.HeightRequest = children.Count * 32;
            dateList.ItemsSource = ev.EventDates;
            dateList.HeightRequest = Convert.ToInt32(Math.Ceiling(ev.EventDates.Count / 2.0M)) * 52;
            dateSelectedList.ItemsSource = ev.SelectedDates;
            dateSelectedList.HeightRequest = ev.SelectedDates.Count * SelectedDateHeight;
            selectDatesLabel.IsVisible = false;
            if (ev.EventDates.Count == 0)
            {
                selectDatesLabel.IsVisible = true;
            }
            UpdateCostsCredits();
        }

        private void UpdateCostsCredits()
        {
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionUpdateCost;
            b.RunWorkerCompleted += RunWorkerCompletedUpdateCost;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionUpdateCost(object sender, DoWorkEventArgs e)
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
            EventCost c = ev.EventCost;
        }

        async private void RunWorkerCompletedUpdateCost(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            Cost.Text = "Total: " + ev.EventCost.SubTotalStr;
            if (ev.EventCost.Credits > 0)
            {
                AvailableCredits.Text = Convert.ToString(ev.EventCost.CreditsAvailable) + " available credit(s)";
                AppliedCredits.Text = Convert.ToString(ev.EventCost.CreditsApplied) + " credit(s) applied";
            }
            else
            {
                AvailableCredits.IsVisible = false;
                AppliedCredits.IsVisible = false;
            }
        }

        void Child_IsCheckedChanged(System.Object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            RadCheckBox b = (RadCheckBox)sender;
            int childId = Convert.ToInt32(b.ClassId);
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    if (Convert.ToBoolean(b.IsChecked))
                    {
                        ((List<ChildMobile>)Application.Current.Properties["selectedchildren"]).Add(c);
                    }
                    else
                    {
                        ((List<ChildMobile>)Application.Current.Properties["selectedchildren"]).Remove(c);
                    }
                    break;
                }
            }
            UpdateCostsCredits();
        }

        async void EventDate_Tapped(System.Object sender, System.EventArgs e)
        {
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            List<ChildMobile> childs = (List<ChildMobile>)Application.Current.Properties["selectedchildren"];
            Xamarin.Essentials.Preferences.Set("campdate", Convert.ToString(((TappedEventArgs)e).Parameter));
            if (childs.Count == 0)
            {
                await DisplayAlert("Select Children", "Please select at least one child", "Close");
            }
            else if (ev.ClassTemplateId1 > 0 && ev.EventCost.CreditsAvailable - childs.Count < 0)
            {
                await DisplayAlert("No More Credits", "You do not have enough available credits to schedule the selected children", "Close");
            }
            else
            {
                Application.Current.Properties["alreadyincamps"] = false;
                activityIndicatorEventDate.IsVisible = true;
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionCamp;
                b.RunWorkerCompleted += RunWorkerCompletedCamp;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }

        private void RunActionCamp(object sender, DoWorkEventArgs e)
        {
            List<ChildMobile> childs = (List<ChildMobile>)Application.Current.Properties["selectedchildren"];
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("campdate", ""));
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            DateTime rs, re;
            bool alreadyInCamps = false;
            int numDates = 0;
            foreach (EventDateMobile dm in ev.SelectedDates)
            {
                if (dm.DateStart == d)
                {
                    numDates++;
                }
            }
            foreach (EventDateMobile dm in ev.EventDates)
            {
                if (dm.DateStart == d)
                {
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gym.Id);
                    ps.Add("classId", dm.ClassId);
                    ClassView_ResultMobile cl = (ClassView_ResultMobile)UtilMobile.CallApiGetParams<ClassView_ResultMobile>("/api/gym/numattendingcamp", ps);
                    if (cl.Attendance + numDates + childs.Count > cl.Max)
                    {
                        Xamarin.Essentials.Preferences.Set("action", "campfull");
                        Xamarin.Essentials.Preferences.Set("spots", cl.Max - (cl.Attendance + numDates));
                        return;
                    }
                    string[] ss = dm.TimeStr.Split('-');
                    rs = new DateTime(d.Year, d.Month, d.Day, Convert.ToDateTime(ss[0]).Hour, Convert.ToDateTime(ss[0]).Minute, 0);
                    re = new DateTime(d.Year, d.Month, d.Day, Convert.ToDateTime(ss[1]).Hour, Convert.ToDateTime(ss[1]).Minute, 0);
                    foreach (ChildMobile c in childs)
                    {
                        ps = new Dictionary<string, object>();
                        ps.Add("childId", c.ChildId);
                        ps.Add("classId", dm.ClassId);
                        string s = UtilMobile.CallApiGetParamsString("/api/gym/childalreadyincamp", ps);
                        bool alreadyInCamp = Convert.ToInt32(s) == 1;
                        if (ev.SelectedDates.ToList().FindAll(m => m.DateStart == rs && m.ChildId == c.ChildId).Count == 0 && alreadyInCamp == false)
                        {
                            EventDateMobile cm = new EventDateMobile();
                            UtilMobile.CopyPropertyValues(dm, cm);
                            cm.ChildId = c.ChildId;
                            cm.SessionStr = string.Format(new CultureInfo(gym.Culture), "{0} - {1:d} {2:h:mmtt}-{3:h:mmtt}", c.First, cm.Date, rs, re);
                            ev.SelectedDates.Add(cm);
                        }
                        else
                        {
                            alreadyInCamps = true;
                        }
                    }
                    break;
                }
            }
            ev.SelectedDates.ToList().Sort((x, y) => x.DateStart.CompareTo(y.DateStart));
            if (alreadyInCamps == true)
            {
                Application.Current.Properties["alreadyincamps"] = true;
            }
        }

        private async void RunWorkerCompletedCamp(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                Xamarin.Essentials.Preferences.Set("action", "");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            else if (action == "campfull")
            {
                int spots = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("spots", ""));
                spots = spots < 0 ? 0 : spots;
                Xamarin.Essentials.Preferences.Set("action", "");
                await DisplayAlert("Camp Date Full", $"The are {spots} available spots in the camp date you are trying to book.", "Close");
                return;
            }
            bool alreadyInCamps = (bool)Application.Current.Properties["alreadyincamps"];
            if (alreadyInCamps == true)
            {
                await DisplayAlert("Already Enrolled in Camp", "Some of your selections where already enrolled and were not added", "Close");
            }
            activityIndicatorEventDate.IsVisible = false;
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            dateSelectedList.ItemsSource = ev.SelectedDates;
            dateSelectedList.HeightRequest = ev.SelectedDates.Count * SelectedDateHeight;
            dateList.IsVisible = true;
            UpdateCostsCredits();
            await scrollView.ScrollToAsync(continueButton, ScrollToPosition.MakeVisible, true);
        }

        void RemoveSession_Clicked(System.Object sender, System.EventArgs e)
        {
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            string sessionStr = Convert.ToString(((Button)sender).CommandParameter);
            foreach (EventDateMobile s in ev.SelectedDates)
            {
                if (s.SessionStr == sessionStr)
                {
                    ev.SelectedDates.Remove(s);
                    dateSelectedList.ItemsSource = ev.SelectedDates;
                    dateSelectedList.HeightRequest = ev.SelectedDates.Count * SelectedDateHeight;
                    break;
                }
            }
            dateSelectedList.IsVisible = false;
            activityIndicatorDelete.IsVisible = true;
            activityIndicatorDelete.IsRunning = true;
            UpdateCostsCredits();
            dateSelectedList.IsVisible = true;
            activityIndicatorDelete.IsVisible = false;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            if (ev.SelectedDates.Count == 0)
            {
                await DisplayAlert("No Sessions Selected", "Please select at least one camp/event session to continue", "Close");
            }
            else
            {
                activityIndicatorContinue.IsVisible = true;
                continueButton.IsVisible = false;
                await Shell.Current.Navigation.PushAsync(new EventDetail());
            }
        }
    }
}
