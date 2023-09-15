using mygymmobiledata;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountProfileChildren : ContentPage
    {
        public string DropInLabel = "";
        public string UnlimitedLabel = "";

        public AccountProfileChildren()
        {
            InitializeComponent();
        }

        private async void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            listView.ItemsSource = account.Children;
            base.OnAppearing();
        }

        private async void BackToProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofile");
        }

        private async void Child_Tapped(object sender, System.EventArgs e)
        {
            TappedEventArgs ev = (TappedEventArgs)e;
            Xamarin.Essentials.Preferences.Set("childid", ev.Parameter.ToString());
            await Shell.Current.GoToAsync("//accountprofileeditchild");
        }

        private async void AddChild_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("childid", "0");
            await Shell.Current.GoToAsync("//accountprofileeditchild");
        }
    }
}