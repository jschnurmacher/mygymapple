using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Telerik.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountWaiverSign : ContentPage
    {
        public AccountWaiverSign()
        {
            InitializeComponent();

            AccountMobile a = (AccountMobile)Application.Current.Properties["account"];
            parent1.Text = "Parent:" + a.Parent1;
            if (string.IsNullOrEmpty(a.SignatureWaiver) == false)
            {
                var byteArray = Convert.FromBase64String(a.SignatureWaiver.Replace("data:image/png;base64,", ""));
                Stream stream = new MemoryStream(byteArray);
                var imageSource = ImageSource.FromStream(() => stream);
                signatureImage.Source = imageSource;
                signatureView.IsVisible = false;
                signatureImage.IsVisible = true;
                signatureViewLabel.IsVisible = true;
            }
            else
            {
                signatureView.IsVisible = true;
                signatureImage.IsVisible = false;
                signatureViewLabel.IsVisible = false;
            }
            parent2.Text = "Parent:" + a.Parent2;
            if (string.IsNullOrEmpty(a.SignatureWaivera) == false)
            {
                var byteArraya = Convert.FromBase64String(a.SignatureWaivera.Replace("data:image/png;base64,", ""));
                Stream streama = new MemoryStream(byteArraya);
                var imageSourcea = ImageSource.FromStream(() => streama);
                signatureImagea.Source = imageSourcea;
                signatureViewa.IsVisible = false;
                signatureImagea.IsVisible = true;
                signatureViewaLabel.IsVisible = true;
            }
            else
            {
                signatureViewa.IsVisible = true;
                signatureImagea.IsVisible = false;
                signatureViewaLabel.IsVisible = false;
            }
            chaperone.Text = "Chaperone/Guardian:" + a.ChaperoneWaiver;
            if (string.IsNullOrEmpty(a.SignatureWaiverb) == false)
            {
                var byteArrayb = Convert.FromBase64String(a.SignatureWaiverb.Replace("data:image/png;base64,", ""));
                Stream streamb = new MemoryStream(byteArrayb);
                var imageSourceb = ImageSource.FromStream(() => streamb);
                signatureImageb.Source = imageSourceb;
                signatureViewb.IsVisible = false;
                signatureImageb.IsVisible = true;
                signatureViewbLabel.IsVisible = true;
            }
            else
            {
                signatureViewb.IsVisible = true;
                signatureImageb.IsVisible = false;
                signatureViewbLabel.IsVisible = false;
            }
        }

        private async void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionLiability;
            b.RunWorkerCompleted += RunWorkerCompletedLiability;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
            base.OnAppearing();
        }

        private void RunActionLiability(object sender, DoWorkEventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdLiability", gym.Id);
            string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/liability", ps));
            Application.Current.Properties["terms"] = l.Replace("\r\n", "\r\n\r\n");
        }

        async private void RunWorkerCompletedLiability(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            Terms.Text = (string)Application.Current.Properties["terms"];
        }

        private void AgreeButton_Clicked(object sender, EventArgs e)
        {
            paymentCheckbox.IsChecked = true;
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            AccountMobile a = (AccountMobile)Application.Current.Properties["account"];
            if (paymentCheckbox.IsChecked == false || (signatureView.IsBlank == true && a.SignatureWaiver == "" && signatureViewa.IsBlank == true && a.SignatureWaivera == "" && signatureViewb.IsBlank == true && a.SignatureWaiverb == ""))
            {
                await DisplayAlert("Missing Input", "Please agree to the Liability Waiver Agreement and complete at least one digital signature", "Close");
            }
            else
            {
                continueButton.IsVisible = false;
                activityIndicatorContinue.IsVisible = true;
                BackgroundWorker b = new BackgroundWorker();
                b.WorkerReportsProgress = true;
                b.WorkerSupportsCancellation = true;
                b.DoWork += RunActionSig;
                b.RunWorkerCompleted += RunWorkerCompletedSig;
                b.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }

        private void RunActionSig(object sender, DoWorkEventArgs e)
        {
            AccountMobile a = (AccountMobile)Application.Current.Properties["account"];
            string sig = "";
            if (a.SignatureWaiver == "")
            {
                var s = signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                if (s != null)
                {
                    byte[] b = new byte[s.Length];
                    s.Read(b, 0, (int)s.Length);
                    sig = Convert.ToBase64String(b);
                    sig = "data:image/png;base64," + sig;
                }
            }
            else
            {
                sig = a.SignatureWaiver;
            }
            string siga = "";
            if (a.SignatureWaivera == "")
            {
                var sa = signatureViewa.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                if (sa != null)
                {
                    byte[] ba = new byte[sa.Length];
                    sa.Read(ba, 0, (int)sa.Length);
                    siga = Convert.ToBase64String(ba);
                    siga = "data:image/png;base64," + siga;
                }
            }
            else
            {
                siga = a.SignatureWaivera;
            }

            string sigb = "";
            if (a.SignatureWaiverb == "")
            {
                var sb = signatureViewb.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                if (sb != null)
                {
                    byte[] bb = new byte[sb.Length];
                    sb.Read(bb, 0, (int)sb.Length);
                    sigb = Convert.ToBase64String(bb);
                    sigb = "data:image/png;base64," + sigb;
                }
            }
            else
            {
                sigb = a.SignatureWaiverb;
            }
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            Application.Current.Properties["account"] = UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/waiversign", ps, sig);

            Dictionary<string, object> psa = new Dictionary<string, object>();
            psa.Add("gymId", gymId);
            psa.Add("accountId", accountId);
            Application.Current.Properties["account"] = UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/waiversigna", psa, siga);

            Dictionary<string, object> psb = new Dictionary<string, object>();
            psb.Add("gymId", gymId);
            psb.Add("accountId", accountId);
            Application.Current.Properties["account"] = UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/waiversignb", psb, sigb);
        }

        async private void RunWorkerCompletedSig(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            account.Waiver = true;
            await Shell.Current.Navigation.PopAsync();
        }

    }
}