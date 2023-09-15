using System;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartyBackToHome : Grid
    {
        public PartyBackToHome()
        {
            InitializeComponent();
        }

        async private void BackToPartyHome_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accountparty");
        }
    }
}
