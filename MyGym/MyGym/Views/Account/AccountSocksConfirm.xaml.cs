using mygymmobiledata;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountSocksConfirm : ContentPage
    {
        public AccountSocksConfirm()
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
            string reset = Xamarin.Essentials.Preferences.Get("reset", "");
            if (reset == "1")
            {
                base.OnAppearing();
                return;
            }
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            if (account.ErrorMessage != null && account.ErrorMessage != "")
            {
                ErrorMessage.IsVisible = true;
                ErrorMessage.Text = "There was an error and your purchase did not complete successfully. " + account.ErrorMessage;
                ErrorMessageLink.IsVisible = true;
                SuccessMessage.IsVisible = false;
            }
            else
            {
                SuccessMessage.IsVisible = true;
                SuccessMessage.Text = "Your My Gym Socks Purchase is Confirmed!";
                ErrorMessage.IsVisible = false;
                ErrorMessageLink.IsVisible = false;
            }
            base.OnAppearing();
        }

        private async void EditPayment_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountsocksbilling");
        }
    }
}