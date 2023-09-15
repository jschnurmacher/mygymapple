using mygymmobiledata;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollConfirm : ContentPage
    {
        public EnrollConfirm()
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
            string reset = Xamarin.Essentials.Preferences.Get("reset", "");
            if (reset == "1")
            {
                base.OnAppearing();
                return;
            }
            AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
            int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
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
            if (account.ErrorMessage != null && account.ErrorMessage != "")
            {
                ErrorMessage.IsVisible = true;
                ErrorMessage.Text = "There was an error processing your transactions and your enrollment did not complete. " + account.ErrorMessage;
                if (accountStatus == 2 || accountStatus == 7)
                {
                    ErrorMessageLink.IsVisible = true;
                }
                SuccessMessage.IsVisible = false;
            }
            else
            {
                SuccessMessage.IsVisible = true;
                SuccessMessage.Text = "You are Confirmed!";
                ErrorMessage.IsVisible = false;
                ErrorMessageLink.IsVisible = false;
            }

            if (accountStatus == 1)
            {
                EnrollTitle.Text = "Guest Class Confirmation";
            }
            else if (accountStatus == 3)
            {
                EnrollTitle.Text = "Makeup Class Confirmation";
            }
            else if (accountStatus == 4)
            {
                EnrollTitle.Text = $"{gym.UnlimitedLabel} Confirmation";
            }
            else if (accountStatus == 5)
            {
                EnrollTitle.Text = "Class Confirmation";
            }
            else if (accountStatus == 6)
            {
                EnrollTitle.Text = $"{gym.DropInLabel} Confirmation";
            }
            else if (accountStatus == 7)
            {
                EnrollTitle.Text = "Trial Confirmation";
                if (gym.TrialNoPayment == true)
                {
                    SuccessMessage.Text = gym.TrialNoPaymentText;
                }
            }

            base.OnAppearing();
        }

        private async void EditPayment_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//accounttransbilling");
        }
    }
}