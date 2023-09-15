using mygymmobiledata;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventConfirm : ContentPage
    {
        public EventConfirm()
        {
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
            EventName.Text = ev.Display;
            SelectedSessions.Text = "";
            if (ev.SelectedDates != null && ev.SelectedDates.Count > 0)
            {
                foreach (EventDateMobile d in ev.SelectedDates)
                {
                    SelectedSessions.Text += string.Format("{0}\r\n", d.SessionStr);
                }
            }
            else
            {
                SelectedSessions.Text = ev.EventDiscountMobile.DisplayList;
            }
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            if (account.ErrorMessage != null && account.ErrorMessage != "")
            {
                ErrorMessage.IsVisible = true;
                ErrorMessage.Text = account.ErrorMessage;
            }
            else
            {
                SuccessMessage.IsVisible = true;
                SuccessMessage.Text = "You are Confirmed!";
            }
            base.OnAppearing();
        }

        private async void EditPayment_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
            await Shell.Current.Navigation.PopAsync();
            await Shell.Current.Navigation.PopAsync();
        }
    }
}