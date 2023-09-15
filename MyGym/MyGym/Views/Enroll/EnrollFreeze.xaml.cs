using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollFreeze : ContentPage
    {
        BackgroundWorker b;

        public EnrollFreeze()
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
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            ps.Add("enrollId", enrollId);
            ClassListView_ResultMobile classTemplate = (ClassListView_ResultMobile)UtilMobile.CallApiGetParams<ClassListView_ResultMobile>("/api/gym/readfreeze", ps);
            Application.Current.Properties["classtemplate"] = classTemplate;
            Xamarin.Essentials.Preferences.Set("classtemplateid", classTemplate.ClassTemplateId.ToString());
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
            ChildMobile child = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            ClassListView_ResultMobile classTemplate = (ClassListView_ResultMobile)Application.Current.Properties["classtemplate"];
            ClassName.Text = child.First + " - " + classTemplate.Display;
            Dates.ItemsSource = classTemplate.ClassDates;
            Weeks.ItemsSource = classTemplate.Sessions;
            EnrollTitle.Text = $"Freeze - {child.First} - {classTemplate.Display}";
            EnrollTitle.IsVisible = true;
            scrollView.IsVisible = true;
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;
            if (Dates.SelectedItem == null || Weeks.SelectedItem == null || signatureView.IsBlank == true)
            {
                InputMissing.IsVisible = true;
                await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
            }
            else
            {
                string date = ((CustomListItemMobile)Dates.SelectedItem).Text;
                string weeks = ((CustomListItemMobile)Weeks.SelectedItem).Text;
                Application.Current.Properties["freezedate"] = Convert.ToDateTime(date);
                Application.Current.Properties["freezeweeks"] = weeks;
                var s = await signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string sig = Convert.ToBase64String(b);
                Xamarin.Essentials.Preferences.Set("signature", "data:image/png;base64," + sig);
                Xamarin.Essentials.Preferences.Set("action", "freeze");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//loading");
            }
        }

        private void Weeks_SelectionChanged(object sender, EventArgs e)
        {
            FreezeTerms.IsVisible = false;
            if (Dates.SelectedItem != null || Weeks.SelectedItem != null)
            {
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionWeeks;
                b.RunWorkerCompleted += RunWorkerCompletedWeeks;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }

        private void RunActionWeeks(object sender, DoWorkEventArgs e)
        {
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            string date = ((CustomListItemMobile)Dates.SelectedItem).Text;
            string weeks = ((CustomListItemMobile)Weeks.SelectedItem).Text;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            ps.Add("enrollId", enrollId);
            ps.Add("date", date);
            ps.Add("weeks", Convert.ToInt32(weeks));
            string s = UtilMobile.CallApiGetParamsString("/api/gym/readfreezeterms", ps);
            string[] ss = s.Split('|');
            Xamarin.Essentials.Preferences.Set("freezecost", ss[0]);
            Xamarin.Essentials.Preferences.Set("freeztax", ss[1]);
            Xamarin.Essentials.Preferences.Set("total", ss[2]);
            Xamarin.Essentials.Preferences.Set("freezeterms", UtilMobile.ConvertHtml(ss[3]));
        }

        private async void RunWorkerCompletedWeeks(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            FreezeTerms.Text = Xamarin.Essentials.Preferences.Get("freezeterms", "");
            FreezeTerms.IsVisible = true;
            await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
        }

        void Dates_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            FreezeTerms.IsVisible = false;
            if (Weeks.SelectedItem != null)
            {
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionWeeks;
                b.RunWorkerCompleted += RunWorkerCompletedWeeks;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }
    }
}