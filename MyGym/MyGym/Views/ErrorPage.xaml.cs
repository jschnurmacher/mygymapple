using System;
using System.Collections.Generic;
using mygymmobiledata;

using Xamarin.Forms;

namespace MyGym
{
    public partial class ErrorPage : ContentPage
    {
        public ErrorPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                string error = Xamarin.Essentials.Preferences.Get("error", "");
                if (error.Contains("Please Update"))
                {
                    ErrorText.IsVisible = false;
                    HomeLink.IsVisible = false;
                    ExitLink.IsVisible = true;
                    MessageText.FontSize = 18;
                    Shell.SetNavBarIsVisible(this, false);
                    Shell.SetTabBarIsVisible(this, false);
                }
                else
                {
                    bool contains = Application.Current.Properties.ContainsKey("account");
                    if (contains == true)
                    {
                        AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                        if (account == null)
                        {
                            HomeLink.IsVisible = false;
                            Application.Current.Properties.Clear();
                            Xamarin.Essentials.Preferences.Set("gymid", "");
                            Xamarin.Essentials.Preferences.Set("accountid", "");
                            Xamarin.Essentials.Preferences.Set("zip", "");
                            Xamarin.Essentials.Preferences.Set("country", "");
                        }
                    }
                    else
                    {
                        HomeLink.IsVisible = false;
                        Application.Current.Properties.Clear();
                        Xamarin.Essentials.Preferences.Set("gymid", "");
                        Xamarin.Essentials.Preferences.Set("accountid", "");
                        Xamarin.Essentials.Preferences.Set("zip", "");
                        Xamarin.Essentials.Preferences.Set("country", "");
                    }
                    Xamarin.Essentials.Preferences.Set("error", "");
                    Xamarin.Essentials.Preferences.Set("action", "");
                }
                MessageText.Text = error;
            }
            catch { }
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        async void Contact_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//contact");
        }

        void Exit_Tapped(System.Object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
