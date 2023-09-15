using mygymmobiledata;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountProfileEditChild : ContentPage
    {
        public AccountProfileEditChild()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            ChildMobile child = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            ChildDOB.MaximumDate = DateTime.Now;
            ChildDOB.MinimumDate = DateTime.Now.AddYears(-18);
            InputMissing.IsVisible = false;
            List<CustomListItemMobile> l = new List<CustomListItemMobile>();
            l.Add(new CustomListItemMobile { Text = "Female", Value = "2" });
            l.Add(new CustomListItemMobile { Text = "Male", Value = "1" });
            l.Add(new CustomListItemMobile { Text = "Prefer Not To Specify", Value = "0" });
            Gender.ItemsSource = l;
            ChildFirst.Text = "";
            ChildLast.Text = "";
            ChildDOB.Date = UtilMobile.MinDate;
            Allergies.Text = "";
            SpecialNeeds.Text = "";
            if (child != null)
            {
                ChildFirst.Text = child.First;
                ChildLast.Text = child.Last;
                ChildDOB.Date = child.DOB;
                Gender.SelectedItem = ((List<CustomListItemMobile>)Gender.ItemsSource).Find(m => m.Value == child.Gender.ToString());
                Allergies.Text = child.Allergy;
                SpecialNeeds.Text = child.SpecialNeeds;
            }
            base.OnAppearing();
        }

        async private void Submit_Clicked(object sender, System.EventArgs e)
        {
            InputMissing.IsVisible = false;

            string childFirst = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(ChildFirst.Text));
            string childLast = UtilMobile.ProperCase(UtilMobile.RemoveNumeric(ChildLast.Text));
            if (childFirst == "" || childLast == "" || ChildDOB.Date == null || Gender.SelectedItem == null)
            {
                InputMissing.IsVisible = true;
            }
            else
            {
                ChildMobile c = new ChildMobile();
                c.First = childFirst;
                c.Last = childLast;
                c.DOB = Convert.ToDateTime(ChildDOB.Date);
                c.Gender = Convert.ToInt32(((CustomListItemMobile)Gender.SelectedItem).Value);
                c.Allergy = Allergies.Text;
                c.SpecialNeeds = SpecialNeeds.Text;
                Application.Current.Properties["updatechild"] = c;
                Xamarin.Essentials.Preferences.Set("action", "accountupdatechild");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//loading");
            }
        }

        private async void BackToProfile_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofilechildren");
        }

        private async void Cancel_Clicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("//accountprofilechildren");
        }
    }
}