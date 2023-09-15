using mygymmobiledata;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollFreezeConfirm : ContentPage
    {
        public EnrollFreezeConfirm()
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
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            if (account.ErrorMessage != null && account.ErrorMessage != "")
            {
                ErrorMessage.IsVisible = true;
                ErrorMessage.Text = account.ErrorMessage;
                ErrorMessageLink.IsVisible = true;
                SuccessMessage.IsVisible = false;
            }
            else
            {
                SuccessMessage.IsVisible = true;
                SuccessMessage.Text = "Your Freeze is Confirmed!";
                ErrorMessage.IsVisible = false;
                ErrorMessageLink.IsVisible = false;
            }
            EnrollTitle.Text = "Freeze Confirmation";
            base.OnAppearing();
        }

        private async void EditPayment_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttransbilling");
        }
    }
}