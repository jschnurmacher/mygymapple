using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Telerik.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymClass : ContentPage
    {
        ClassListView_ResultMobile cl = null;

        public GymClass()
        {
            Xamarin.Essentials.Preferences.Set("signature", "");
            Xamarin.Essentials.Preferences.Set("signatureguest", "0");
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            grid1.IsVisible = false;
            grid2.IsVisible = false;
            activityIndicator.IsVisible = true;

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));

            Xamarin.Essentials.Preferences.Set("redirected", "1");
            int classTemplateId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classtemplateid", ""));
            ScheduleViewMobile classesMobile = (ScheduleViewMobile)Application.Current.Properties["classes"];
            foreach (ClassListView_ResultMobile v in classesMobile.ClassList)
            {
                if (v.ClassTemplateId == classTemplateId)
                {
                    cl = v;
                    break;
                }
            }
            this.BindingContext = cl;

            string childId = Xamarin.Essentials.Preferences.Get("childid", "0");
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            scheduleTitle.Text = "Schedule Classes";
            if (childId != "" && childId != "null")
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                foreach (ChildMobile c in account.Children)
                {
                    if (c.ChildId == Convert.ToInt32(childId))
                    {
                        scheduleTitle.Text = c.First + " - ";
                        switch (accountStatus)
                        {
                            case 1:
                                scheduleTitle.Text += "Schedule Guest Class";
                                break;
                            case 2:
                                scheduleTitle.Text += "Enroll In Class";
                                TermsExpander.IsVisible = true;
                                ClassCost.IsVisible = true;
                                membershipMessage.IsVisible = true;
                                break;
                            case 3:
                                scheduleTitle.Text += "Schedule Makeup";
                                break;
                            case 4:
                                GymMobile gym1 = (GymMobile)Application.Current.Properties["gym"];
                                scheduleTitle.Text += $"Schedule {gym1.UnlimitedLabel}";
                                break;
                            case 5:
                                scheduleTitle.Text += "Schedule Class Card";
                                break;
                            case 6:
                                GymMobile gym2 = (GymMobile)Application.Current.Properties["gym"];
                                scheduleTitle.Text += $"Schedule {gym2.DropInLabel}";
                                break;
                            case 7:
                                scheduleTitle.Text += $"Start Your Trial";
                                TermsExpander.IsVisible = true;
                                ClassCost.IsVisible = true;
                                membershipMessage.IsVisible = true;
                                break;
                        }
                        break;
                    }
                }
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            CalculateClass(gym, cl, accountStatus);
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int classTemplateId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classtemplateid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdClassCost", gym.Id);
            ps.Add("classTemplateIdClassCost", classTemplateId);
            string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/classcost", ps));
            string[] a = l.Split('|');
            Xamarin.Essentials.Preferences.Set("classcost", a[0].ToString());
            Application.Current.Properties["terms"] = a[1].Replace("rnrn", "\r\n\r\n");
            Thread.Sleep(1000);
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
            string accountStatus = Xamarin.Essentials.Preferences.Get("accountstatus", "0");
            Terms.Text = (string)Application.Current.Properties["terms"];
            ClassCost.Text = Xamarin.Essentials.Preferences.Get("classcost", "");
            if (accountStatus == "7")
            {
                ClassCost.Text += " *after trial ends";
            }
            activityIndicator.IsVisible = false;
            grid1.IsVisible = true;
            grid2.IsVisible = true;
        }

        private string GetPhoto(string photo)
        {
            photo = photo.Replace("-", "").Replace(" ", "").Replace("&", "and").ToLower();
            string[] resourceNames = this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
            bool exists = false;
            foreach (var name in resourceNames)
            {
                if (name.ToLower().Contains(photo))
                {
                    exists = true;
                    photo = photo + ".jpg";
                    break;
                }
            }
            if (exists == false)
            {
                if (photo.Contains("dance"))
                {
                    photo = "dance1.jpg";
                }
                else if (photo.Contains("zumba"))
                {
                    photo = "zumbakids.jpg";
                }
                else if (photo.Contains("ballet"))
                {
                    photo = "balletjazzcombo.jpg";
                }
                else if (photo.Contains("free"))
                {
                    photo = "free.jpg";
                }
                else if (photo.Contains("sibling"))
                {
                    photo = "siblings.jpg";
                }
                else if (photo.Contains("sport"))
                {
                    photo = "sports.jpg";
                }
                else if (photo.Contains("parent") || photo.Contains("pno"))
                {
                    photo = "pno.jpg";
                }
                else if (photo.Contains("hip"))
                {
                    photo = "hiphop.jpg";
                }
                else if (photo.Contains("karate"))
                {
                    photo = "karate.jpg";
                }
                else if (photo.Contains("art"))
                {
                    photo = "art.jpg";
                }
                else
                {
                    photo = "logo.png";
                }
            }
            return photo;
        }

        private void CalculateClass(GymMobile gym, ClassListView_ResultMobile cl, int accountStatus)
        {
            cl.ProgramFull1 = false;
            cl.ProgramFull2 = false;
            cl.MembersOnly1 = false;
            cl.MembersOnly2 = false;
            cl.MembersOnly3 = false;
            cl.ChooseDate = false;
            cl.ChooseTime = false;

            bool unlimited = false;
            if (gym.Unlimited == true && DateTime.Now.Date >= gym.UnlimitedStart.Date && DateTime.Now.Date <= gym.UnlimitedEnd.Date)
            {
                unlimited = true;
            }
            ChildMobile childMobile = null;
            string childId = Xamarin.Essentials.Preferences.Get("childid", "");
            bool member = false;
            if (childId != "" && childId != "null")
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                foreach (ChildMobile c in account.Children)
                {
                    if (c.ChildId == Convert.ToInt32(childId))
                    {
                        childMobile = c;
                        if (c.Member == true)
                        {
                            member = true;
                        }
                    }
                }
            }
            membershipMessage.IsVisible = false;
            if (member == false && (accountStatus == 2 || accountStatus == 7))
            {
                membershipMessage.IsVisible = true;
                membershipMessage.Text = string.Format(new CultureInfo(gym.Culture), "{0:c} lifetime membership fee may be due depending on current membership status", gym.MembershipFee);
            }
            cl.DescLong = UtilMobile.ConvertHtml(cl.DescLong);
            cl.AgeDesc = UtilMobile.ConvertHtml(cl.AgeDesc);
            cl.Photo = GetPhoto(cl.Display);
            if (childMobile != null)
            {
                var classesExist = false;
                var bookingAllowedGuest = false;
                var bookingAllowed = false;
                var bookingAllowedTrial = false;
                var bookingAllowedDropIn = false;
                foreach (ClassView_ResultMobile t in cl.Classes)
                {
                    cl.TimesListDisplay = new ObservableCollection<CustomListItemMobile>();
                    for (int i = 0; i != 7; i++)
                    {
                        DayOfWeek dayofweek = DayOfWeek.Monday;
                        string day = UtilMobile.GetDayOfWeekInt(i, out dayofweek);
                        List<ClassView_ResultMobile> dayClasses = new List<ClassView_ResultMobile>();
                        foreach (ClassView_ResultMobile c in cl.Classes)
                        {
                            if (c.Start.DayOfWeek == dayofweek)
                            {
                                dayClasses.Add(c);
                            }
                        }
                        foreach (ClassView_ResultMobile c in dayClasses)
                        {
                            string time = "";
                            string timeAux = "";
                            time += "• " + day + " ";
                            time += string.Format(new System.Globalization.CultureInfo(gym.Culture), "{0:h:mm}{1}", c.Start, c.Start.Hour >= 12 ? "p" : "a");
                            if (c.ClassButtons.Count == 0)
                            {
                                timeAux += "*class full or ineligble";
                            }
                            else if (c.Start.Date > DateTime.Now.Date)
                            {
                                timeAux += string.Format(new System.Globalization.CultureInfo(gym.Culture), "*starts {0:d}", c.Start.Date);
                            }
                            else if (c.StartFuture != UtilMobile.MinDate)
                            {
                                timeAux += string.Format(new System.Globalization.CultureInfo(gym.Culture), "*moves to {0:h:mmt} {0:d}", c.StartFuture).ToLower();
                            }
                            else if (c.End < DateTime.Now.AddDays(56))
                            {
                                timeAux += string.Format(new System.Globalization.CultureInfo(gym.Culture), "*ends on {0:d}", c.End).ToLower();
                            }
                            cl.TimesListDisplay.Add(new CustomListItemMobile { Value = time, Text = timeAux });
                        }
                    }
                    cl.TimesListHeight = cl.TimesListDisplay.Count * 24;
                    cl.DescShort = "";
                    foreach (CustomListItemMobile i in cl.TimesListDisplay)
                    {
                        cl.DescShort += i.Value + " ";
                    }
                    if (t.ClassButtons.Count > 0)
                    {
                        classesExist = true;
                    }
                    if (t.BookingGuest == true)
                    {
                        bookingAllowedGuest = true;
                    }
                    if (t.Booking == true)
                    {
                        bookingAllowed = true;
                    }
                    if (t.BookingTrial == true)
                    {
                        bookingAllowedTrial = true;
                    }
                    if (t.BookingMakeup == true && childMobile != null)
                    {
                        bookingAllowed = true;
                    }
                    if (t.BookingDropIn == true)
                    {
                        bookingAllowedDropIn = true;
                    }
                    if (
                        (childMobile != null && ((unlimited == true && t.BookingUnlimited == true) || childMobile.AllowUnlimited == true))
                        || (childMobile != null && unlimited == false && t.BookingMakeup == true)
                        || (childMobile != null && accountStatus == 6 && t.BookingDropIn == true)
                        || (childMobile != null && accountStatus == 5 && t.BookingCard == true)
                    )
                    {
                        bookingAllowed = true;
                    }
                }
                if (classesExist == false && bookingAllowed == true)
                {
                    cl.ProgramFull1 = true;
                    cl.ProgramFull2 = true;
                }
                else if (bookingAllowed == false && bookingAllowedTrial == false && bookingAllowedGuest == false && bookingAllowedDropIn == true)
                {
                    cl.MembersOnly1 = true;
                    cl.MembersOnly2 = true;
                    cl.MembersOnly3 = true;
                }
                else if (bookingAllowed == false && bookingAllowedTrial == false && bookingAllowedGuest == false && bookingAllowedDropIn == false)
                {
                    cl.MembersOnly3 = true;
                }
                else
                {
                    cl.Buttons = new ObservableCollection<CustomListItemMobile>();
                    switch (accountStatus)
                    {
                        case 0:
                            if (gym.TrialEnabled)
                            {
                                cl.Buttons.Add(new CustomListItemMobile { Text = "Start Your Trial", Value = cl.ClassTemplateId.ToString() });
                            }
                            else if (gym.GuestEnabled)
                            {
                                cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Guest Class", Value = cl.ClassTemplateId.ToString() });
                            }
                            if (gym.EnrollEnabled)
                            {
                                cl.Buttons.Add(new CustomListItemMobile { Text = "Enroll Now", Value = cl.ClassTemplateId.ToString() });
                            }
                            if (gym.TrialEnabled == false && gym.GuestEnabled == false && gym.EnrollEnabled == false)
                            {
                                cl.Contact = true;
                            }
                            break;
                        case 1:
                            cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Guest Class", Value = cl.ClassTemplateId.ToString() });
                            break;
                        case 2:
                            cl.Buttons.Add(new CustomListItemMobile { Text = "Enroll Now", Value = cl.ClassTemplateId.ToString() });
                            break;
                        case 3:
                            cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Makeup", Value = cl.ClassTemplateId.ToString() });
                            break;
                        case 4:
                            cl.Buttons.Add(new CustomListItemMobile { Text = $"Schedule {gym.UnlimitedLabel}", Value = cl.ClassTemplateId.ToString() });
                            break;
                        case 5:
                            cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Class Card", Value = cl.ClassTemplateId.ToString() });
                            break;
                        case 6:
                            cl.Buttons.Add(new CustomListItemMobile { Text = $"Schedule {gym.DropInLabel}", Value = cl.ClassTemplateId.ToString() });
                            break;
                        case 7:
                            cl.Buttons.Add(new CustomListItemMobile { Text = "Start Your Trial", Value = cl.ClassTemplateId.ToString() });
                            break;
                    }
                    cl.ButtonsHeight = cl.Buttons.Count * 62;
                }
                Xamarin.Essentials.Preferences.Set("mode", "1");
            }
            else
            {
                cl.Buttons = new ObservableCollection<CustomListItemMobile>();
                cl.Buttons.Add(new CustomListItemMobile { Text = "Login or Register to Schedule", Value = "-1" });
                cl.ButtonsHeight = 1 * 62;
                Xamarin.Essentials.Preferences.Set("mode", "0");
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            int mode = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("mode", ""));
            if (mode == 0)
            {
                await Shell.Current.GoToAsync("//accounthome");
            }
            else if (mode == 1)
            {
                cl.ChooseTime = true;
                cl.ChooseDate = false;
                Xamarin.Essentials.Preferences.Set("mode", "2");
                int index = 1;
                cl.Buttons = new ObservableCollection<CustomListItemMobile>();
                cl.ButtonsHeight = cl.TimesList.Count * 62;
                foreach (CustomListItemTimeMobile t in cl.TimesList)
                {
                    ClassView_ResultMobile c = null;
                    foreach(ClassView_ResultMobile cx in cl.Classes)
                    {
                        if (cx.Id == Convert.ToInt32(t.Value))
                        {
                            c = cx;
                            break;
                        }
                    }
                    bool full = true;
                    foreach(ClassButtonMobile b in c.ClassButtons)
                    {
                        if (b.Attendance < b.Max)
                        {
                            full = false;
                            break;
                        }
                    }
                    string f = t.Text;
                    f = f.ToLower();
                    f = f.Replace("am", "a");
                    f = f.Replace("pm", "p");
                    f = UtilMobile.ProperCase(f);
                    if (full == true)
                    {
                        cl.Buttons.Add(new CustomListItemMobile { Value = index.ToString(), Text = f + " (Full)" });
                    }
                    else
                    {
                        cl.Buttons.Add(new CustomListItemMobile { Value = index.ToString(), Text = f });
                    }
                    index++;
                }
            }
            else if (mode == 2)
            {
                cl.ChooseTime = false;
                cl.ChooseDate = true;
                int timeIndex = Convert.ToInt32(((Button)sender).CommandParameter);
                Xamarin.Essentials.Preferences.Set("timeindex", timeIndex.ToString());
                CustomListItemTimeMobile mt = cl.TimesList[timeIndex - 1];
                int classId = Convert.ToInt32(mt.Value);
                Xamarin.Essentials.Preferences.Set("classid", classId.ToString());
                foreach (ClassView_ResultMobile t in cl.Classes)
                {
                    if (t.Id == classId)
                    {
                        bool full = true;
                        foreach (ClassButtonMobile b in t.ClassButtons)
                        {
                            if (b.Attendance < b.Max)
                            {
                                full = false;
                            }
                        }
                        if (full == true)
                        {
                            await DisplayAlert("Class is Full", "This class is full. Please choose another time.", "Close");
                        }
                        else
                        {
                            Xamarin.Essentials.Preferences.Set("mode", "3");
                            int index = 1;
                            cl.Buttons = new ObservableCollection<CustomListItemMobile>();
                            cl.ButtonsHeight = t.ClassButtons.Count * 62;
                            foreach (ClassButtonMobile b in t.ClassButtons)
                            {
                                string f = b.MobileStr;
                                f = f.ToLower();
                                f = UtilMobile.ProperCase(f);
                                if (b.Closed)
                                {
                                    cl.Buttons.Add(new CustomListItemMobile { Value = "-1", Text = f + " (Closed)" });
                                }
                                else if (b.Attendance >= b.Max)
                                {
                                    cl.Buttons.Add(new CustomListItemMobile { Value = "-2", Text = f + " (Full)" });
                                }
                                else
                                {
                                    cl.Buttons.Add(new CustomListItemMobile { Value = index.ToString(), Text = f });
                                }
                                index++;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                int dateIndex = Convert.ToInt32(((Button)sender).CommandParameter);
                if (dateIndex == -1)
                {
                    await DisplayAlert("Gym is Closed", "The Gym is closed this date. Please choose another date.", "Close");
                    return;
                }
                else if (dateIndex == -2)
                {
                    await DisplayAlert("Class is Full", "This class is full. Please choose another date.", "Close");
                    return;
                }
                int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
                foreach (ClassView_ResultMobile t in cl.Classes)
                {
                    if (t.Id == classId)
                    {
                        ClassButtonMobile b = t.ClassButtons[dateIndex - 1];
                        DateTime d = b.Date;
                        Xamarin.Essentials.Preferences.Set("classdate", d.ToShortDateString());
                        break;
                    }
                }
                int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                if (accountStatus == 2 || accountStatus == 7)
                {
                    await Shell.Current.Navigation.PushAsync(new EnrollDetail());
                }
                else if (accountStatus == 1 && gym.WaiverEnabled && string.IsNullOrEmpty(account.SignatureWaiver) == true && string.IsNullOrEmpty(account.SignatureWaivera) == true && string.IsNullOrEmpty(account.SignatureWaiverb) == true)
                {
                    await Shell.Current.Navigation.PushAsync(new EnrollGuestWaiver());
                }
                else if (accountStatus == 1 || accountStatus == 3 || accountStatus == 4 || accountStatus == 5 || accountStatus == 6)
                {
                    await Shell.Current.Navigation.PushAsync(new EnrollSingle());
                }
            }
        }

        private void Contact_Clicked(object sender, EventArgs e)
        {
        }

        private void AccountSchedule_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//accountschedule");
        }
    }
}