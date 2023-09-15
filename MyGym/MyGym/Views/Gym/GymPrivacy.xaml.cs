using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymPrivacy : ContentPage
    {
        public GymPrivacy()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionPrivacy;
            b.RunWorkerCompleted += RunWorkerCompletedPrivacy;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            base.OnAppearing();
        }

        private void RunActionPrivacy(object sender, DoWorkEventArgs e)
        {
            string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdReadGym", gymId.ToString());
            GymMobile gym = (GymMobile)UtilMobile.CallApiGetParams<GymMobile>($"/api/gym/readgym", ps);
            Application.Current.Properties["gym"] = gym;
        }

        async private void RunWorkerCompletedPrivacy(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            webView.Source = $"https://www.mygym.com/{gym.HomeUrl}/privacymobile";
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }
    }
}