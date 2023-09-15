using mygymmobiledata;
using System;
using System.ComponentModel;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymRegister : ContentPage
    {
        public GymRegister()
        {
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected async override void OnAppearing()
        {
            string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
            if (gymId == "" || gymId == "null")
            {
                await Shell.Current.GoToAsync("//findagym");
            }
            else
            {
                activityIndicator.IsEnabled = false;
                activityIndicator.IsVisible = false;
                activityIndicator.IsRunning = false;

                Email.Text = Xamarin.Essentials.Preferences.Get("email", "");
                InputMissing.IsVisible = false;
                InvalidEmail.IsVisible = false;
                EmailExists.IsVisible = false;
                LoginPageButton.IsVisible = false;
                base.OnAppearing();
            }
        }

        private void RegisterButton_Clicked(object sender, System.EventArgs e)
        {
            activityIndicator.IsEnabled = true;
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
            submitButton.IsVisible = false;
            InputMissing.IsVisible = false;
            InvalidEmail.IsVisible = false;
            EmailExists.IsVisible = false;
            LoginPageButton.IsVisible = false;
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void LoginPageButton_Clicked(object sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync("//gymlogin");
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            string parentFirst = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(ParentFirst.Text));
            string parentLast = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(ParentLast.Text));
            string email = Email.Text.ToLower();
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            bool valid = true;
            if (parentFirst == "" || parentLast == "" || email == "")
            {
                Xamarin.Essentials.Preferences.Set("result", "inputmissing");
                valid = false;
            }
            if (UtilMobile.IsValidEmail(email) == false)
            {
                Xamarin.Essentials.Preferences.Set("result", "invalidemail");
                valid = false;
            }
            else if (UtilMobile.EmailExists(gym.Id, email) == true)
            {
                Xamarin.Essentials.Preferences.Set("result", "emailexists");
                valid = false;
            }
            if (valid)
            {
                Xamarin.Essentials.Preferences.Set("result", "");
                AccountMobile a = new AccountMobile();
                a.First = parentFirst;
                a.Last = parentLast;
                a.Email = email;
                Application.Current.Properties["registeraccount"] = a;
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
            activityIndicator.IsEnabled = false;
            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
            submitButton.IsVisible = true;

            string result = Xamarin.Essentials.Preferences.Get("result", "");
            if (result == "")
            {
                await Shell.Current.GoToAsync("//gymregister2");
            }
            else if (result == "inputmissing")
            {
                InputMissing.IsVisible = true;
            }
            else if (result == "invalidemail")
            {
                InvalidEmail.IsVisible = true;
            }
            else if (result == "emailexists")
            {
                EmailExists.IsVisible = true;
                LoginPageButton.IsVisible = true;
            }
        }
    }
}