using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using mygymmobiledata;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace MyGym
{
    public partial class AccountSocksSummary : ContentPage
    {
        public AccountSocksSummary()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            decimal socksCost = 0M;
            decimal socksTax = 0M;
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            List<ChildMobile> childs = new List<ChildMobile>();
            foreach (ChildMobile c in account.Children)
            {
                if (c.IncludeSocks)
                {
                    c.SocksText = $"My Gym Socks for {c.First}";
                    c.SocksReceivedOrPurchasedText = "";
                    ChildMobile cm = new ChildMobile();
                    UtilMobile.CopyPropertyValues(c, cm);
                    childs.Add(cm);
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
            listView.ItemsSource = childs;
            listView.HeightRequest = childs.Count * 54;
            SocksText.Text = gym.ShowSocksText;
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

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            Xamarin.Essentials.Preferences.Set("action", "purchasesocks");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}
