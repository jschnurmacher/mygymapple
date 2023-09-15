using System;
using System.Globalization;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class ClassCardPackages : ContentPage
    {
        public ClassCardPackages()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            bool member = false;
            foreach (ChildMobile c in account.Children)
            {
                if (c.Member == true)
                {
                    member = true;
                    break;
                }
            }
            ClassCardMobile p = (ClassCardMobile)Application.Current.Properties["classcardpackages"];
            foreach (ClassCardPackageMobile pp in p.ClassCardPackages)
            {
                pp.CreditsStr = $"{pp.Credits} Classes";
                pp.AmountStr = string.Format(new CultureInfo(gym.Culture), "{0:c}", pp.Amount);
                if (pp.MemberRequired && member == false)
                {
                    pp.AmountStr += $"\r\n\r\n* a {gym.MembershipFee:c} membership fee is required to purchase this class card.";
                }
                pp.ValidDaysStr = $"Card is Valid for {pp.ValidDays} Days";
            }
            classCardDesc.Text = UtilMobile.ConvertHtml(gym.ClassCardIndexDescription);
            listView.ItemsSource = p.ClassCardPackages;
        }

        async void BookClassCard_Clicked(System.Object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("classcardpackageid", Convert.ToString(((Button)sender).CommandParameter));
            await Shell.Current.GoToAsync("//classcardpackage");
        }
    }
}
