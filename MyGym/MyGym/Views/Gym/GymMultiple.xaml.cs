using mygymmobiledata;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymMultiple : ContentPage
    {
        public GymMultiple()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            List<AccountMobile> accounts = (List<AccountMobile>)Application.Current.Properties["accounts"];
            listView.IsVisible = true;
            selectAGym.IsVisible = true;
            listView.ItemsSource = accounts;
            base.OnAppearing();
        }

        async private void listView_SelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            Application.Current.Properties["account"] = null;
            AccountMobile g = (AccountMobile)((CollectionView)sender).SelectedItem;
            Application.Current.Properties["logingym"] = g.GymId;
            Xamarin.Essentials.Preferences.Set("action", "login");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}