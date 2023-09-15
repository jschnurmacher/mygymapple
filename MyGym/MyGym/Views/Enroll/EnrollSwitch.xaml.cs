using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollSwitch : ContentPage
    {
        BackgroundWorker b;

        public EnrollSwitch()
        {
            InitializeComponent();
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            EnrollTitle.IsVisible = false;
            scrollView.IsVisible = false;
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdReadClasses", Convert.ToInt32(gymId));
            ps.Add("accountIdReadClasses", Convert.ToInt32(accountId));
            ps.Add("childId", Convert.ToInt32(childId));
            ps.Add("accountStatus", Convert.ToInt32(10));
            ps.Add("accountAction", Convert.ToInt32(0));
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
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            ChildMobile child = null;
            EnrollMobile enroll = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    foreach (EnrollMobile em in c.Enrolls)
                    {
                        if (em.EnrollId == enrollId)
                        {
                            enroll = em;
                            break;
                        }
                    }
                    break;
                }
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            ChildName.Text = $"{child.First}";
            ClassName.Text = $"{enroll.DisplayClass}";
            ScheduleViewMobile model = (ScheduleViewMobile)Application.Current.Properties["classes"];
            List<CustomListItemMobile> classes = new List<CustomListItemMobile>();
            foreach (ClassListView_ResultMobile m in model.ClassList)
            {
                foreach (ClassView_ResultMobile cl in m.Classes)
                {
                    classes.Add(new CustomListItemMobile { Text = string.Format(new CultureInfo(gym.Culture), "{0}", cl.DisplayFull), Value = cl.Id.ToString() });
                }
            }
            classes.Sort((x, y) => x.Text.CompareTo(y.Text));
            Classes.ItemsSource = classes;
            EnrollTitle.IsVisible = true;
            scrollView.IsVisible = true;
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;
            if (Dates.SelectedItem == null || Classes.SelectedItem == null)
            {
                InputMissing.IsVisible = true;
                await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
            }
            else
            {
                string date = ((CustomListItemMobile)Dates.SelectedItem).Text;
                string classIdSwitch = ((CustomListItemMobile)Classes.SelectedItem).Value;
                Application.Current.Properties["switchdate"] = date;
                Application.Current.Properties["switchclassid"] = classIdSwitch;
                Xamarin.Essentials.Preferences.Set("action", "switch");
                await Shell.Current.GoToAsync("//loading");
            }
        }

        private void Class_SelectionChanged(object sender, EventArgs e)
        {
            SwitchTerms.IsVisible = false;
            if (Dates.SelectedItem != null || Classes.SelectedItem != null)
            {
                activityIndicator.IsVisible = true;
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionClass;
                b.RunWorkerCompleted += RunWorkerCompletedClass;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }

        private void RunActionClass(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            string classIdSwitch = ((CustomListItemMobile)Classes.SelectedItem).Value;
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            ps.Add("enrollId", enrollId);
            ps.Add("classId", Convert.ToInt32(classIdSwitch));
            ClassListView_ResultMobile classTemplate = (ClassListView_ResultMobile)UtilMobile.CallApiGetParams<ClassListView_ResultMobile>("/api/gym/readswitchdates", ps);
            Application.Current.Properties["classtemplate"] = classTemplate;
        }

        async private void RunWorkerCompletedClass(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            ClassListView_ResultMobile classTemplate = (ClassListView_ResultMobile)Application.Current.Properties["classtemplate"];
            InputMissing.IsVisible = false;
            if (classTemplate.ClassDates.Count > 0)
            {
                classFull = false;
                Dates.ItemsSource = classTemplate.ClassDates;
                Dates.SelectedItem = classTemplate.ClassDates[0];
                Dates.Placeholder = "--select last class date--";
                SwitchClasses.IsVisible = true;
            }
            else
            {
                classFull = true;
                ObservableCollection<CustomListItemMobile> s = new System.Collections.ObjectModel.ObservableCollection<CustomListItemMobile>();
                Dates.Placeholder = "This class is full and can not be switched to";
                Dates.ItemsSource = s;
                Dates.SelectedItem = null;
                SwitchClasses.IsVisible = false;
                SwitchTerms.Text = "";
                SwitchTerms.IsVisible = false;
            }
            activityIndicator.IsVisible = false;
        }

        public bool classFull = false;
        void Dates_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            if (classFull)
            {
                classFull = false;
                return;
            }
            activityIndicator.IsVisible = true;
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionSwitch;
            b.RunWorkerCompleted += RunWorkerCompletedSwitch;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionSwitch(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            string date = ((CustomListItemMobile)Dates.SelectedItem).Text;
            string classIdSwitch = ((CustomListItemMobile)Classes.SelectedItem).Value;
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("enrollId", enrollId);
            ps.Add("date", date);
            ps.Add("classId", Convert.ToInt32(classIdSwitch));
            string s = UtilMobile.CallApiGetParamsString("/api/gym/readswitchterms", ps);
            string[] ss = s.Split('|');
            Xamarin.Essentials.Preferences.Set("switchcost", ss[0]);
            Xamarin.Essentials.Preferences.Set("switchterms", UtilMobile.ConvertHtml(ss[1]));
        }

        async private void RunWorkerCompletedSwitch(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            SwitchTerms.Text = Xamarin.Essentials.Preferences.Get("switchterms", "");
            SwitchTerms.IsVisible = true;
            activityIndicator.IsVisible = false;
        }
    }
}