using mygymmobiledata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace MyGym
{
    public partial class FindAGym : ContentPage
    {
        public FindAGym()
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
            ZipCodeEntry.Text = "";
            if (Application.Current.Properties.ContainsKey("countries") == false)
            {
                BackgroundWorker b;
                b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionLoadCountries;
                b.RunWorkerCompleted += RunWorkerCompletedLoadCountries;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
            else
            {
                Countries.ItemsSource = (List<CustomListItemMobile>)Application.Current.Properties["countries"];
                Countries.SelectedItem = null;
            }
        }

        private void RunActionLoadCountries(object sender, DoWorkEventArgs e)
        {
            Application.Current.Properties["countries"] = (List<CustomListItemMobile>)UtilMobile.CallApiGet<List<CustomListItemMobile>>($"/api/gym/countries");
        }

        async private void RunWorkerCompletedLoadCountries(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            Countries.ItemsSource = (List<CustomListItemMobile>)Application.Current.Properties["countries"];
            Countries.SelectedItem = null;
        }

        async public void SearchZip_Clicked(object sender, System.EventArgs e)
        {
            string zip = ZipCodeEntry.Text;
            Xamarin.Essentials.Preferences.Set("zip", zip);
            Application.Current.Properties["gyms"] = null;
            Xamarin.Essentials.Preferences.Set("action", "loadgyms");
            await Shell.Current.GoToAsync("//loading");
        }

        async private void Countries_SelectionChanged(object sender, System.EventArgs e)
        {
            if (Countries.SelectedItem != null)
            {
                string country = ((CustomListItemMobile)Countries.SelectedItem).Text;
                Xamarin.Essentials.Preferences.Set("country", country);
                Application.Current.Properties["gyms"] = null;
                Xamarin.Essentials.Preferences.Set("action", "loadgymscountries");
                await Shell.Current.GoToAsync("//loading");
            }
        }
    }
}
