using System;
using mygymmobiledata;
using Telerik.Barcode;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace MyGym
{
    public partial class AccountProfileProfile : ContentPage
    {
        public AccountProfileProfile()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            Email.Text = account.Email.ToLower();
            Name.Text = $"{account.First} {account.Last}";
            Address.Text = account.Address + " " + (account.Apt != "" ? account.Apt : "");
            CityStateZip.Text = $"{account.City}, {account.State} {account.Zip}";
            Phone.Text = account.Phone1;
            signLiabilityGrid.IsVisible = true;
            if (string.IsNullOrEmpty(account.SignatureWaiver) == false && string.IsNullOrEmpty(account.SignatureWaivera) == false && string.IsNullOrEmpty(account.SignatureWaiverb) == false)
            {
                signLiabilityGrid.IsVisible = false;
            }
            viewLiabilityGrid.IsVisible = false;
            if (string.IsNullOrEmpty(account.SignatureWaiver) == false || string.IsNullOrEmpty(account.SignatureWaivera) == false || string.IsNullOrEmpty(account.SignatureWaiverb) == false)
            {
                viewLiabilityGrid.IsVisible = true;
            }
            base.OnAppearing();
        }

        private async void BackToProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofile");
        }

        private async void EditProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofileedit");
        }

        private async void SignLiability_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AccountWaiverSign());
        }

        private async void ViewLiability_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AccountWaiver());
        }

        private async void DeleteAccount_Tapped(System.Object sender, System.EventArgs e)
        {
            var answer = await DisplayAlert("Delete Account", $"Deleting your account will remove all data associated with your account.  Any scheduled classes, enrollments or parties pending will remain unless you contact the gym to remove them. You will no longer be able to log in. Are you sure?", "Yes", "No");
            if (answer)
            {
                Xamarin.Essentials.Preferences.Set("action", "deleteaccount");
                await Shell.Current.GoToAsync("//loading");
            }
        }
    }
}
