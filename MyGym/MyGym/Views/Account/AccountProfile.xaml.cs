using mygymmobiledata;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountProfile : ContentPage
    {
        public AccountProfile()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void Profile_Tapped(System.Object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("//accountprofileprofile");
        }

        void Marketing_Tapped(System.Object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("//accountprofilemarketing");
        }

        void Children_Tapped(System.Object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("//accountprofilechildren");
        }
    }
}