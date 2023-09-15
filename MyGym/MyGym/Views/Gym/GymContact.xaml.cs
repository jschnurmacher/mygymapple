using System;
using System.Collections.Generic;
using mygymmobiledata;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyGym
{
    public partial class GymContact : ContentPage
    {
        public GymContact()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            waitImage.Source = $"wait{new Random().Next(17)}.webp";
            Comments.Text = "";
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            contactTitle.Text = "Contact " + gym.Name;
            gymName.Text = gym.Name;
            gymAddress.Text = gym.Address1 + " " + gym.Address2;
            gymCityStateZip.Text = $"{gym.City}, {gym.State} {gym.Zip}";
            gymEmail.IsVisible = false;
            gymSMSText.IsVisible = false;
            if (string.IsNullOrEmpty(gym.Email) == false)
            {
                gymEmail.Text = gym.Email;
                gymEmail.IsVisible = true;
            }
            if (string.IsNullOrEmpty(gym.SMSText.Replace("_", "")) == false)
            {
                gymSMSText.Text = gym.SMSText;
                gymSMSText.IsVisible = true;
            }
        }

        async void Comments_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties["comments"] = Comments.Text;
            Xamarin.Essentials.Preferences.Set("action", "contact");
            await Shell.Current.GoToAsync("//loading");
        }

        async void gymEmail_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                var message = new EmailMessage
                {
                    Subject = "My Gym Inquiry",
                    Body = "",
                    To = new List<string> { gym.Email }
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                await DisplayAlert("Email not supported", "Email is not supported on this device." + fbsEx.Message, "Close");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Email not supported", "Email is not supported on this device. " + ex.Message, "Close");
            }
        }

        async void gymSMSText_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                var message = new SmsMessage("", new[] { gym.SMSText });
                await Sms.ComposeAsync(message);
            }
            catch
            {
                await DisplayAlert("SMS not supported", "SMS is not supported on this device", "Close");
            }
        }
    }
}
