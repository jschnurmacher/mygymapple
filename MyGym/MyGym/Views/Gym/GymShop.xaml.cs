using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
namespace MyGym
{
    public partial class GymShop : ContentPage
    {
        public GymShop()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Task s = OpenBrowser();
        }

        public async Task OpenBrowser()
        {
            await Browser.OpenAsync("https://mygymfun.com", BrowserLaunchMode.SystemPreferred);
            await Shell.Current.GoToAsync("//accounthome");
        }
    }
}

