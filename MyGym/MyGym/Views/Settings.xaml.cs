using System;
using System.Collections.Generic;
using mygymmobiledata;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyGym
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            waitImage.Source = $"wait{new Random().Next(17)}.webp";
            versionTitle.Text = $"Version {VersionTracking.CurrentVersion}";
            int accountId = 0;
            if (Xamarin.Essentials.Preferences.ContainsKey("accountid"))
            {
                string accountIdStr = Xamarin.Essentials.Preferences.Get("accountid", "");
                if (string.IsNullOrEmpty(accountIdStr) == false)
                {
                    accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                }
            }
            if (accountId > 0)
            {
                logOff.GestureRecognizers.Add(
                    new TapGestureRecognizer()
                    {
                        Command = new Command(() =>
                        {
                            Application.Current.Properties.Clear();
                            Xamarin.Essentials.Preferences.Set("gymid", "");
                            Xamarin.Essentials.Preferences.Set("accountid", "");
                            Xamarin.Essentials.Preferences.Set("zip", "");
                            Xamarin.Essentials.Preferences.Set("country", "");
                            Shell.Current.GoToAsync("//gymlogin");
                        })
                    });
                AccountMobile account = (AccountMobile) Application.Current.Properties["account"];
                EmailForm.Text = account.Email;
                First.Text = account.First;
                Last.Text = account.Last;
            }
            else
            {
                logOff.IsVisible = false;
                logOffBox.IsVisible = false;
            }
        }

        async void Submit_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                string email = EmailForm.Text;
                string last = Last.Text;
                string first = First.Text;
                string comments = Comments.Text;
                if (email == "" || last == "" || first == "" || comments == "")
                {
                    await DisplayAlert("Please enter all information", "Please enter all information in the required fields", "Close");
                }
                else
                {
                    string gymName = "";
                    if (Application.Current.Properties.ContainsKey("gym"))
                    {
                        GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                        gymName = gym.Name;
                    }
                    var message = new EmailMessage
                    {
                        Subject = "My Gym App Support Ticket",
                        Body = "Gym: " + gymName + "\r\n" + "Email: " + EmailForm.Text + "\r\n" + "First: " + First.Text + "\r\n" + "Last: " + Last.Text + "\r\n" + "Issue: " + Comments.Text,
                        To = new List<string> { "jschnurmacher@gmail.com", "appsupport@mygym.com" }
                    };
                    await Email.ComposeAsync(message);

                }
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                await DisplayAlert("Email not supported", "Email is not supported on this device." + fbsEx.Message, "Close");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Email not supported", "Email is not supported on this device. " + ex.Message, "Close");
            }

        }
    }

    public class MaskedBehaviorPhone : Behavior<Entry>
    {
        private string _mask = "";
        public MaskedBehaviorPhone()
        {
            SetMask();
        }
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        public void SetMask()
        {
            GymMobile gym = (GymMobile) Application.Current.Properties["gym"];
            if (gym.Culture == "en-GB")
            {
                Mask = "(XXXXX) XXXXXX";
            }
            else if (gym.Culture == "en-AU")
            {
                Mask = "(XX) XXXX XXXX";
            }
            else if (gym.Culture == "fr-FR")
            {
                Mask = "XX XX XX XX XX";
            }
            else if (gym.Country.ToLower() == "india")
            {
                Mask = "+XX XXXXX XXXXX";
            }
            else if (gym.Country.ToLower() == "bahrain")
            {
                Mask = "+XXX XXXX XXXX";
            }
            else if (gym.Culture == "ru-RU")
            {
                Mask = "XXX-XX-XX";
            }
            else if (gym.Culture == "lv-LV")
            {
                Mask = "X XX XXXXX";
            }
            else
            {
                Mask = "(XXX) XXX-XXXX";
            }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        IDictionary<int, char> _positions;

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
                return;

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();
                    if (text.Substring(position.Key, 1) != value)
                        text = text.Insert(position.Key, value);
                }

            if (entry.Text != text)
                entry.Text = text;
        }
    }

    public class MaskedBehaviorZip : Behavior<Entry>
    {
        private string _mask = "";
        public MaskedBehaviorZip()
        {
            SetMask();
        }
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                SetPositions();
            }
        }

        public void SetMask()
        {
            GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
            if (gym.Culture == "en-GB")
            {
                Mask = "XXXX XXX";
            }
            else if (gym.Culture == "en-AU")
            {
                Mask = "XXXX";
            }
            else if (gym.Culture == "fr-FR")
            {
                Mask = "XXX XXX";
            }
            else if (gym.Culture == "nl-NL")
            {
                Mask = "XXXXXX";
            }
            else if (gym.Culture == "lv-LV")
            {
                Mask = "XX-XXXX";
            }
            else if (gym.Culture == "ru-RU")
            {
                Mask = "XXXXX";
            }
            else
            {
                Mask = "XXXXX";
            }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        IDictionary<int, char> _positions;

        void SetPositions()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positions = null;
                return;
            }

            var list = new Dictionary<int, char>();
            for (var i = 0; i < Mask.Length; i++)
                if (Mask[i] != 'X')
                    list.Add(i, Mask[i]);

            _positions = list;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var entry = sender as Entry;

            var text = entry.Text;

            if (string.IsNullOrWhiteSpace(text) || _positions == null)
                return;

            if (text.Length > _mask.Length)
            {
                entry.Text = text.Remove(text.Length - 1);
                return;
            }

            foreach (var position in _positions)
                if (text.Length >= position.Key + 1)
                {
                    var value = position.Value.ToString();
                    if (text.Substring(position.Key, 1) != value)
                        text = text.Insert(position.Key, value);
                }

            if (entry.Text != text)
                entry.Text = text;
        }
    }
}
