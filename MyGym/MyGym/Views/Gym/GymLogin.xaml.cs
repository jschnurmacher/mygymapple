using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymLogin : ContentPage
    {
        public GymLogin()
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
            Application.Current.Properties.Clear();
            Xamarin.Essentials.Preferences.Set("gymid", "");
            Xamarin.Essentials.Preferences.Set("accountid", "");
            Xamarin.Essentials.Preferences.Set("zip", "");
            Xamarin.Essentials.Preferences.Set("country", "");

            LoginNotVerified.IsVisible = false;
            PasswordEntry.Text = "";
            string error = Xamarin.Essentials.Preferences.Get("error", "");
            if (error == "LoginNotVerified")
            {
                LoginNotVerified.IsVisible = true;
                Xamarin.Essentials.Preferences.Set("error", "");
            }
            base.OnAppearing();
        }

        async public void LoginButton_Clicked(object sender, System.EventArgs e)
        {
            LoginNotVerified.IsVisible = false;
            Xamarin.Essentials.Preferences.Set("email", EmailEntry.Text);
            Xamarin.Essentials.Preferences.Set("password", PasswordEntry.Text);
            Xamarin.Essentials.Preferences.Set("action", "login");
            await Shell.Current.GoToAsync("//loading");
        }

        async private void ForgotPassword_Clicked(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("email", EmailEntry.Text);
            await Shell.Current.GoToAsync("//gymloginreset");
        }

        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("email", EmailEntry.Text);
            await Shell.Current.GoToAsync("//findagym");
        }
    }
}