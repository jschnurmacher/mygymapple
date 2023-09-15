using mygymmobiledata;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountParty : ContentPage
    {
        public AccountParty()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            upcomingParties.ItemsSource = account.Parties;
            upcomingParties.HeightRequest = account.Parties.Count * 160;
        }

        async void ScheduleParty_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "readparty");
            await Shell.Current.GoToAsync("//loading");
        }

        async void Invitation_Tapped(System.Object sender, System.EventArgs e)
        {
            string invitation = Convert.ToString(((TappedEventArgs)e).Parameter);
            await Clipboard.SetTextAsync(invitation);
            await DisplayAlert("Invitation Copied", "Invitation copied successfully", "Close");
        }

        async void EditAddOns_Tapped(System.Object sender, System.EventArgs e)
        {
            int partyId = Convert.ToInt32(((TappedEventArgs)e).Parameter);
            Xamarin.Essentials.Preferences.Set("partyid", partyId);
            await Shell.Current.Navigation.PushAsync(new PartyAddOnsEdit());
        }
    }
}