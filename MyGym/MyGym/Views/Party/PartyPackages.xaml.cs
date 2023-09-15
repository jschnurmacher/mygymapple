using System;
using System.Collections.Generic;
using System.Globalization;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartyPackages : ContentPage
    {
        public PartyPackages()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Application.Current.Properties["selectedchildren"] = new List<ChildMobile>();
            Application.Current.Properties["selectedaddons"] = new List<int>();
            Xamarin.Essentials.Preferences.Set("partynumkids", 0);
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.BirthdayDeposit > 0)
            {
                partyDeposit.Text = string.Format(new CultureInfo(gym.Culture), "A non-refundable deposit of {0:c} is required for booking", gym.BirthdayDeposit);
            }
            else
            {
                partyDeposit.IsVisible = false;
            }
            PartyMobile p = (PartyMobile)Application.Current.Properties["party"];
            foreach (PartyPackageMobile pk in p.PartyPackages)
            {
                pk.Cost = pk.Member == pk.NonMember ? string.Format(new CultureInfo(gym.Culture), "{0:c}", pk.Member) : string.Format(new CultureInfo(gym.Culture), "{0:c} (nonmembers: {1:c})", pk.Member, pk.NonMember);
            }
            listView.ItemsSource = p.PartyPackages;
            Xamarin.Essentials.Preferences.Set("membership", "");
        }

        async void BookParty_Clicked(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("partypackageid", Convert.ToString(((Button)sender).CommandParameter));
            await Shell.Current.GoToAsync("//partypackage");
        }
    }
}
