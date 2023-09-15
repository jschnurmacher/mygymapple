using mygymmobiledata;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollCancelTrial : ContentPage
    {
        public EnrollCancelTrial()
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
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", Convert.ToInt32(gym.Id));
            List<CustomListItemMobile> cancelreasons = (List<CustomListItemMobile>)UtilMobile.CallApiGetParams<List<CustomListItemMobile>>("/api/gym/cancelreasons", ps);
            Application.Current.Properties["cancelreasons"] = cancelreasons;
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
            List<CustomListItemMobile> cancelreasons = (List<CustomListItemMobile>)Application.Current.Properties["cancelreasons"];
            Reasons.ItemsSource = cancelreasons;
            Reasons.SelectedItem = null;
        }

        public async void Submit_Clicked(object sender, EventArgs e)
        {
            if (Reasons.SelectedItem != null)
            {
                string reason = ((CustomListItemMobile)Reasons.SelectedItem).Text;
                Xamarin.Essentials.Preferences.Set("cancelreason", reason);
                Xamarin.Essentials.Preferences.Set("action", "canceltrial");
                await Shell.Current.Navigation.PopAsync();
                await Shell.Current.GoToAsync("//loading");
            }
            else
            {
                await DisplayAlert("Please Select a Reason", "Please select a reason why you are cancelling.", "Close");
            }
        }

        public async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}