using System.Collections.Generic;
using System.Collections.ObjectModel;
using mygymmobiledata;
using Telerik.XamarinForms.Input;
using Xamarin.Forms;

namespace MyGym
{
    public partial class GymCamps : ContentPage
    {
        public GymCamps()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            string action = Xamarin.Essentials.Preferences.Get("actionaux", "");
            if (Application.Current.Properties.ContainsKey("camp") && action != "enrollcamp")
            {
                EventMobile e = (EventMobile)Application.Current.Properties["camp"];
                e.SelectedDates = new ObservableCollection<EventDateMobile>();
            }
            if (action == "enrollcamp")
            {
                Xamarin.Essentials.Preferences.Set("actionaux", "");
            }
            List<EventMobile> camps = new List<EventMobile>();
            List<EventMobile> evs = (List<EventMobile>)Application.Current.Properties["camps"];
            string campevent = Xamarin.Essentials.Preferences.Get("campevent", "");
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            bool hasMember = false;
            bool hasEnrolled = false;
            foreach (ChildMobile c in account.Children)
            {
                if (c.Member == true)
                {
                    hasMember = true;
                }
                foreach (EnrollMobile e in c.Enrolls)
                {
                    if (e.Type == "Enrollment")
                    {
                        hasEnrolled = true;
                    }
                }
            }
            foreach (EventMobile em in evs)
            {
                if ((campevent == "camp" && em.IncludeSchedOnly == false) || (campevent == "event" && em.IncludeSchedOnly == true))
                {
                    camps.Add(em);
                }
                em.Book = $"Book {em.Display}";
                em.BookVisible = em.LCF == false && em.AvailableBookings == true;
                em.BookingNotAvailable = false;
                em.CampEnrollmentNotAvailable = false;
                em.Bullets = "\r\n";
                foreach (EventItemMobile i in em.EventItems)
                {
                    em.Bullets += "* " + i.Item + "\r\n\r\n";
                }
                if (gym.EnrollCampEnabled == true)
                {
                    if (em.EnrolledOnly == true && hasEnrolled == false)
                    {
                        em.BookVisible = false;
                        em.BookingNotAvailable = true;
                    }
                    if (em.MembersOnly == true && hasMember == false)
                    {
                        em.BookVisible = false;
                        em.BookingNotAvailable = true;
                    }
                    em.EventDiscountsStr = em.EventDiscountsStr.Replace("(", "\r\n(");
                }
                else
                {
                    em.BookVisible = false;
                    em.CampEnrollmentNotAvailable = true;
                }
            }
            campTitle.Text = campevent == "camp" ? "Camps" : "Events";
            listView.ItemsSource = camps;
            Xamarin.Essentials.Preferences.Set("membership", "0");
        }

        async void BookCamp_Clicked(System.Object sender, System.EventArgs e)
        {
            string camp = ((Button)sender).CommandParameter.ToString();
            List<EventMobile> evs = (List<EventMobile>)Application.Current.Properties["camps"];
            bool packages = false;
            foreach (EventMobile ev in evs)
            {
                if (ev.Display == camp)
                {
                    Application.Current.Properties["camp"] = ev;
                    ev.SelectedDates = new ObservableCollection<EventDateMobile>();
                    if (ev.ClassTemplateId1 == 1)
                    {
                        packages = true;
                    }
                    break;
                }
                ev.Book = $"Book {ev.Display}";
                ev.SelectedDates = new ObservableCollection<EventDateMobile>();
                ev.StoreCredit = 0;
            }
            if (packages == false)
            {
                EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
                ev.EventDiscountMobile = null;
                await Shell.Current.Navigation.PushAsync(new EventSelect());
            }
            else
            {
                StackLayout p = (StackLayout)((Button)sender).Parent;
                bool sel = true;
                EventDiscountMobile selectedPackage = null;
                foreach (Element t in p.Children)
                {
                    if (t.GetType().ToString().Contains("RadListPicker"))
                    {
                        if (((RadListPicker)t).SelectedItem == null)
                        {
                            sel = false;
                        }
                        else
                        {
                            selectedPackage = (EventDiscountMobile)((RadListPicker)t).SelectedItem;
                        }
                    }
                }
                EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
                if (sel == true)
                {
                    ev.EventDiscountMobile = selectedPackage;
                    await Shell.Current.Navigation.PushAsync(new EventDetail());
                }
                else
                {
                    ev.EventDiscountMobile = null;
                    await DisplayAlert("No Package Selected", "Please select a package to continue", "Close");
                }
            }
        }
    }
}
