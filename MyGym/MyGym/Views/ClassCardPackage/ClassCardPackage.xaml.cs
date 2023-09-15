using System;
using System.Globalization;
using System.Security.Cryptography;
using mygymmobiledata;
using Xamarin.Forms;

namespace MyGym
{
    public partial class ClassCardPackage : ContentPage
    {
        public ClassCardPackage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            int classCardPackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classcardpackageid", ""));
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
            foreach (ClassCardPackageMobile pk in p.ClassCardPackages)
            {
                if (pk.Id == classCardPackageId)
                {
                    ClassCardPackageTitle.Text = pk.Name;
                    AmountStr.Text = string.Format(new CultureInfo(gym.Culture), "Cost: {0:c}\r\n", pk.Amount);
                    if (pk.MemberRequired == true && member == false)
                    {
                        AmountStr.Text += string.Format(new CultureInfo(gym.Culture), "Membership Fee: {0:c}\r\n", gym.MembershipFee);
                    }
                    if (pk.Tax > 0)
                    {
                        AmountStr.Text += string.Format(new CultureInfo(gym.Culture), "Tax: {0:c}\r\n", pk.Tax);
                    }
                    if (pk.MemberRequired == true && member == false)
                    {
                        AmountStr.Text += string.Format(new CultureInfo(gym.Culture), "\r\n* a {0:c} membership fee is required to purchase this class card.", gym.MembershipFee);
                    }
                    Description.Text = pk.Description;
                    Ages.Text = pk.Ages;
                    CreditsStr.Text = $"{pk.Credits} Classes";
                    ValidDaysStr.Text = $"Card is Valid for {pk.ValidDays} Days";
                    bookButton.Text = $"Purchase {pk.Name}";
                    break;
                }
            }
        }

        async void BookClassCard_Clicked(System.Object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new ClassCardDetail());
        }
    }
}
