using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Forms9Patch;
using Xamarin.Essentials;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountVIMA : ContentPage
    {
        public AccountVIMA()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            activityIndicator.IsVisible = true;

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
            string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
            string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", Convert.ToInt32(gymId));
            ps.Add("enrollId", Convert.ToInt32(enrollId));
            string s = UtilMobile.CallApiGetParamsString("/api/gym/vima", ps);
            s = s.Replace(">rn", ">");
            s = s.Replace("rn ", "");
            s = s.Replace("rnrn", "");
            s = s.Replace("</script>rn", "</script>");
            Application.Current.Properties["vima"] = s;
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
            string vima = (string)Application.Current.Properties["vima"];
            VIMA.Html = $"" +
                $"<html>" +
                $"<header><meta name='viewport' content='width=device-width, initial-scale=0.4, maximum-scale=0.4, minimum-scale=0.4, user-scalable=no'></header>" +
                $"<div>{vima}</div>" +
                $"</html>";
            activityIndicator.IsVisible = false;
        }

        async void Email_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                var message = new EmailMessage
                {
                    Subject = "My Gym Membership Agreement",
                    Body = VIMA.Html,
                    BodyFormat = EmailBodyFormat.Html,
                    To = new List<string> { account.Email }
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
    }
}
