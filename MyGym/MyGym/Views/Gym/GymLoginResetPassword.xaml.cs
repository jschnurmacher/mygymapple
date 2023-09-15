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
    public partial class GymLoginResetPassword : ContentPage
    {
        BackgroundWorker b;

        public GymLoginResetPassword()
        {
            InitializeComponent();
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            ResetPasswordButton.IsVisible = true;
            InputMissing.IsVisible = false;
            PasswordMismatch.IsVisible = false;
            base.OnAppearing();
        }

        private void ResetPassword_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;
            PasswordMismatch.IsVisible = false;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            string password = Password.Text;
            string passwordVerify = PasswordVerify.Text;
            Xamarin.Essentials.Preferences.Set("result", "");
            if (password == "" || passwordVerify == "")
            {
                Xamarin.Essentials.Preferences.Set("result", "inputmissing");
            }
            else if (password != passwordVerify)
            {
                Xamarin.Essentials.Preferences.Set("result", "passwordmismatch");
            }
            else
            {
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("accountId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "")));
                ps.Add("password", Password.Text);
                AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/resetpasswordconfirm", ps);
                Application.Current.Properties["account"] = account;
            }
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
            string result = Xamarin.Essentials.Preferences.Get("result", "");
            if (result == "")
            {
                ResetPasswordButton.IsVisible = false;
                await DisplayAlert("Password Reset", "Your password has successfully been reset", "Close");
                await Shell.Current.GoToAsync("//gymlogin");
            }
            else if (result == "inputmissing")
            {
                InputMissing.IsVisible = true;
            }
            else if (result == "passwordmismatch")
            {
                PasswordMismatch.IsVisible = true;
            }
        }
    }
}