using mygymmobiledata;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountSchedule : ContentPage
    {
        public string DropInLabel = "";
        public string UnlimitedLabel = "";

        public AccountSchedule()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            DropInLabel = gym.DropInLabel;
            UnlimitedLabel = gym.UnlimitedLabel;
            if (gym.ClassCardEnabled == true)
            {
                purchaseClassCard.IsVisible = true;
            }

            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            listView.ItemsSource = account.Children;
            string returns = "";

            foreach (ChildMobile c in account.Children)
            {
                returns = Environment.NewLine;
                c.UnlimitedAvailable = $"*{gym.UnlimitedLabel} Available";
                if (c.ScheduleUnlimited)
                {
                    returns += Environment.NewLine;
                }
                c.MakeupsAvailable = returns + $"*{c.NumMakeupsAvailable} Makeups Available";
                if (c.ScheduleMakeup)
                {
                    returns += Environment.NewLine;
                }
                c.DropInAvailable = returns + $"*{gym.DropInLabel} Available";
                if (c.ScheduleDropIn)
                {
                    returns += Environment.NewLine;
                }
                c.VIMANotSignedMessage = returns + "*Sign Member Agreement";
                if (c.ScheduleUnlimited == false && c.ScheduleMakeup == false && c.ScheduleDropIn == false)
                {
                    c.ScheduleClasses = true;
                }
            }

            base.OnAppearing();
        }

        async private void Child_Tapped(object sender, System.EventArgs e)
        {
            TappedEventArgs ev = (TappedEventArgs)e;
            Xamarin.Essentials.Preferences.Set("childid", ev.Parameter.ToString());
            await Shell.Current.GoToAsync("//accountschedulechild");
        }

        async void PurchaseClassCard_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "readclasscardpackages");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}