using mygymmobiledata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountWaiver : ContentPage
    {
        public AccountWaiver()
        {
            InitializeComponent();
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

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("accountIdWaiver", Convert.ToInt32(accountId));
            string s = UtilMobile.CallApiGetParamsString("/api/gym/waiver", ps);
            s = s.Replace(">rn", ">");
            s = s.Replace("rn ", "");
            s = s.Replace("rnrn", "");
            s = s.Replace("</script>rn", "</script>");
            Application.Current.Properties["waiver"] = s;
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
            string waiver = (string)Application.Current.Properties["waiver"];
            Waiver.Html = $"" +
                $"<html>" +
                $"<header><meta name='viewport' content='width=device-width, initial-scale=0.4, maximum-scale=0.4, minimum-scale=0.4, user-scalable=no'></header>" +
                $"<div>{waiver}</div>" +
                $"</html>";
            activityIndicator.IsVisible = false;
        }
    }
}
