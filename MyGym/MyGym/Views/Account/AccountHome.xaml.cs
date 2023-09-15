using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Essentials.Permissions;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountHome : ContentPage
    {
        public AccountHome()
        {
            InitializeComponent();
        }

        async protected override void OnAppearing()
        {
            await Shell.Current.Navigation.PopToRootAsync();
            Application.Current.Properties["promocode"] = "";
            string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
            string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
            if (gymId == "0" || gymId == "" || gymId == "null")
            {
                await Shell.Current.GoToAsync("//gymlogin");
            }
            else if (accountId == "0" || accountId == "" || accountId == "null")
            {
                await Shell.Current.GoToAsync("//gymlogin");
            }
            else
            {
                mainLayout.IsVisible = true;
                reminders.IsVisible = false;
                completeEmergencyInformation.IsVisible = false;
                completeVIMA.IsVisible = false;
                AccountMobile account = null;
                string socksNeededStr = "";
                if (Application.Current.Properties.ContainsKey("account"))
                {
                    account = (AccountMobile)Application.Current.Properties["account"];
                    if (account == null)
                    {
                        Xamarin.Essentials.Preferences.Set("action", "init");
                        await Shell.Current.GoToAsync("//loading");
                        return;
                    }
                    AccountEmailLabel.Text = "Home";
                    AccountEmailLabel.Text += $" - {account.Email}";
                    reminders.IsVisible = false;
                    completeWaiver.IsVisible = false;
                    completeVIMA.IsVisible = false;
                    completeEmergencyInformation.IsVisible = false;
                    if (account.ContactFirst == "" || account.ContactLast == "" || account.Phone3 == "")
                    {
                        reminders.IsVisible = true;
                        completeEmergencyInformation.IsVisible = true;
                    }
                    if (string.IsNullOrEmpty(account.SignatureWaiver) && string.IsNullOrEmpty(account.SignatureWaivera))
                    {
                        reminders.IsVisible = true;
                        completeWaiver.IsVisible = true;
                    }
                    foreach (ChildMobile ch in account.Children)
                    {
                        foreach (EnrollMobile m in ch.Enrolls)
                        {
                            if (m.Type == "Enrollment" || m.Type == "Trial")
                            {
                                if (string.IsNullOrEmpty(m.SignatureVIMA) && string.IsNullOrEmpty(m.SignatureVIMAa))
                                {
                                    reminders.IsVisible = true;
                                    completeVIMA.IsVisible = true;
                                    break;
                                }
                            }
                        }
                        if (Application.Current.Properties["gym"] != null)
                        {
                            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                            if (ch.IncludeSocks == true && gym.ShowSocksAlert)
                            {
                                socksNeeded.IsVisible = true;
                                reminders.IsVisible = true;
                                if (socksNeededStr != "")
                                {
                                    socksNeededStr += $", ";
                                }
                                else if (socksNeededStr == "")
                                {
                                    socksNeededStr = $"My Gym Socks needed for: ";
                                }
                                socksNeededStr += ch.First;
                            }
                        }
                    }
                    if (socksNeededStr != "")
                    {
                        socksNeededText.Text = socksNeededStr;
                    }
                    int noShowAlertDismiss = 0;
                    if (Application.Current.Properties.ContainsKey("noshowalertdismiss"))
                    {
                        noShowAlertDismiss = Convert.ToInt32(Application.Current.Properties["noshowalertdismiss"]);
                    }
                    if (Application.Current.Properties["gym"] != null)
                    {
                        GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                        if (gym.NoShowAlert == true && noShowAlertDismiss == 0)
                        {
                            NoShowAlerts.ItemsSource = account.NoShowAlerts;
                            NoShowAlerts.HeightRequest = account.NoShowAlerts.Count * 75;
                        }
                        else
                        {
                            NoShowAlerts.ItemsSource = new ObservableCollection<NoShowAlertMobile>();
                            NoShowAlerts.HeightRequest = 0;
                        }
                    }
                }
            }
            base.OnAppearing();
        }
        async private void UpcomingVisits_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountupcomingvisits");
        }

        async private void ScheduleClasses_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountschedule");
        }

        async private void CampsEvents_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountevent");
        }

        async private void Party_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountparty");
        }

        async private void Profile_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofile");
        }

        async private void Shop_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//gymshop");
        }

        async private void Payments_Tapped(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttrans");
        }

        async void CompleteEmergencyInformation_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofileedit");
        }

        async void CompleteVIMA_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountschedule");
        }

        async void CompleteWaiver_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofileprofile");
        }

        void DismissNoShowAlert_Tapped(System.Object sender, System.EventArgs e)
        {
            string desc = Convert.ToString(((TappedEventArgs)e).Parameter);
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int index = 0;
            foreach (NoShowAlertMobile a in account.NoShowAlerts)
            {
                if (a.Desc == desc)
                {
                    account.NoShowAlerts.RemoveAt(index);
                    break;
                }
                index++;
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.NoShowAlert == true && account.NoShowAlerts.Count > 0)
            {
                NoShowAlerts.ItemsSource = account.NoShowAlerts;
                NoShowAlerts.HeightRequest = account.NoShowAlerts.Count * 75;
                NoShowAlerts.IsVisible = true;
            }
            else
            {
                NoShowAlerts.ItemsSource = new ObservableCollection<NoShowAlertMobile>();
                NoShowAlerts.HeightRequest = 0;
                NoShowAlerts.IsVisible = false;
            }
        }

        async void SocksNeeded_Tapped(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountsocks");
        }
    }
}