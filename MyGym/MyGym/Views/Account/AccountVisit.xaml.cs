using mygymmobiledata;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountVisit : ContentPage
    {
        public AccountVisit()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            List<EnrollMobile> enrolls = new List<EnrollMobile>();
            foreach (ChildMobile c in account.Children)
            {
                foreach (EnrollMobile e in c.Enrolls)
                {
                    if (e.NextClass != "")
                    {
                        enrolls.Add(e);
                    }
                }
            }
            foreach (EnrollMobile m in enrolls)
            {
                m.ScheduleClasses = m.Type != "Party" && m.Type != "Camp/Event";
            }
            enrolls.Sort((x, y) => x.Start.CompareTo(y.Start));
            listView.ItemsSource = enrolls;
        }

        private async void BackToAccountHome_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
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

        private async void MarkAbsent_Tapped(object sender, System.EventArgs e)
        {
            TappedEventArgs ev = (TappedEventArgs)e;
            Xamarin.Essentials.Preferences.Set("enrollid", ev.Parameter.ToString());
            Xamarin.Essentials.Preferences.Set("source", "accountupcomingvisits");
            await PopupNavigation.Instance.PushAsync(new AccountMarkAbsentPopup());
        }

        private async void UnMarkAbsent_Tapped(object sender, System.EventArgs e)
        {
            TappedEventArgs ev = (TappedEventArgs)e;
            Xamarin.Essentials.Preferences.Set("enrollid", ev.Parameter.ToString());
            Xamarin.Essentials.Preferences.Set("source", "accountupcomingvisits");
            await PopupNavigation.Instance.PushAsync(new AccountMarkUnAbsentPopup());
        }

        private void ScheduleClasses_Tapped(object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("//accountschedule");
        }

        async void EditAddOns_Tapped(System.Object sender, System.EventArgs e)
        {
            int partyId = Convert.ToInt32(((TappedEventArgs)e).Parameter);
            Xamarin.Essentials.Preferences.Set("partyid", partyId);
            await Shell.Current.Navigation.PushAsync(new PartyAddOnsEdit());
        }

        async void Invitation_Tapped(System.Object sender, System.EventArgs e)
        {
            string invitation = Convert.ToString(((TappedEventArgs)e).Parameter);
            await Clipboard.SetTextAsync(invitation);
            await DisplayAlert("Invitation Copied", "Invitation copied successfully", "Close");
        }
    }
}