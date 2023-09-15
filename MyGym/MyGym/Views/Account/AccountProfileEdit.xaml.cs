using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Barcode;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Essentials.Permissions;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountProfileEdit : ContentPage
    {
        BackgroundWorker b;

        public AccountProfileEdit()
        {
            InitializeComponent();
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
        }

        protected override void OnAppearing()
        {
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            State.IsVisible = false;
            EmergencyRelationship.IsVisible = false;
            List<CustomListItemMobile> l = new List<CustomListItemMobile>();
            l.Add(new CustomListItemMobile { Text = "Cell", Value = "3" });
            l.Add(new CustomListItemMobile { Text = "Home", Value = "1" });
            l.Add(new CustomListItemMobile { Text = "Work", Value = "2" });
            PhoneType.ItemsSource = l;
            EmergencyPhoneType.ItemsSource = l;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            Email.Text = account.Email;
            First.Text = account.First;
            Last.Text = account.Last;
            Address.Text = account.Address;
            Apt.Text = account.Apt;
            City.Text = account.City;
            Zip.Text = account.Zip;
            Zip.MaxLength = 10;
            Phone.Text = account.Phone1;
            PhoneType.SelectedItem = ((List<CustomListItemMobile>)PhoneType.ItemsSource).Find(m => m.Value == account.Phone1Type.ToString());
            PhoneExt.Text = account.Phone1Ext;
            EmergencyFirst.Text = account.ContactFirst;
            EmergencyLast.Text = account.ContactLast;
            EmergencyPhone.Text = account.Phone3;
            EmergencyPhoneType.SelectedItem = ((List<CustomListItemMobile>)EmergencyPhoneType.ItemsSource).Find(m => m.Value == account.Phone3Type.ToString());
            EmergencyPhoneExt.Text = account.Phone3Ext;
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Application.Current.Properties["states"] = UtilMobile.GetStates(gym.Country == "Canada", true);
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            Application.Current.Properties["relationships"] = (List<CustomListItemMobile>)UtilMobile.CallApiGetParams<List<CustomListItemMobile>>($"/api/gym/relationships", ps);
        }

        async private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }

            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            State.ItemsSource = (List<CustomListItemMobile>)Application.Current.Properties["states"];
            State.SelectedItem = ((List<CustomListItemMobile>)State.ItemsSource).Find(m => m.Value == account.State);
            State.IsVisible = true;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.Country == "Canada")
            {
                State.Placeholder = "-Province-";
                Zip.Placeholder = "-Postal-";
            }
            else
            {
                State.Placeholder = "-State-";
                Zip.Placeholder = "-Zip-";
            }

            EmergencyRelationship.ItemsSource = (List<CustomListItemMobile>)Application.Current.Properties["relationships"];
            EmergencyRelationship.IsVisible = true;
            EmergencyRelationship.SelectedItem = ((List<CustomListItemMobile>)EmergencyRelationship.ItemsSource).Find(m => m.Value == account.ContactRelationshipId.ToString());
        }

        async private void Submit_Clicked(object sender, System.EventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            if (string.IsNullOrEmpty(Email.Text)
                || string.IsNullOrEmpty(First.Text)
                || string.IsNullOrEmpty(Last.Text)
                || string.IsNullOrEmpty(Address.Text)
                || string.IsNullOrEmpty(City.Text)
                || State.SelectedItem == null
                || string.IsNullOrEmpty(Zip.Text)
                || string.IsNullOrEmpty(Phone.Text)
                || PhoneType.SelectedItem == null
                || string.IsNullOrEmpty(EmergencyFirst.Text)
                || string.IsNullOrEmpty(EmergencyLast.Text)
                || string.IsNullOrEmpty(EmergencyPhone.Text)
                || EmergencyPhoneType.SelectedItem == null)
            {
                await DisplayAlert("Required Information", "Please complete all required information", "Close");
            }
            else if (UtilMobile.ValidEmail(Email.Text) == false)
            {
                await DisplayAlert("Invalid Email", "Invalid email entered", "Close");
            }
            else if (UtilMobile.EmailExistsEdit(gym.Id, Email.Text, account.AccountId))
            {
                await DisplayAlert("Email Exists", "The email you entered already exists. Please enter another email.", "Close");
            }
            else
            {
                AccountMobile a = new AccountMobile();
                a.AccountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                a.GymId = gym.Id;
                a.Email = Email.Text;
                a.First = First.Text;
                a.Last = Last.Text;
                a.Address = Address.Text;
                a.Apt = Apt.Text;
                a.City = City.Text;
                a.State = ((CustomListItemMobile)State.SelectedItem).Value;
                a.Zip = Zip.Text;
                a.Phone1 = Phone.Text;
                a.Phone1Type = Convert.ToInt32(((CustomListItemMobile)PhoneType.SelectedItem).Value);
                a.Phone1Ext = PhoneExt.Text;
                a.ContactFirst = EmergencyFirst.Text;
                a.ContactLast = EmergencyLast.Text;
                a.ContactRelationshipId = Convert.ToInt32(((CustomListItemMobile)EmergencyRelationship.SelectedItem).Value);
                a.Phone3 = EmergencyPhone.Text;
                a.Phone3Type = Convert.ToInt32(((CustomListItemMobile)EmergencyPhoneType.SelectedItem).Value);
                a.Phone3Ext = EmergencyPhoneExt.Text;
                Application.Current.Properties["updateaccount"] = a;
                Xamarin.Essentials.Preferences.Set("action", "accountupdate");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//loading");
            }
        }

        private async void BackToProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofileprofile");
        }

        private async void Cancel_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofileprofile");
        }
    }
}
