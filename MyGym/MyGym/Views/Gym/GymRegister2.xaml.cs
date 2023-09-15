using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GymRegister2 : ContentPage
    {
        public GymRegister2()
        {
            InitializeComponent();
            ChildDOB.MaximumDate = DateTime.Now;
            ChildDOB.MinimumDate = DateTime.Now.AddYears(-18);
            ChildDOB.DefaultHighlightedDate = DateTime.Now.AddYears(-5);
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
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

        async private void RegisterButton_Clicked(object sender, System.EventArgs e)
        {
            string childFirst = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(ChildFirst.Text));
            string childLast = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(ChildLast.Text));
            string password = Password.Text;
            string passwordVerify = PasswordVerify.Text;
            if (childFirst == "" || childLast == "" || ChildDOB.Date == null || privavyCheckbox.IsChecked == false || HowHeard.SelectedItem == null || password == "" || passwordVerify == "")
            {
                await DisplayAlert("Missing Input", "Please enter all values and agree to privacy policy", "Close");
            }
            else if (password != passwordVerify)
            {
                await DisplayAlert("Password Mismatch", "Password and confirmation do not match", "Close");
            }
            else
            {
                AccountMobile a = (AccountMobile)Application.Current.Properties["registeraccount"];
                a.Children = new System.Collections.ObjectModel.ObservableCollection<ChildMobile>();
                ChildMobile c = new ChildMobile();
                c.First = childFirst;
                c.Last = childLast;
                c.DOB = Convert.ToDateTime(ChildDOB.Date);
                a.Children.Add(c);
                a.Password = password;
                Xamarin.Essentials.Preferences.Set("action", "registerconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//loading");
            }
        }

        private async void AgreePrivacyButton_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new GymPrivacy());
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gym.Id);
            HowHeard.ItemsSource = (List<HowHeardMobile>)UtilMobile.CallApiGetParams<List<HowHeardMobile>>($"/api/gym/howheard", ps);
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
        }
    }
}