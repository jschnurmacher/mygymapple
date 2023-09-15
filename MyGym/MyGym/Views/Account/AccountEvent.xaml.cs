using mygymmobiledata;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountEvent : ContentPage
    {
        public AccountEvent()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            string reset = Xamarin.Essentials.Preferences.Get("reset", "");
            if (reset == "1")
            {
                base.OnAppearing();
                return;
            }
            base.OnAppearing();
            AccountMobile account = (AccountMobile)App.Current.Properties["account"];
            campCards.ItemsSource = account.CampCards;
            campCards.HeightRequest = account.CampCards.Count * 160;

            List<EnrollMobile> enrolls = new List<EnrollMobile>();
            foreach (ChildMobile c in account.Children)
            {
                foreach (EnrollMobile m in c.Enrolls)
                {
                    if (m.Type == "Camp/Event")
                    {
                        enrolls.Add(m);
                    }
                }
            }
            enrolls.Sort((x, y) => x.Start.CompareTo(y.Start));
            campVisits.ItemsSource = enrolls;
            campVisits.HeightRequest = enrolls.Count * 160;
        }

        async void ScheduleCamps_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "readcamps");
            Xamarin.Essentials.Preferences.Set("campevent", "camp");
            await Shell.Current.GoToAsync("//loading");
        }

        async void ScheduleEvents_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "readcamps");
            Xamarin.Essentials.Preferences.Set("campevent", "event");
            await Shell.Current.GoToAsync("//loading");
        }

        async void ScheduleEventsCard_Tapped(System.Object sender, System.EventArgs e)
        {
            int classTemplateId = Convert.ToInt32(((TappedEventArgs)e).Parameter);
            if (Application.Current.Properties.ContainsKey("camps") == false)
            {
                UtilMobile.ReadCamps();
            }
            List<EventMobile> evs = (List<EventMobile>)Application.Current.Properties["camps"];
            foreach (EventMobile ev in evs)
            {
                if (ev.EventId == classTemplateId)
                {
                    Application.Current.Properties["camp"] = ev;
                    break;
                }
            }
            Application.Current.Properties["selectedpackage"] = null;
            await Shell.Current.Navigation.PushAsync(new EventSelect());
        }

        private async void ViewLiability_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AccountWaiver());
        }

        private async void CancelClass_Tapped(object sender, System.EventArgs e)
        {
            var answer = await DisplayAlert("Cancel Visit", $"Cancel visit. Are you sure?", "Yes", "No");
            if (answer)
            {
                TappedEventArgs ev = (TappedEventArgs)e;
                Xamarin.Essentials.Preferences.Set("enrollid", ev.Parameter.ToString());
                Xamarin.Essentials.Preferences.Set("action", "cancelclass");
                await Shell.Current.GoToAsync("//loading");
            }
        }
    }
}