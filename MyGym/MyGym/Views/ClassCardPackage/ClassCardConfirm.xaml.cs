using mygymmobiledata;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClassCardConfirm : ContentPage
    {
        public ClassCardConfirm()
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
                SuccessMessage.Text = "Class Card Successfully Purchased!";
                ErrorMessage.IsVisible = false;
                ErrorMessageLink.IsVisible = false;
            }
            base.OnAppearing();
        }

        private async void EditPayment_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new ClassCardDetail());
            await Shell.Current.Navigation.PushAsync(new ClassCardBilling());
        }
    }
}