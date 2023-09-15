using System;
using System.Globalization;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartyPackage : ContentPage
    {
        public PartyPackage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            int partyPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", ""));
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.BirthdayEnabled == false)
            {
                bookButton.IsVisible = false;
                contactButton.IsVisible = true;
            }
            if (gym.BirthdayDeposit > 0)
            {
                partyDeposit.Text = $"A non-refundable deposit of {gym.BirthdayDeposit:c} is required for booking";
            }
            else
            {
                partyDeposit.IsVisible = false;
            }
            PartyMobile p = (PartyMobile)Application.Current.Properties["party"];
            foreach (PartyPackageMobile pk in p.PartyPackages)
            {
                if (pk.Id == partyPackageId)
                {
                    partyPackageTitle.Text = pk.Name;
                    pk.BirthdayPackageCaptionsHeight = 0;
                    foreach (PartyCaptionMobile c in pk.BirthdayPackageCaptions)
                    {
                        c.BirthdayCaptionItemsHeight = 0;
                        foreach (PartyItemMobile i in c.BirthdayCaptionItems)
                        {
                            i.Item = "● " + i.Item.Replace("rn", " ").Replace("\r\n", " ");
                            c.BirthdayCaptionItemsHeight += (Convert.ToInt32(Math.Ceiling(i.Item.Length / 40.0M) * 20.0M));
                        }
                        pk.BirthdayPackageCaptionsHeight += (Convert.ToInt32(Math.Ceiling(c.Caption.Length / 40.0M) * 20.0M)) + (c.BirthdayCaptionItemsHeight);
                    }
                    Cost.Text = pk.Member == pk.NonMember ? string.Format(new CultureInfo(gym.Culture), "{0:c}", pk.Member) : string.Format(new CultureInfo(gym.Culture), "{0:c} (nonmembers: {1:c})", pk.Member, pk.NonMember);
                    bookButton.Text = $"Book {pk.Name}";
                    partyCaptions.ItemsSource = pk.BirthdayPackageCaptions;
                    partyCaptions.HeightRequest = pk.BirthdayPackageCaptionsHeight;
                    break;
                }
            }
            Xamarin.Essentials.Preferences.Set("membership", "");
        }

        async void BookParty_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new PartyDate());
        }

        async void ContactParty_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//gymcontact");
        }
    }
}
