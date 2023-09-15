using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartyConfirm : ContentPage
    {
        public PartyConfirm()
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
            List<ChildMobile> selectedChildren = (List<ChildMobile>)Application.Current.Properties["selectedchildren"];
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int partyPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", ""));
            PartyMobile p = (PartyMobile)Application.Current.Properties["party"];
            foreach (PartyPackageMobile pk in p.PartyPackages)
            {
                if (pk.Id == partyPackageId)
                {
                    PartyPackageText.Text = $"Package: {pk.Name}";
                    break;
                }
            }
            PartyTimeMobile pt = (PartyTimeMobile)Application.Current.Properties["partyselectedtime"];
            PartyTime.Text = string.Format(new CultureInfo(gym.Culture), "Date: {0:d} {0:h:mmtt}-{1:h:mmtt}", pt.Start, pt.End);
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            PartyChild.Text = "Children:\r\n";
            foreach (ChildMobile c in selectedChildren)
            {
                PartyChild.Text += c.First + "\r\n";
            }
            List<int> addOns = (List<int>)Application.Current.Properties["selectedaddons"];
            if (addOns.Contains(0))
            {
                PartyUpgrades.IsVisible = false;
            }
            else
            {
                PartyOptionsMobile s = (PartyOptionsMobile)Application.Current.Properties["partyaddons"];
                PartyUpgrades.Text = "Party Upgrades:\r\n";
                foreach (PartyOptionMobile po in s.PartyOptions)
                {
                    if (addOns.Contains(po.Id))
                    {
                        PartyUpgrades.Text += po.Name + "\r\n";
                    }
                }
            }
            ErrorMessage.IsVisible = false;
            ErrorMessageLink1.IsVisible = false;
            ErrorMessageLink2.IsVisible = false;
            if (account.ErrorMessage != null && string.IsNullOrEmpty(account.ErrorMessage) == false && account.ErrorMessage.Contains("credit card") == true)
            {
                ErrorMessage.IsVisible = true;
                ErrorMessage.Text = UtilMobile.ConvertHtml(account.ErrorMessage);
                ErrorMessageLink1.IsVisible = true;
            }
            else if (account.ErrorMessage != null && string.IsNullOrEmpty(account.ErrorMessage) == false && account.ErrorMessage.Contains("credit card") == false)
            {
                ErrorMessage.IsVisible = true;
                ErrorMessage.Text = UtilMobile.ConvertHtml(account.ErrorMessage);
                ErrorMessageLink2.IsVisible = true;
            }
            else
            {
                SuccessMessage.IsVisible = true;
                SuccessMessage.Text = "You are Confirmed!";
            }
            base.OnAppearing();
        }

        private async void EditPayment_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttransbilling");
        }

        private async void EditDate_Tapped(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "readparty");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}