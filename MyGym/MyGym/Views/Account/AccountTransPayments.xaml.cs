using System;
using System.Collections.Generic;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountTransPayments : ContentPage
    {
        public AccountTransPayments()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            listView.ItemsSource = (List<TransactionMobile>)Application.Current.Properties["transactions"];
            base.OnAppearing();
        }

        private async void BackToProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttrans");
        }

        private async void EditProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttransedit");
        }
    }
}
