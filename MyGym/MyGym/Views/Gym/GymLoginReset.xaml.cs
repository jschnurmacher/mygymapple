using mygymmobiledata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymLoginReset : ContentPage
    {
        public GymLoginReset()
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
            ResetPasswordLabel.IsVisible = true;
            EmailEntry.IsVisible = true;
            ResetPasswordButton.IsVisible = true;
            EmailNotVerifiedLabel.IsVisible = false;
            EmailResetSentLabel.IsVisible = false;
            CodeEntry.IsVisible = false;
            EnterCodeButton.IsVisible = false;
            ResetCodeAgainButton.IsVisible = false;
            CodeEntryNotVerified.IsVisible = false;
            base.OnAppearing();
        }

        public void ResetPassword_Clicked(object sender, System.EventArgs e)
        {
            ResetPasswordLabel.IsVisible = true;
            EmailEntry.IsVisible = true;
            ResetPasswordButton.IsVisible = true;
            EmailNotVerifiedLabel.IsVisible = false;
            EmailResetSentLabel.IsVisible = false;
            CodeEntry.IsVisible = false;
            EnterCodeButton.IsVisible = false;
            ResetCodeAgainButton.IsVisible = false;
            CodeEntryNotVerified.IsVisible = false;
            Xamarin.Essentials.Preferences.Set("action", "resetpassword");
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        public void EnterCode_Clicked(object sender, System.EventArgs e)
        {
            ResetPasswordLabel.IsVisible = false;
            EmailEntry.IsVisible = false;
            ResetPasswordButton.IsVisible = false;
            EmailNotVerifiedLabel.IsVisible = false;
            EmailResetSentLabel.IsVisible = false;
            CodeEntry.IsVisible = false;
            EnterCodeButton.IsVisible = false;
            ResetCodeAgainButton.IsVisible = false;
            CodeEntryNotVerified.IsVisible = false;

            Xamarin.Essentials.Preferences.Set("action", "resetpasswordcode");
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        public void ResetCodeAgain_Clicked(object sender, System.EventArgs e)
        {
            ResetPasswordLabel.IsVisible = true;
            EmailEntry.IsVisible = true;
            ResetPasswordButton.IsVisible = true;
            EmailNotVerifiedLabel.IsVisible = false;
            EmailResetSentLabel.IsVisible = false;
            CodeEntry.IsVisible = false;
            EnterCodeButton.IsVisible = false;
            ResetCodeAgainButton.IsVisible = false;
            CodeEntryNotVerified.IsVisible = false;
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "resetpassword")
            {
                int gymId = 0;
                if (Xamarin.Essentials.Preferences.ContainsKey("gym"))
                {
                    gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gym", ""));
                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("gymId", gymId);
                ps.Add("username", EmailEntry.Text);
                string accountIdStr = (string)UtilMobile.CallApiGetParams<string>($"/api/gym/resetpassword", ps);
                if (accountIdStr != null)
                {
                    int accountId = Convert.ToInt32(accountIdStr.Replace("\"", ""));
                    if (accountId > 0)
                    {
                        Xamarin.Essentials.Preferences.Set("accountid", accountId.ToString());
                    }
                }
            }
            else if (action == "resetpasswordcode")
            {
                GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("gymId", gym.Id);
                ps.Add("code", CodeEntry.Text);
                string accountIdStr = (string)UtilMobile.CallApiGetParams<string>($"/api/gym/resetpasswordcode", ps);
                if (accountIdStr != null)
                {
                    int accountId = Convert.ToInt32(accountIdStr.Replace("\"", ""));
                    if (accountId > 0)
                    {
                        Xamarin.Essentials.Preferences.Set("accountid", accountId.ToString());
                    }
                }
            }
            Thread.Sleep(500);
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
            string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
            if (action == "resetpassword")
            {
                if (accountId != "")
                {
                    ResetPasswordLabel.IsVisible = false;
                    EmailEntry.IsVisible = false;
                    ResetPasswordButton.IsVisible = false;
                    EmailResetSentLabel.IsVisible = true;
                    CodeEntry.IsVisible = true;
                    EnterCodeButton.IsVisible = true;
                }
                else
                {
                    EmailNotVerifiedLabel.IsVisible = true;
                }
            }
            if (action == "resetpasswordcode")
            {
                if (accountId != "")
                {
                    await Shell.Current.GoToAsync("//gymloginresetpassword");
                }
                else
                {
                    EmailResetSentLabel.IsVisible = true;
                    CodeEntry.IsVisible = true;
                    EnterCodeButton.IsVisible = true;
                    ResetCodeAgainButton.IsVisible = true;
                    CodeEntryNotVerified.IsVisible = true;
                }
            }
        }
    }
}