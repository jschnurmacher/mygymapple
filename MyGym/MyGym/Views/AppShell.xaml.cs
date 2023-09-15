
using mygymmobiledata;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            string target = args.Target.Location.OriginalString;
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (target == "//loading" && args.Source == ShellNavigationSource.Pop && action == "readpartydates")
            {
                Xamarin.Essentials.Preferences.Set("action", "returntopartypackages");
            }
            else if (target == "//gymclasses" && args.Source == ShellNavigationSource.ShellItemChanged)
            {
                string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
                if (accountId == "" || accountId == "null")
                {
                    Xamarin.Essentials.Preferences.Set("childid", "");
                    Xamarin.Essentials.Preferences.Set("accountstatus", "");
                    Xamarin.Essentials.Preferences.Set("accountaction", "");
                }
            }
            else if (target == "//campselect")
            {
                Xamarin.Essentials.Preferences.Set("action", "readcamps");
                Xamarin.Essentials.Preferences.Set("campevent", "camp");
            }
            else if (target == "//eventselect")
            {
                Xamarin.Essentials.Preferences.Set("action", "readcamps");
                Xamarin.Essentials.Preferences.Set("campevent", "event");
            }
            else if (target == "//partyselect")
            {
                Xamarin.Essentials.Preferences.Set("action", "readparty");
            }
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
        }
    }
}