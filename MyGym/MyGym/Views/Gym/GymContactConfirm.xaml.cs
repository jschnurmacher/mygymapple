using System;
using System.Collections.Generic;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class GymContactConfirm : ContentPage
    {
        public GymContactConfirm()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            contactTitle.Text = "Contact " + gym.Name;
            thankYou.Text = $"Thank you for contacting {gym.Name}.  Our representatives will be contacting you shortly.";
        }

        async void BackTopHome_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounthome");
        }
    }
}
