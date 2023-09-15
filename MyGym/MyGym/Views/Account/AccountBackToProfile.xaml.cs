using System;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountBackToProfile : Grid
    {
        public AccountBackToProfile()
        {
            InitializeComponent();
        }

        private async void BackToAccountProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofile");
        }
    }
}
