using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using mygymmobiledata;
using Telerik.XamarinForms.Input;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartyAddOns : ContentPage
    {
        public PartyAddOns()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Xamarin.Essentials.Preferences.Set("partyhalfhour", "");
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            PartyTimeMobile t = (PartyTimeMobile)Application.Current.Properties["partyselectedtime"];
            if (t.HalfHour == true)
            {
                halfHourGrid.IsVisible = true;
                halfHourDisplay.Text = string.Format(new CultureInfo(gym.Culture), "Add an extra 1/2 hour to your party for {0:c}", t.HalfHourFee);
            }
            int partyPackageId = Xamarin.Essentials.Preferences.Get("partypackageid", 0);
            PartyMobile p = (PartyMobile)Application.Current.Properties["party"];
            if (gym.ShowBirthdayNumKids == true)
            {
                foreach (PartyPackageMobile pk in p.PartyPackages)
                {
                    if (pk.Id == partyPackageId)
                    {
                        partyNumKids.Text = $"Your party package includes {pk.Max} kids with an extra cost of {pk.Extra:c} per additional kid.";
                        ObservableCollection<int> includeNumbers = new ObservableCollection<int>();
                        for (int i = 0; i != gym.MaxBirthdayKids; i++)
                        {
                            includeNumbers.Add(i + 1);
                        }
                        totalKidsPicker.ItemsSource = includeNumbers;
                        int numKids = Xamarin.Essentials.Preferences.Get("partynumkids", 0);
                        if (numKids == 0)
                        {
                            numKids = pk.Max;
                            Xamarin.Essentials.Preferences.Set("partynumkids", pk.Max);
                        }
                        totalKidsPicker.SelectedItem = numKids;
                        break;
                    }
                }
            }
            else
            {
                partyNumKids.IsVisible = false;
                totalKidsLabel.IsVisible = false;
                totalKidsPicker.IsVisible = false;
            }

            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            base.OnAppearing();
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            clearingChecks = true;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            ps.Add("birthdayPackageId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", "")));
            PartyOptionsMobile s = (PartyOptionsMobile)UtilMobile.CallApiGetParams<PartyOptionsMobile>("/api/gym/readpartyaddons", ps);
            foreach (PartyOptionMobile p in s.PartyOptions)
            {
                p.Display = string.Format(new CultureInfo(gym.Culture), "{0} - {1:c}{2}", p.Name, p.Retail, p.PerChild == true ? " (per child)" : "");
            }
            s.PartyOptions.Add(new PartyOptionMobile { Display = "I do not want any upgrades at this time", Id = 0 });
            Application.Current.Properties["partyaddons"] = s;
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
            PartyOptionsMobile s = (PartyOptionsMobile)Application.Current.Properties["partyaddons"];
            addOnList.ItemsSource = s.PartyOptions;
            addOnList.HeightRequest = s.PartyOptions.Count * 30;
            clearingChecks = false;
        }

        public bool clearingChecks = false;
        void AddOn_IsCheckedChanged(System.Object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            if (clearingChecks)
            {
                return;
            }
            RadCheckBox b = (RadCheckBox)sender;
            int addOnId = Convert.ToInt32(b.ClassId);
            PartyOptionsMobile s = (PartyOptionsMobile)Application.Current.Properties["partyaddons"];
            if (addOnId == 0)
            {
                Application.Current.Properties["selectedaddons"] = new List<int>();
                clearingChecks = true;
                foreach (PartyOptionMobile p in s.PartyOptions)
                {
                    if (p.Id != 0)
                    {
                        p.Checked = false;
                    }
                }
                clearingChecks = false;
            }
            else
            {
                ((List<int>)Application.Current.Properties["selectedaddons"]).Remove(0);
                clearingChecks = true;
                foreach (PartyOptionMobile p in s.PartyOptions)
                {
                    if (p.Id == 0)
                    {
                        p.Checked = false;
                    }
                }
                clearingChecks = false;
            }
            if (Convert.ToBoolean(b.IsChecked))
            {
                ((List<int>)Application.Current.Properties["selectedaddons"]).Add(addOnId);
            }
            else
            {
                ((List<int>)Application.Current.Properties["selectedaddons"]).Remove(addOnId);
            }
        }

        async void continueButton_Clicked(System.Object sender, System.EventArgs e)
        {
            List<int> t = (List<int>)Application.Current.Properties["selectedaddons"];
            if (t.Count == 0)
            {
                await DisplayAlert("No Selection", "Select at least one option below. If you do not want any upgrades please select the last option.", "Close");
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new PartyBilling());
            }
        }

        void HalfHour_IsCheckedChanged(System.Object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            RadCheckBox b = (RadCheckBox)sender;
            if (Convert.ToBoolean(b.IsChecked))
            {
                Xamarin.Essentials.Preferences.Set("partyhalfhour", "1");
            }
            else
            {
                Xamarin.Essentials.Preferences.Set("partyhalfhour", "");
            }
        }

        void totalKidsPicker_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            RadListPicker p = (RadListPicker)sender;
            int numKids = Convert.ToInt32(p.SelectedItem);
            Xamarin.Essentials.Preferences.Set("partynumkids", numKids);
        }
    }
}
