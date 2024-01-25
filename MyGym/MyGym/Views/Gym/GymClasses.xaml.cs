using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymClasses : ContentPage
    {
        public GymClasses()
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
            base.OnAppearing();
            listView.IsVisible = false;
            selectProgram.IsVisible = false;
            activityIndicator.IsVisible = true;
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            string accountId = "0";
            string childId = Xamarin.Essentials.Preferences.Get("childid", "0");
            string accountStatus = Xamarin.Essentials.Preferences.Get("accountstatus", "0");
            string accountAction = Xamarin.Essentials.Preferences.Get("accountaction", "0");
            if (childId != "" && childId != "null")
            {
                accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
            }
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdReadClasses", gym.Id);
            ps.Add("accountIdReadClasses", accountId == "null" || accountId == "" ? 0 : Convert.ToInt32(accountId));
            ps.Add("childId", childId == "" || childId == "null" ? 0 : Convert.ToInt32(childId));
            ps.Add("accountStatus", accountStatus == "" || accountStatus == "null" ? 0 : Convert.ToInt32(accountStatus));
            ScheduleViewMobile classesMobile = (ScheduleViewMobile)UtilMobile.CallApiGetParams<ScheduleViewMobile>($"/api/gym/readclasses", ps);
            Application.Current.Properties["classes"] = classesMobile;
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
            ScheduleViewMobile model = (ScheduleViewMobile)Application.Current.Properties["classes"];
            string childId = Xamarin.Essentials.Preferences.Get("childid", "0");
            string accountStatus = Xamarin.Essentials.Preferences.Get("accountstatus", "0");
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
                            case "1":
                                scheduleTitle.Text += "Schedule Guest Class";
                                break;
                            case "2":
                                scheduleTitle.Text += "Enroll In Class";
                                break;
                            case "3":
                                scheduleTitle.Text += "Schedule Makeup";
                                break;
                            case "4":
                                GymMobile gym1 = (GymMobile)Application.Current.Properties["gym"];
                                scheduleTitle.Text += $"Schedule {gym1.UnlimitedLabel}";
                                break;
                            case "5":
                                scheduleTitle.Text += "Schedule Class Card";
                                break;
                            case "6":
                                GymMobile gym2 = (GymMobile)Application.Current.Properties["gym"];
                                scheduleTitle.Text += $"Schedule {gym2.DropInLabel}";
                                break;
                            case "7":
                                scheduleTitle.Text += $"Start Your Trial";
                                break;
                        }
                        break;
                    }
                }
            }

            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            foreach (ClassListView_ResultMobile cl in model.ClassList)
            {
                CalculateClass(gym, cl, accountStatus);
            }
            listView.ItemsSource = model.ClassList;
            Application.Current.Properties["classes"] = model;
            if (model.ClassList.Count > 0)
            {
                selectProgram.IsVisible = true;
                noClasses.IsVisible = false;
            }
            else
            {
                selectProgram.IsVisible = false;
                noClasses.IsVisible = true;
            }

            activityIndicator.IsVisible = false;
            listView.IsVisible = true;
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

        private void CalculateClass(GymMobile gym, ClassListView_ResultMobile cl, string accountStatus)
        {
            bool unlimited = false;
            if (gym != null && gym.Unlimited == true && DateTime.Now.Date >= gym.UnlimitedStart.Date && DateTime.Now.Date <= gym.UnlimitedEnd.Date)
            {
                unlimited = true;
            }
            ChildMobile childMobile = null;
            string childId = Xamarin.Essentials.Preferences.Get("childid", "");
            if (childId != "" && childId != "null")
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                foreach (ChildMobile c in account.Children)
                {
                    if (c.ChildId == Convert.ToInt32(childId))
                    {
                        childMobile = c;
                        break;
                    }
                }
            }
            cl.DescLong = UtilMobile.ConvertHtml(cl.DescLong);
            cl.AgeDesc = UtilMobile.ConvertHtml(cl.AgeDesc);
            cl.Photo = GetPhoto(cl.Display);
            cl.Display = " " + cl.Display;

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
                        bool full = true;
                        foreach(ClassButtonMobile b in c.ClassButtons)
                        {
                            if (b.Attendance < b.Max)
                            {
                                full = false;
                                break;
                            }
                        }
                        if (full == true)
                        {
                            timeAux += "*class full or ineligible";
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
                cl.TimesListHeight = cl.TimesListDisplay.Count * 22;
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
                    || (childMobile != null && accountStatus == "6" && t.BookingDropIn == true)
                    || (childMobile != null && accountStatus == "5" && t.BookingCard == true)
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
                    case "":
                    case "null":
                    case "0":
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
                    case "1":
                        cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Guest Class", Value = cl.ClassTemplateId.ToString() });
                        break;
                    case "2":
                        cl.Buttons.Add(new CustomListItemMobile { Text = "Enroll Now", Value = cl.ClassTemplateId.ToString() });
                        break;
                    case "3":
                        cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Makeup", Value = cl.ClassTemplateId.ToString() });
                        break;
                    case "4":
                        cl.Buttons.Add(new CustomListItemMobile { Text = $"Schedule {gym.UnlimitedLabel}", Value = cl.ClassTemplateId.ToString() });
                        break;
                    case "5":
                        cl.Buttons.Add(new CustomListItemMobile { Text = "Schedule Class Card", Value = cl.ClassTemplateId.ToString() });
                        break;
                    case "6":
                        cl.Buttons.Add(new CustomListItemMobile { Text = $"Schedule {gym.DropInLabel}", Value = cl.ClassTemplateId.ToString() });
                        break;
                    case "7":
                        cl.Buttons.Add(new CustomListItemMobile { Text = "Start Your Trial", Value = cl.ClassTemplateId.ToString() });
                        break;
                }
                cl.ButtonsHeight = cl.Buttons.Count * 56;
            }
        }

        private void Class_Tapped(object sender, EventArgs e)
        {
            int classTemplateId = Convert.ToInt32(((TappedEventArgs)e).Parameter);
            Xamarin.Essentials.Preferences.Set("classtemplateid", classTemplateId.ToString());
            Shell.Current.Navigation.PushAsync(new GymClass()); ;
        }

        private async void AccountSchedule_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountschedule");
        }
    }
}