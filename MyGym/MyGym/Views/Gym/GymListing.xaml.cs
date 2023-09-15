using mygymmobiledata;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymListing : ContentPage
    {
        public GymListing()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            GymsMobile gymsMobile = null;
            if (Application.Current.Properties.ContainsKey("gyms"))
            {
                gymsMobile = (GymsMobile)Application.Current.Properties["gyms"];
            }
            listView.IsVisible = true;
            selectAGym.IsVisible = true;
            noGymsFound.IsVisible = true;
            findAGym.IsVisible = true;
            if (gymsMobile == null || gymsMobile.gyms == null || gymsMobile.gyms.Count == 0)
            {
                listView.ItemsSource = new ObservableCollection<GymMobile>();
                listView.IsVisible = false;
                selectAGym.IsVisible = false;
            }
            else
            {
                listView.ItemsSource = gymsMobile.gyms;
                noGymsFound.IsVisible = false;
                findAGym.IsVisible = false;
            }
            base.OnAppearing();
        }

        private async void FindAGymButton_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.Properties["gym"] = null;
            Xamarin.Essentials.Preferences.Set("gymid", "");
            await Shell.Current.GoToAsync("//findagym");
        }

        private void listView_SelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            Application.Current.Properties["account"] = null;
            GymMobile g = (GymMobile)((CollectionView)sender).SelectedItem;
            Application.Current.Properties["gym"] = g;
            Xamarin.Essentials.Preferences.Set("gymid", g.Id.ToString());
            Shell.Current.GoToAsync("//gymregister");
        }
    }
}