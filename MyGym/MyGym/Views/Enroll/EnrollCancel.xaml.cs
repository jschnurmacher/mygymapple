using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Telerik.XamarinForms.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollCancel : ContentPage
    {
        public EnrollCancel()
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
            cancelContent.IsVisible = false;
            activityIndicator.IsVisible = true;

            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            EnrollMobile enroll = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    foreach (EnrollMobile m in c.Enrolls)
                    {
                        if (m.EnrollId == enrollId)
                        {
                            enroll = m;
                            break;
                        }
                    }
                    break;
                }
            }
            ClassName.Text = enroll.Display;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            ClassDateTime.Text = string.Format(new CultureInfo(gym.Culture), "{0:ddd} {0:h:mmtt}", enroll.Start);

            BackgroundWorker b1 = new BackgroundWorker();
            b1.WorkerReportsProgress = true;
            b1.WorkerSupportsCancellation = true;
            b1.DoWork += RunActionReasons;
            b1.RunWorkerCompleted += RunWorkerCompletedReasons;
            b1.RunWorkerAsync("");

            settingDate = true;
            BackgroundWorker b2 = new BackgroundWorker();
            b2.WorkerReportsProgress = true;
            b2.WorkerSupportsCancellation = true;
            b2.DoWork += RunActionDates;
            b2.RunWorkerCompleted += RunWorkerCompletedDates;
            b2.RunWorkerAsync("");
            settingDate = false;

            base.OnAppearing();
        }

        private void RunActionReasons(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps = new Dictionary<string, object>();
            ps.Add("gymId", Convert.ToInt32(gym.Id));
            List<CustomListItemMobile> cancelreasons = (List<CustomListItemMobile>)UtilMobile.CallApiGetParams<List<CustomListItemMobile>>("/api/gym/cancelreasons", ps);
            Application.Current.Properties["cancelreasons"] = cancelreasons;
        }

        async private void RunWorkerCompletedReasons(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            Reasons.ItemsSource = (List<CustomListItemMobile>)Application.Current.Properties["cancelreasons"];
            Reasons.SelectedItem = null;
        }

        private void RunActionDates(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            string lastClass = Xamarin.Essentials.Preferences.Get("lastclass", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            ps.Add("enrollId", enrollId);
            ps.Add("lastClass", lastClass);
            ClassListView_ResultMobile classTemplate = (ClassListView_ResultMobile)UtilMobile.CallApiGetParams<ClassListView_ResultMobile>("/api/gym/readcancel", ps);
            Application.Current.Properties["classtemplate"] = classTemplate;
        }

        private bool settingDate = false;
        async private void RunWorkerCompletedDates(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            ClassListView_ResultMobile classTemplate = (ClassListView_ResultMobile)Application.Current.Properties["classtemplate"];
            Dates.ItemsSource = classTemplate.ClassDates;
            settingDate = true;
            string lastClass = Xamarin.Essentials.Preferences.Get("lastclass", "");
            foreach (CustomListItemMobile i in classTemplate.ClassDates)
            {
                if (i.Text == lastClass)
                {
                    Dates.SelectedItem = i;
                    break;
                }
            }
            if (Dates.SelectedItem == null)
            {
                Dates.SelectedItem = classTemplate.ClassDates[0];
            }
            CustomListItemMobile li = (CustomListItemMobile)Dates.SelectedItem;
            Xamarin.Essentials.Preferences.Set("lastclass", Convert.ToDateTime(li.Text).ToShortDateString());
            settingDate = false;
            paymentSummary.Text = UtilMobile.ConvertHtml(classTemplate.AgeDesc);

            cancelContent.IsVisible = true;
            activityIndicator.IsVisible = false;
        }

        private void Reasons_SelectionChanged(object sender, System.EventArgs e)
        {
            if (Reasons.SelectedItem != null)
            {
                string reason = ((CustomListItemMobile)Reasons.SelectedItem).Text;
                Xamarin.Essentials.Preferences.Set("cancelreason", reason);
            }
        }

        void Dates_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            if (settingDate)
            {
                return;
            }
            cancelContent.IsVisible = false;
            activityIndicator.IsVisible = true;

            string lastClass = ((CustomListItemMobile)((RadListPicker)sender).SelectedItem).Text;
            Xamarin.Essentials.Preferences.Set("lastclass", lastClass);

            BackgroundWorker b1 = new BackgroundWorker();
            b1.WorkerReportsProgress = true;
            b1.WorkerSupportsCancellation = true;
            b1.DoWork += RunActionDates;
            b1.RunWorkerCompleted += RunWorkerCompletedDates;
            b1.RunWorkerAsync(lastClass);
        }

        public async void Submit_Clicked(object sender, EventArgs e)
        {
            if (signatureView.IsBlank == true || Reasons.SelectedItem == null)
            {
                await DisplayAlert("Incomplete Information", "Please provide reason for cancelling and digital signature to continue", "Close");
            }
            else
            {
                var s = signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string sig = Convert.ToBase64String(b);
                Xamarin.Essentials.Preferences.Set("signature", "data:image/png;base64," + sig);
                Xamarin.Essentials.Preferences.Set("action", "cancelenroll");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//loading");
            }
        }

        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}