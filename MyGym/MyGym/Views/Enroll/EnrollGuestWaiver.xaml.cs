using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollGuestWaiver : ContentPage
    {
        public EnrollGuestWaiver()
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
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];

            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionWaiver;
            b.RunWorkerCompleted += RunWorkerCompletedWaiver;
            b.RunWorkerAsync(new DoWorkEventArgs(null));

            base.OnAppearing();
        }

        private void RunActionWaiver(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            Xamarin.Essentials.Preferences.Set("waiver", UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/waiverenroll", ps)));
        }

        async private void RunWorkerCompletedWaiver(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            Waiver.Text = Xamarin.Essentials.Preferences.Get("waiver", "");
        }

        private void signLaterCheckbox_IsCheckedChanged(object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            RadCheckBox checkbox = (RadCheckBox)sender;
            if (checkbox.IsChecked == true)
            {
                signatureView.IsVisible = false;
            }
            else
            {
                signatureView.IsVisible = true;
            }
        }

        private void SignLater_Clicked(object sender, EventArgs e)
        {
            if (signLaterCheckbox.IsChecked == true)
            {
                signLaterCheckbox.IsChecked = false;
                signatureView.IsVisible = true;
            }
            else
            {
                signLaterCheckbox.IsChecked = true;
                signatureView.IsVisible = false;
            }
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;
            if (signLaterCheckbox.IsChecked == false && signatureView.IsBlank == true)
            {
                InputMissing.IsVisible = true;
                await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
            }
            else
            {
                if (signLaterCheckbox.IsChecked == true)
                {
                    Xamarin.Essentials.Preferences.Set("signature", "");
                    Xamarin.Essentials.Preferences.Set("signatureguest", "0");
                    await Shell.Current.Navigation.PushAsync(new EnrollSingle());
                }
                else
                {
                    var s = signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, (int)s.Length);
                    string sig = Convert.ToBase64String(b);
                    Xamarin.Essentials.Preferences.Set("signature", "data:image/png;base64," + sig);
                    Xamarin.Essentials.Preferences.Set("signatureguest", "1");
                    await Shell.Current.Navigation.PushAsync(new EnrollSingle());
                }
            }
        }
    }
}