using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using mygymmobiledata;
using System.Collections.Generic;
using System;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;
using System.ComponentModel;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountMarkUnAbsentPopup : PopupPage
    {
        public AccountMarkUnAbsentPopup()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionAbsent;
            b.RunWorkerCompleted += RunWorkerCompletedAbsent;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            base.OnAppearing();
        }

        private void RunActionAbsent(object sender, DoWorkEventArgs e)
        {
            base.OnAppearing();
            string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("enrollId", Convert.ToInt32(enrollId));
            ps.Add("markUnmark", 1);
            List<CustomListItemMobile> items = (List<CustomListItemMobile>)UtilMobile.CallApiGetParams<List<CustomListItemMobile>>("/api/gym/absentdates", ps);
            Application.Current.Properties["absentdates"] = items;
        }

        async private void RunWorkerCompletedAbsent(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }

            List<CustomListItemMobile> items = (List<CustomListItemMobile>)Application.Current.Properties["absentdates"];
            Dates.ItemsSource = items;
            if (items.Count == 0)
            {
                Dates.IsVisible = false;
                DatesNotAvailable.IsVisible = true;
            }
            Dates.SelectedItem = null;
        }

        private void Dates_SelectionChanged(object sender, System.EventArgs e)
        {
            if (Dates.SelectedItem != null)
            {
                string date = ((CustomListItemMobile)Dates.SelectedItem).Text;
                Xamarin.Essentials.Preferences.Set("absentdate", date);
                Xamarin.Essentials.Preferences.Set("action", "unmarkabsent");
                this.Navigation.PopPopupAsync().ConfigureAwait(false);
                Shell.Current.GoToAsync("//loading");
            }
        }

        public void Cancel_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopPopupAsync().ConfigureAwait(false);
        }
    }
}