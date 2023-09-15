using mygymmobiledata;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollSingle : ContentPage
    {
        public EnrollSingle()
        {
            InitializeComponent();
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }

        protected override void OnAppearing()
        {
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int childId = System.Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
            ChildMobile child = null;
            foreach (ChildMobile c in account.Children)
            {
                if (c.ChildId == childId)
                {
                    child = c;
                    break;
                }
            }
            ScheduleViewMobile classes = (ScheduleViewMobile)Application.Current.Properties["classes"];
            int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
            int classTemplateId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classtemplateid", ""));
            ClassView_ResultMobile cl = null;
            foreach (ClassListView_ResultMobile v in classes.ClassList)
            {
                if (v.ClassTemplateId == classTemplateId)
                {
                    foreach (ClassView_ResultMobile c in v.Classes)
                    {
                        if (c.Id == classId)
                        {
                            cl = c;
                            break;
                        }
                    }
                    break;
                }
            }
            DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
            ClassName.Text = child.First + " - " + cl.Display;
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            ClassDateTime.Text = string.Format(new CultureInfo(gym.Culture), "{0:ddd} {0:MMM} {0:dd} - ", d) + string.Format(new CultureInfo(gym.Culture), "{0:h:mmt} to {1:h:mmt}", cl.Start, cl.End).ToLower(); ;

            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            if (accountStatus == 1)
            {
                EnrollTitle.Text = $"{child.First} - Schedule Guest Class";
                ActionButton.Text = "Schedule Guest Class";
            }
            else if (accountStatus == 3)
            {
                EnrollTitle.Text = $"{child.First} - Schedule Makeup";
                ActionButton.Text = "Schedule Makeup";
            }
            else if (accountStatus == 4)
            {
                EnrollTitle.Text = $"{child.First} - Schedule {gym.UnlimitedLabel}";
                ActionButton.Text = $"Schedule {gym.UnlimitedLabel}";
            }
            else if (accountStatus == 5)
            {
                EnrollTitle.Text = $"{child.First} - Book Class Card";
                ActionButton.Text = "Book Class Card";
            }
            else if (accountStatus == 6)
            {
                EnrollTitle.Text = $"{child.First} - Schedule {gym.DropInLabel}";
                ActionButton.Text = $"Schedule {gym.DropInLabel}";
            }

            base.OnAppearing();
        }

        private async void Continue_Clicked(object sender, System.EventArgs e)
        {
            Xamarin.Essentials.Preferences.Set("notes", Notes.Text);
            int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
            await Shell.Current.Navigation.PopToRootAsync();
            Xamarin.Essentials.Preferences.Set("action", "single");
            await Shell.Current.GoToAsync("//loading");
        }
    }
}