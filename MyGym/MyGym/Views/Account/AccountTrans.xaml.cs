using mygymmobiledata;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountTrans : ContentPage
    {
        public AccountTrans()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void Payments_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "accounttranspayments");
            Shell.Current.GoToAsync("//loading");
        }

        void Billing_Tapped(System.Object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("//accounttransbilling");
        }
    }
}