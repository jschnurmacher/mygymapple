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
    public partial class PartyAddOnsEdit : ContentPage
    {
        public PartyAddOnsEdit()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
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
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int partyId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partyid", ""));
            string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", Convert.ToInt32(gymId));
            PartyMobile party = (PartyMobile)UtilMobile.CallApiGetParams<PartyMobile>($"/api/gym/readparty", ps);
            Application.Current.Properties["party"] = party;

            ps = new Dictionary<string, object>();
            ps.Add("partyId", Convert.ToInt32(partyId));
            int partyPackageId = Convert.ToInt32((string)UtilMobile.CallApiGetParams<string>($"/api/gym/readpartypackage", ps));
            Xamarin.Essentials.Preferences.Set("partypackageid", partyPackageId.ToString());

            Application.Current.Properties["selectedaddons"] = new List<int>();
            ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            ps.Add("partyId", partyId);
            PartyOptionsMobile s = (PartyOptionsMobile)UtilMobile.CallApiGetParams<PartyOptionsMobile>("/api/gym/readpartyaddonsedit", ps);
            foreach (PartyOptionMobile o in s.PartyOptions)
            {
                o.Display = string.Format(new CultureInfo(gym.Culture), "{0} - {1:c}{2}", o.Name, o.Retail, o.PerChild == true ? " (per child)" : "");
            }
            s.PartyOptions.Add(new PartyOptionMobile { Display = "I do not want any upgrades at this time", Id = 0 });
            Application.Current.Properties["partyaddons"] = s;
            Xamarin.Essentials.Preferences.Set("partynumkids", s.NumKids);
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
            int partyPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", 0));
            PartyMobile p = (PartyMobile)Application.Current.Properties["party"];
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
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
            PartyOptionsMobile s = (PartyOptionsMobile)Application.Current.Properties["partyaddons"];
            addOnList.ItemsSource = s.PartyOptions;
            addOnList.HeightRequest = s.PartyOptions.Count * 30;
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
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionAddOns;
                b.RunWorkerCompleted += RunWorkerCompletedAddOns;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }

        private void RunActionAddOns(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int partyId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partyid", ""));
            List<int> addons = (List<int>)Application.Current.Properties["selectedaddons"];
            string selectedAddOns = "";
            foreach (int r in addons)
            {
                if (string.IsNullOrEmpty(selectedAddOns) == false)
                {
                    selectedAddOns += ",";
                }
                selectedAddOns += r.ToString();
            }
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            ps.Add("accountId", accountId);
            ps.Add("partyId", partyId);
            ps.Add("selectedAddOns", selectedAddOns);
            ps.Add("numKids", Xamarin.Essentials.Preferences.Get("partynumkids", 0));
            AccountMobile a = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/partyaddonsupdate", ps);
            Application.Current.Properties["account"] = a;
        }

        private async void RunWorkerCompletedAddOns(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            await Shell.Current.Navigation.PopAsync();
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

        async void cancelButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        void totalKidsPicker_SelectionChanged(System.Object sender, System.EventArgs e)
        {
            RadListPicker p = (RadListPicker)sender;
            int numKids = Convert.ToInt32(p.SelectedItem);
            Xamarin.Essentials.Preferences.Set("partynumkids", numKids);
        }
    }
}
