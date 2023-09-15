using System;
using System.Collections.Generic;
using System.ComponentModel;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountProfileMarketing : ContentPage
    {
        private bool init = false;
        public AccountProfileMarketing()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            init = true;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            marketingSwitch.IsToggled = account.Marketing;
            init = false;
            base.OnAppearing();
        }

        private async void BackToProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofile");
        }

        void marketingSwitch_Toggled(System.Object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (init == true)
            {
                return;
            }
            Xamarin.Essentials.Preferences.Set("marketing", ((Switch)sender).IsToggled == true ? "1" : "0");
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("accountId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "")));
            ps.Add("marketing", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("marketing", "")));
            UtilMobile.CallApiGetParamsString("/api/gym/marketingtoggle", ps);
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

            string marketing = Xamarin.Essentials.Preferences.Get("marketing", "");
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            account.Marketing = marketing == "1" ? true : false;
        }
    }
}
