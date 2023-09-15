using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using mygymmobiledata;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace MyGym
{
    public partial class PartyChildren : ContentPage
    {
        public PartyChildren()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            List<ChildMobile> children = new List<ChildMobile>();
            List<ChildMobile> childrenOld = new List<ChildMobile>();
            foreach (ChildMobile c in account.Children)
            {
                double age = DateTime.Now.Subtract(c.DOB).TotalDays / 365.0;
                if (age <= gym.BirthdayMaxAge)
                {
                    ChildMobile m = new ChildMobile();
                    UtilMobile.CopyPropertyValues(c, m);
                    children.Add(m);
                }
                else
                {
                    ChildMobile m = new ChildMobile();
                    UtilMobile.CopyPropertyValues(c, m);
                    m.First += "  - too old";
                    childrenOld.Add(m);
                }
            }
            childrenList.ItemsSource = children;
            childrenList.HeightRequest = children.Count * 30;
            childrenListOld.ItemsSource = childrenOld;
            childrenListOld.HeightRequest = childrenOld.Count * 30;
        }

        void Child_IsCheckedChanged(System.Object sender, Telerik.XamarinForms.Primitives.CheckBox.IsCheckedChangedEventArgs e)
        {
            RadCheckBox b = (RadCheckBox)sender;
            int childId = Convert.ToInt32(b.ClassId);
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    if (Convert.ToBoolean(b.IsChecked) && ((List<ChildMobile>)Application.Current.Properties["selectedchildren"]).Contains(c) == false)
                    {
                        ((List<ChildMobile>)Application.Current.Properties["selectedchildren"]).Add(c);
                    }
                    else
                    {
                        ((List<ChildMobile>)Application.Current.Properties["selectedchildren"]).Remove(c);
                    }
                    break;
                }
            }
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            List<ChildMobile> selected = (List<ChildMobile>)Application.Current.Properties["selectedchildren"];
            if (selected.Count == 0)
            {
                await DisplayAlert("No Children Selected", "Please select at least one child to continue", "Close");
            }
            else
            {
                PartyTimeMobile t = (PartyTimeMobile)Application.Current.Properties["partyselectedtime"];
                DateTime now = DateTime.Now;
                bool pastBuffer = false;
                if (gym.OnlinePartyAddOnBufferDays > 0)
                {
                    pastBuffer = now > t.Start.AddDays(-gym.OnlinePartyAddOnBufferDays);
                }
                if (gym.EnableOnlinePartyAddOns == true && pastBuffer == false)
                {
                    await Shell.Current.Navigation.PushAsync(new PartyAddOns());
                }
                else
                {
                    Application.Current.Properties["selectedaddons"] = new List<int>();
                    PartyOptionsMobile s = new PartyOptionsMobile();
                    s.PartyOptions = new ObservableCollection<PartyOptionMobile>();
                    s.PartyOptions.Add(new PartyOptionMobile { Display = "I do not want any upgrades at this time", Id = 0 });
                    Application.Current.Properties["partyaddons"] = s;
                    await Shell.Current.Navigation.PushAsync(new PartyBilling());
                }
            }
        }
    }
}
