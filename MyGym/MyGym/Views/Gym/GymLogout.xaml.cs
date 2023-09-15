using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymLogout : ContentPage
    {
        public GymLogout()
        {
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected async override void OnAppearing()
        {
            Application.Current.Properties.Clear();
            Xamarin.Essentials.Preferences.Set("gymid", "");
            Xamarin.Essentials.Preferences.Set("accountid", "");
            Xamarin.Essentials.Preferences.Set("zip", "");
            Xamarin.Essentials.Preferences.Set("country", "");
            await Shell.Current.GoToAsync("//gymlogin");
        }
    }
}