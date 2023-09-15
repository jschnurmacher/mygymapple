using System;
using mygymmobiledata;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TitleBar : ContentView
    {
        public TitleBar()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            GymMobile gym = null;
            if (Application.Current.Properties.ContainsKey("gym"))
            {
                gym = (GymMobile)Application.Current.Properties["gym"];
                if (gym != null)
                {
                    ShellTitle.Text = gym.Name;
                }
            }

            base.OnSizeAllocated(width, height);
        }

        async private void Home_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync("//accounthome");
        }
    }
}
