using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Telerik.XamarinForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountVIMASign : ContentPage
    {
        public AccountVIMASign()
        {
            InitializeComponent();
        }

        private async void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            EnrollMobile enroll = null;
            foreach (ChildMobile c in account.Children)
            {
                foreach (EnrollMobile e in c.Enrolls)
                {
                    if (e.EnrollId == enrollId)
                    {
                        enroll = e;
                        break;
                    }
                }
                if (enroll != null)
                {
                    break;
                }
            }

            AccountMobile a = (AccountMobile)Application.Current.Properties["account"];
            if (string.IsNullOrEmpty(enroll.SignatureVIMA) == false)
            {
                parent1.Text = "Parent:" + enroll.Parent1;
                var byteArray = Convert.FromBase64String(enroll.SignatureVIMA.Replace("data:image/png;base64,", ""));
                Stream stream = new MemoryStream(byteArray);
                var imageSource = ImageSource.FromStream(() => stream);
                signatureImage.Source = imageSource;
                signatureView.IsVisible = false;
                signatureImage.IsVisible = true;
                signatureViewLabel.IsVisible = true;
            }
            else
            {
                parent1.Text = "Parent:" + account.Parent1;
                signatureView.IsVisible = true;
                signatureImage.IsVisible = false;
                signatureViewLabel.IsVisible = false;
            }
            if (string.IsNullOrEmpty(enroll.SignatureVIMAa) == false)
            {
                parent2.Text = "Parent:" + enroll.Parent2;
                var byteArray = Convert.FromBase64String(enroll.SignatureVIMAa.Replace("data:image/png;base64,", ""));
                Stream stream = new MemoryStream(byteArray);
                var imageSource = ImageSource.FromStream(() => stream);
                signatureImagea.Source = imageSource;
                signatureViewa.IsVisible = false;
                signatureImagea.IsVisible = true;
                signatureViewaLabel.IsVisible = true;
            }
            else
            {
                parent2.Text = "Parent:" + account.Parent2;
                signatureViewa.IsVisible = true;
                signatureImagea.IsVisible = false;
                signatureViewaLabel.IsVisible = false;
            }
            if (string.IsNullOrEmpty(enroll.SignatureVIMAb) == false)
            {
                chaperone.Text = "Chaperone/Guardian:" + enroll.Chaperone;
                var byteArray = Convert.FromBase64String(enroll.SignatureVIMAb.Replace("data:image/png;base64,", ""));
                Stream stream = new MemoryStream(byteArray);
                var imageSource = ImageSource.FromStream(() => stream);
                signatureImageb.Source = imageSource;
                signatureViewb.IsVisible = false;
                signatureImageb.IsVisible = true;
                signatureViewbLabel.IsVisible = true;
            }
            else
            {
                chaperone.Text = "Chaperone/Guardian:" + account.ChaperoneWaiver;
                signatureViewb.IsVisible = true;
                signatureImageb.IsVisible = false;
                signatureViewbLabel.IsVisible = false;
            }

            BackgroundWorker b;
            b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunActionTerms;
            b.RunWorkerCompleted += RunWorkerCompletedTerms;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionTerms(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymIdTerms", gymId);
            ps.Add("childIdTerms", childId);
            ps.Add("classIdTerms", enrollId);
            ps.Add("accountStatus", 100);
            ps.Add("storeCredit", 0);
            ps.Add("includeSocks", 0);
            string l = UtilMobile.ConvertHtml(UtilMobile.CallApiGetParamsString("/api/gym/termsenroll", ps));
            string[] a = l.Split('|');
            Xamarin.Essentials.Preferences.Set("costsummary", a[0]);
            Xamarin.Essentials.Preferences.Set("terms", a[1].Replace("rnrn", "\r\n\r\n"));
            base.OnAppearing();
        }

        async private void RunWorkerCompletedTerms(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            CostSummary.Text = Xamarin.Essentials.Preferences.Get("costsummary", "");
            Terms.Text = Xamarin.Essentials.Preferences.Get("terms", "");
        }

        private void AgreeButton_Clicked(object sender, EventArgs e)
        {
            paymentCheckbox.IsChecked = true;
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            EnrollMobile enroll = null;
            foreach (ChildMobile c in account.Children)
            {
                foreach (EnrollMobile em in c.Enrolls)
                {
                    if (em.EnrollId == enrollId)
                    {
                        enroll = em;
                        break;
                    }
                }
                if (enroll != null)
                {
                    break;
                }
            }
            if (paymentCheckbox.IsChecked == false || (signatureView.IsBlank == true && enroll.SignatureVIMA == "" && signatureViewa.IsBlank == true && enroll.SignatureVIMAa == "" && signatureViewb.IsBlank == true && enroll.SignatureVIMAb == ""))
            {
                await DisplayAlert("Missing Input", "Please agree to the Terms and complete the digital signature", "Close");
                await scrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
            }
            else
            {
                if (enroll.SignatureVIMA == "")
                {
                    var s = signatureView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                    if (s != null)
                    {
                        byte[] b = new byte[s.Length];
                        s.Read(b, 0, (int)s.Length);
                        string sig = Convert.ToBase64String(b);
                        sig = "data:image/png;base64," + sig;
                        Application.Current.Properties["signature"] = sig;
                    }
                    else
                    {
                        Application.Current.Properties["signature"] = "";
                    }
                }
                else
                {
                    Application.Current.Properties["signature"] = enroll.SignatureVIMA;
                }

                if (enroll.SignatureVIMAa == "")
                {
                    var sa = signatureViewa.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                    if (sa != null)
                    {
                        byte[] ba = new byte[sa.Length];
                        sa.Read(ba, 0, (int)sa.Length);
                        string siga = Convert.ToBase64String(ba);
                        siga = "data:image/png;base64," + siga;
                        Application.Current.Properties["signaturea"] = siga;
                    }
                    else
                    {
                        Application.Current.Properties["signaturea"] = "";
                    }
                }
                else
                {
                    Application.Current.Properties["signaturea"] = enroll.SignatureVIMAa;
                }

                if (enroll.SignatureVIMAb == "")
                {
                    var sb = signatureViewb.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png).Result;
                    if (sb != null)
                    {
                        byte[] bb = new byte[sb.Length];
                        sb.Read(bb, 0, (int)sb.Length);
                        string sigb = Convert.ToBase64String(bb);
                        sigb = "data:image/png;base64," + sigb;
                        Application.Current.Properties["signatureb"] = sigb;
                    }
                    else
                    {
                        Application.Current.Properties["signatureb"] = "";
                    }
                }
                else
                {
                    Application.Current.Properties["signatureb"] = enroll.SignatureVIMAb;
                }

                BackgroundWorker b1;
                b1 = new BackgroundWorker();
                b1.WorkerReportsProgress = true;
                b1.WorkerSupportsCancellation = true;
                b1.DoWork += RunActionSig;
                b1.RunWorkerCompleted += RunWorkerCompletedSig;
                b1.RunWorkerAsync(new DoWorkEventArgs(null));
            }
        }

        private void RunActionSig(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            ps.Add("enrollId", enrollId);
            string sig = (string)Application.Current.Properties["signature"];
            Application.Current.Properties["account"] = UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/vimasign", ps, sig);
        }

        private async void RunWorkerCompletedSig(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            BackgroundWorker b1;
            b1 = new BackgroundWorker();
            b1.WorkerReportsProgress = true;
            b1.WorkerSupportsCancellation = true;
            b1.DoWork += RunActionSiga;
            b1.RunWorkerCompleted += RunWorkerCompletedSiga;
            b1.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionSiga(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            ps.Add("enrollId", enrollId);
            string siga = (string)Application.Current.Properties["signaturea"];
            Application.Current.Properties["account"] = UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/vimasigna", ps, siga);
        }

        private async void RunWorkerCompletedSiga(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            BackgroundWorker b1;
            b1 = new BackgroundWorker();
            b1.WorkerReportsProgress = true;
            b1.WorkerSupportsCancellation = true;
            b1.DoWork += RunActionSigb;
            b1.RunWorkerCompleted += RunWorkerCompletedSigb;
            b1.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunActionSigb(object sender, DoWorkEventArgs e)
        {
            int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
            int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
            int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("gymId", gymId);
            ps.Add("accountId", accountId);
            ps.Add("enrollId", enrollId);
            string sigb = (string)Application.Current.Properties["signatureb"];
            Application.Current.Properties["account"] = UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/vimasignb", ps, sigb);
        }

        private async void RunWorkerCompletedSigb(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
                return;
            }
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }

    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;

            Assembly assembly = typeof(ImageResourceExtension).Assembly;
            var imageSource = ImageSource.FromResource(Source, assembly);

            return imageSource;
        }
    }
}
