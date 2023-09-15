using System;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountBackToHome : Grid
    {
        public AccountBackToHome()
        {
            InitializeComponent();
        }

        private async void BackToAccountHome_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }
    }
}
