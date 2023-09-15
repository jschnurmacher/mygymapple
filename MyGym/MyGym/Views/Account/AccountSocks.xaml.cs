using System;
using System.Collections.Generic;
using mygymmobiledata;
using Xamarin.Forms;
namespace MyGym
{
    public partial class AccountSocks : ContentPage
    {
        public AccountSocks()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            string reset = Xamarin.Essentials.Preferences.Get("reset", "");
            if (reset == "1")
            {
                base.OnAppearing();
                return;
            }
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            decimal socksCost = 0M;
            decimal socksTax = 0M;
            if (gym.ShowSocksOnCheckout && gym.SocksPrice > 0)
            {
                AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                List<ChildMobile> childs = new List<ChildMobile>();
                foreach (ChildMobile c in account.Children)
                {
                    c.SocksText = $"My Gym Socks for {c.First}";
                    c.SocksReceivedOrPurchasedText = "";
                    if (c.SocksPurchased || c.SocksReceived)
                    {
                        c.SocksReceivedOrPurchasedText = "*purchased or received in the past";
                    }
                    if (DateTime.Now.Subtract(c.DOB).TotalDays < gym.ShowSocksAgeMin * 365)
                    {
                        c.SocksReceivedOrPurchasedText = "*socks not required due to age";
                    }
                    ChildMobile cm = new ChildMobile();
                    UtilMobile.CopyPropertyValues(c, cm);
                    childs.Add(cm);
                    if (c.IncludeSocks == true)
                    {
                        socksCost += gym.SocksPrice;
                        if (gym.SocksTax > 0)
                        {
                            socksTax += gym.SocksTax;
                        }
                    }
                }
                listView.ItemsSource = childs;
                listView.HeightRequest = childs.Count * 54;
                SocksText.Text = gym.ShowSocksText;
            }
            else
            {
                listView.IsVisible = false;
                SocksText.IsVisible = false;
            }
            CalculateSocksCost(socksCost, socksTax);
            base.OnAppearing();
        }

        private void CalculateSocksCost(decimal socksCost, decimal socksTax)
        {
            CostSummary.Text = "";
            if (socksCost > 0)
            {
                if (socksTax > 0)
                {
                    CostSummary.Text += $"My Gym Socks: {socksCost:c}\n";
                    CostSummary.Text += $"My Gym Socks Tax: {socksTax:c}\n";
                    CostSummary.Text += $"Total: {(socksCost + socksTax):c}\n";
                }
                else
                {
                    CostSummary.Text += $"My Gym Socks Total: {socksCost}\n";
                }
            }
            purchaseSocksButton.IsVisible = true;
            if (socksCost == 0)
            {
                purchaseSocksButton.IsVisible = false;
            }
        }

        void ChildSocksCheckbox_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            CheckBox b = (CheckBox)sender;
            int childId = System.Convert.ToInt32(b.ClassId);
            bool isChecked = ((CheckBox)sender).IsChecked;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            decimal socksCost = 0M;
            decimal socksTax = 0M;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    c.IncludeSocks = isChecked;
                }
                if (c.IncludeSocks == true)
                {
                    socksCost += gym.SocksPrice;
                    if (gym.SocksTax > 0)
                    {
                        socksTax += gym.SocksTax;
                    }
                }
            }
            CalculateSocksCost(socksCost, socksTax);
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AccountSocksBilling());
        }
    }
}

