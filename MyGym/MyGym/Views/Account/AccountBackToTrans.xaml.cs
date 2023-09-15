using System;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountBackToTrans : Grid
    {
        public AccountBackToTrans()
        {
            InitializeComponent();
        }

        private async void BackToAccountTrans_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttrans");
        }
    }
}
