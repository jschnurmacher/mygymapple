using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountBillingEditHTML : ContentPage
    {
        public AccountBillingEditHTML()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var source = new HtmlWebViewSource();
            string url = DependencyService.Get<IBaseUrl>().Get();
            string uri = Path.Combine(url, "Billing.html");
            source.BaseUrl = url;
            using (var sr = new StreamReader(uri))
            {
                source.Html = sr.ReadToEnd();
                webView.Source = source;
            }
            webView.RegisterDisplayJSTextAction(DisplayJSTextAction);
        }

        private void DisplayJSTextAction(string text)
        {
            Device.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert(text, string.Empty, "Great"));
        }
    }

    public class HybridWebView : WebView
    {
        public void Cleanup()
        {
            _displayJSTextAction = null;
        }

        private Action<string> _displayJSTextAction;

        public object Uri
        {
            get
            {
                string url = DependencyService.Get<IBaseUrl>().Get();
                string uri = Path.Combine(url, "Billing.html");
                return uri;
            }
            set { }
        }

        public void RegisterDisplayJSTextAction(Action<string> callback)
        {
            _displayJSTextAction = callback;
        }

        public void InvokeDisplayJSTextAction(string text)
        {
            _displayJSTextAction?.Invoke(text);
        }


        public void SetXFAppText(string text) => Device.BeginInvokeOnMainThread(async () => await EvaluateJavaScriptAsync($"DisplayXFText('{text}')"));

        public void InvokeAction(string v)
        {
            throw new NotImplementedException();
        }
    }
}

