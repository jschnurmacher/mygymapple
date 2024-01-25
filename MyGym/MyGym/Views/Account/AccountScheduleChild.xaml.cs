using mygymmobiledata;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountScheduleChild : ContentPage
    {
        public AccountScheduleChild()
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
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.ClassCardEnabled == true)
            {
                purchaseClassCard.IsVisible = true;
            }

            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    ActionsStr.Text = $"Actions for {c.First}";
                    ScheduleGuest.IsVisible = c.ScheduleGuest;
                    ScheduleEnroll.IsVisible = c.ScheduleEnroll;
                    ScheduleTrial.IsVisible = c.ScheduleTrial;
                    ScheduleMakeup.IsVisible = c.ScheduleMakeup;
                    ScheduleUnlimited.IsVisible = c.ScheduleUnlimited;
                    ScheduleDropIn.IsVisible = c.ScheduleDropIn;
                    OverduePayment.IsVisible = account.Overdue;
                    ScheduleClassCard.IsVisible = account.ClassCards.Count > 0;
                    ScheduleDropInText.Text = $"Schedule {gym.DropInLabel}";
                    ScheduleUnlimitedText.Text = $"Schedule {gym.UnlimitedLabel}";
                    DropInMax.IsVisible = false;
                    UnlimitedMax.IsVisible = false;
                    if (c.DropInMax == true)
                    {
                        DropInMax.IsVisible = true;
                        DropInMax.Text = $"You've reached your limit for {gym.DropInLabel} bookings. The link will reappear after you attend one of your scheduled classes.";
                    }
                    if (c.UnlimitedMax == true && (gym.UnlimitedMode == true || c.AllowUnlimited))
                    {
                        UnlimitedMax.IsVisible = true;
                    }
                    ObservableCollection<EnrollMobile> enrolls = new ObservableCollection<EnrollMobile>();
                    ObservableCollection<EnrollMobile> trials = new ObservableCollection<EnrollMobile>();
                    foreach (EnrollMobile e in c.Enrolls)
                    {
                        e.UnlimitedEnrollmentText = "*" + gym.UnlimitedLabel + " Enrollment";
                        e.UpgradeUnlimitedText = "Upgrade to " + gym.UnlimitedLabel;
                        e.RemoveUnlimitedText = "Remove " + gym.UnlimitedLabel + " Upgrade";
                        if (e.Type == "Enrollment")
                        {
                            enrolls.Add(e);
                            e.Overdue = account.Overdue;
                            e.SwitchClass = false;
                            if (gym.MoveEnabled && e.EnrollMoveToId == 0)
                            {
                                e.SwitchClass = true;
                            }
                            e.CancelEnrollment = false;
                            if (account.Overdue == false && e.LastClass == "" && gym.CancelEnabled)
                            {
                                e.CancelEnrollment = true;
                            }
                            if ((account.Overdue == true && e.FreezeEnrollment == true && gym.FreezeEnabled) || e.FreezeDeny == true)
                            {
                                e.FreezeEnrollment = false;
                            }
                            e.FirstClassVisible = e.FirstClass != "" && e.Start.Date <= e.EndReal.Date;
                            e.LastClassVisible = e.LastClass != "";
                            if (e.AbsencesExists && e.Absences.StartsWith("Marked") == false)
                            {
                                e.Absences = "Marked Absent On: " + e.Absences;
                            }
                            if (account.Overdue)
                            {
                                e.SwitchClass = false;
                            }
                            if (e.FreezeDeny)
                            {
                                FreezeDeny.IsVisible = true;
                                FreezeDeny.Text = c.First + " may not freeze within " + gym.MinimumFreezeMonths + " month(s) of their last freeze";
                            }
                            e.UpgradeUnlimited = false;
                            e.RemoveUnlimited = false;
                            if (e.Unlimited == false)
                            {
                                e.UpgradeUnlimited = true;
                                if (e.Overdue && gym.OfferUnlimitedOnline == true && gym.UnlimitedEnroll == true)
                                {
                                    UpgradeDeny.IsVisible = true;
                                    e.UpgradeUnlimited = false;
                                }
                                else if (gym.OfferUnlimitedOnline == false || gym.UnlimitedEnroll == false || e.Overdue == true)
                                {
                                    e.UpgradeUnlimited = false;
                                }
                            }
                            else if (gym.UnlimitedMode == false && gym.OfferUnlimitedOnline == true)
                            {
                                e.RemoveUnlimited = true;
                            }
                        }
                        else if (e.Type == "Trial")
                        {
                            trials.Add(e);
                            e.UpgradeUnlimited = false;
                            e.RemoveUnlimited = false;
                            if (e.Unlimited == false)
                            {
                                e.UpgradeUnlimited = true;
                                if (e.Overdue && gym.OfferUnlimitedOnline == true && gym.UnlimitedEnroll == true)
                                {
                                    UpgradeDeny.IsVisible = true;
                                    e.UpgradeUnlimited = false;
                                }
                                else if (gym.OfferUnlimitedOnline == false || gym.UnlimitedEnroll == false || e.Overdue == true)
                                {
                                    e.UpgradeUnlimited = false;
                                }
                            }
                            else if (gym.UnlimitedMode == false && gym.OfferUnlimitedOnline == true)
                            {
                                e.RemoveUnlimited = true;
                            }
                        }
                        e.FreezeEnrollmentStr = e.FreezeEnrollmentStr.Replace("rn", "\r\n");
                    }
                    EnrolledInStr.Text = $"{c.First} is currently enrolled in:";
                    EnrolledInStr.IsVisible = enrolls.Count > 0;
                    EnrolledInStr.IsEnabled = enrolls.Count > 0;
                    Enrollments.IsVisible = enrolls.Count > 0;
                    Enrollments.IsEnabled = enrolls.Count > 0;
                    if (enrolls.Count > 0)
                    {
                        Enrollments.ItemsSource = enrolls;
                        Enrollments.HeightRequest = enrolls.Count * 290;
                    }

                    TrialStr.Text = $"Trial enrollments for {c.First}:";
                    TrialStr.IsVisible = trials.Count > 0;
                    TrialStr.IsEnabled = trials.Count > 0;
                    Trials.IsVisible = trials.Count > 0;
                    Trials.IsEnabled = trials.Count > 0;
                    Trials.HeightRequest = 0;
                    if (trials.Count > 0)
                    {
                        Trials.ItemsSource = trials;
                        Trials.HeightRequest = trials.Count * 290;
                    }

                    break;
                }
            }
            List<AccountCampCard> cards = new List<AccountCampCard>();
            foreach (AccountCampCard c in account.ClassCards)
            {
                AccountCampCard nc = new AccountCampCard();
                UtilMobile.CopyPropertyValues(c, nc);
                nc.SessionsStr = $"Sessions: {nc.Sessions}";
                nc.EndStr = $"Expires: {nc.EndStr}";
                cards.Add(nc);
            }
            ClassCards.ItemsSource = cards;
            base.OnAppearing();
        }

        /*
        StatusGuest = 1,
        StatusEnroll = 2,
        StatusMakeup = 3,
        StatusUnlimited = 4,
        StatusClassCard = 5,
        StatusDropIn = 6,
        StatusTrial = 7
         */
        async private void ScheduleEnroll_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "2");
            await Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        async private void ScheduleGuest_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "1");
            await Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        private void ScheduleTrial_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "7");
            Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        private void ScheduleUnlimited_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "4");
            Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        private void ScheduleMakeup_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "3");
            Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        private void ScheduleDropIn_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "6");
            Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        private void ScheduleClassCard_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("accountstatus", "5");
            Shell.Current.Navigation.PushAsync(new GymClasses());
        }

        private void ViewVIMA_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("enrollid", ((TappedEventArgs)e).Parameter.ToString());
            Shell.Current.Navigation.PushAsync(new AccountVIMA());
        }

        private void SignVIMA_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("enrollid", ((TappedEventArgs)e).Parameter.ToString());
            Shell.Current.Navigation.PushAsync(new AccountVIMASign());
        }

        private void CancelTrial_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("enrollid", ((TappedEventArgs)e).Parameter.ToString());
            Shell.Current.Navigation.PushAsync(new EnrollCancelTrial());
        }

        private void CancelEnrollment_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("enrollid", ((TappedEventArgs)e).Parameter.ToString());
            Shell.Current.Navigation.PushAsync(new EnrollCancel());
        }

        private void FreezeEnrollment_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("enrollid", ((TappedEventArgs)e).Parameter.ToString());
            Shell.Current.Navigation.PushAsync(new EnrollFreeze());
        }

        private void SwitchClass_Tapped(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("enrollid", ((TappedEventArgs)e).Parameter.ToString());
            Shell.Current.Navigation.PushAsync(new EnrollSwitch());
        }

        async private void UpgradeUnlimited_Tapped(object sender, System.EventArgs e)
        {
            int enrollId = Convert.ToInt32(((TappedEventArgs)e).Parameter);
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            bool trial = false;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    foreach (EnrollMobile enroll in c.Enrolls)
                    {
                        if (enroll.EnrollId == enrollId)
                        {
                            if (enroll.Type == "Trial")
                            {
                                trial = true;
                                break;
                            }
                        }
                    }
                }
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            string s = "";
            if (trial == true)
            {
                s = string.Format(new CultureInfo(gym.Culture), "Upgrade your enrollment to {0} for {1:c} per billing cycle. (you will be charged after your trial period ends)", gym.UnlimitedLabel, gym.UnlimitedFee);
            }
            else
            {
                s = string.Format(new CultureInfo(gym.Culture), "Upgrade your enrollment to {0} for {1:c} per billing cycle. (you will be charged an initial prorated fee for the remaining days left in your billing cycle)", gym.UnlimitedLabel, gym.UnlimitedFee);
            }
            var res = await DisplayAlert("Upgrade Enrollment to Unlimited", s, "OK", "Cancel");
            if (res == true)
            {
                Xamarin.Essentials.Preferences.Set("enrollid", enrollId);
                Xamarin.Essentials.Preferences.Set("action", "upgradeunlimited");
                await Shell.Current.GoToAsync("//loading");
            }
        }

        async private void RemoveUnlimited_Tapped(object sender, System.EventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            var res = await DisplayAlert($"Remove Unlimited Enrollment", $"Remove the {gym.UnlimitedLabel} option from this enrollment", "OK", "Cancel");
            if (res == true)
            {
                int enrollId = Convert.ToInt32(((TappedEventArgs)e).Parameter);
                Xamarin.Essentials.Preferences.Set("enrollid", enrollId);
                Xamarin.Essentials.Preferences.Set("action", "degradeunlimited");
                await Shell.Current.GoToAsync("//loading");
            }

        }

        private async void MarkAbsent_Tapped(object sender, System.EventArgs e)
        {
            TappedEventArgs ev = (TappedEventArgs)e;
            Xamarin.Essentials.Preferences.Set("enrollid", ev.Parameter.ToString());
            Xamarin.Essentials.Preferences.Set("source", "accountschedulechild");
            await PopupNavigation.Instance.PushAsync(new AccountMarkAbsentPopup());
        }

        private async void UnMarkAbsent_Tapped(object sender, System.EventArgs e)
        {
            TappedEventArgs ev = (TappedEventArgs)e;
            Xamarin.Essentials.Preferences.Set("enrollid", ev.Parameter.ToString());
            Xamarin.Essentials.Preferences.Set("source", "accountschedulechild");
            await PopupNavigation.Instance.PushAsync(new AccountMarkUnAbsentPopup());
        }

        private async void BackToAccountSchedule_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountschedule");
        }

        async void PurchaseClassCard_Tapped(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("action", "readclasscardpackages");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}