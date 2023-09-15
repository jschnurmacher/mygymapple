using mygymmobiledata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Telerik.XamarinForms.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyGym
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loading : ContentPage
    {
        public Loading()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            waitImage.Source = $"wait{new Random().Next(17)}.webp";
            BackgroundWorker b = new BackgroundWorker();
            b.WorkerReportsProgress = true;
            b.WorkerSupportsCancellation = true;
            b.DoWork += RunAction;
            b.RunWorkerCompleted += RunWorkerCompleted;
            b.RunWorkerAsync(new DoWorkEventArgs(null));
        }

        private void RunAction(object sender, DoWorkEventArgs e)
        {
            try
            {
                Xamarin.Essentials.Preferences.Set("error", "");
                string action = Xamarin.Essentials.Preferences.Get("action", "");
                if (action == "init")
                {
                    string zip = Xamarin.Essentials.Preferences.Get("zip", "");
                    if (zip != "" && zip != "null")
                    {
                        Dictionary<string, object> ps = new Dictionary<string, object>();
                        ps.Add("zip", zip);
                        GymsMobile gyms = (GymsMobile)UtilMobile.CallApiGetParams<GymsMobile>($"/api/gym/readlocations", ps);
                        Application.Current.Properties["gyms"] = gyms;
                    }
                    else
                    {
                        string country = Xamarin.Essentials.Preferences.Get("country", "");
                        if (country != "" && country != "null")
                        {
                            Dictionary<string, object> ps = new Dictionary<string, object>();
                            ps.Add("country", country);
                            GymsMobile gyms = (GymsMobile)UtilMobile.CallApiGetParams<GymsMobile>($"/api/gym/readlocationcountries", ps);
                            Application.Current.Properties["gyms"] = gyms;
                        }
                    }
                    string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
                    if (gymId != "0" && gymId != "" && gymId != "null")
                    {
                        Dictionary<string, object> ps = new Dictionary<string, object>();
                        ps.Add("gymIdReadGym", gymId.ToString());
                        GymMobile gym = (GymMobile)UtilMobile.CallApiGetParams<GymMobile>($"/api/gym/readgym", ps);
                        Application.Current.Properties["gym"] = gym;
                        UtilMobile.ReadCamps();
                    }
                    string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
                    if (accountId != "0" && accountId != "" && accountId != "null")
                    {
                        Dictionary<string, object> ps = new Dictionary<string, object>();
                        ps.Add("accountId", accountId.ToString());
                        ps.Add("version", VersionTracking.CurrentVersion);
                        AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/readaccount", ps);
                        Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                        if (account.ErrorMessage.Contains("Please Update") == false)
                        {
                            Application.Current.Properties["account"] = account;
                            ps = new Dictionary<string, object>();
                            ps.Add("gymIdReadGym", account.GymId.ToString());
                            GymMobile gym = (GymMobile)UtilMobile.CallApiGetParams<GymMobile>($"/api/gym/readgym", ps);
                            Application.Current.Properties["gym"] = gym;
                            UtilMobile.ReadCamps();
                        }
                        else
                        {
                            Xamarin.Essentials.Preferences.Set("action", "errorpage");
                        }
                    }
                    Application.Current.Properties["selectedchildren"] = new List<ChildMobile>();
                    Application.Current.Properties["selectedsessions"] = new List<EventDateMobile>();
                    Application.Current.Properties["creditapply"] = 0;
                    Application.Current.Properties["creditavailable"] = 0;
                }
                else if (action == "loadgyms")
                {
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("zip", Xamarin.Essentials.Preferences.Get("zip", ""));
                    GymsMobile gymsMobile = (GymsMobile)UtilMobile.CallApiGetParams<GymsMobile>($"/api/gym/readlocations", ps);
                    Application.Current.Properties["gyms"] = gymsMobile;
                }
                else if (action == "loadgymscountries")
                {
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("country", Xamarin.Essentials.Preferences.Get("country", ""));
                    GymsMobile gymsMobile = (GymsMobile)UtilMobile.CallApiGetParams<GymsMobile>($"/api/gym/readlocationscountries", ps);
                    Application.Current.Properties["gyms"] = gymsMobile;
                }
                else if (action == "login")
                {
                    int loginGym = 0;
                    if (Application.Current.Properties.ContainsKey("logingym"))
                    {
                        loginGym = Convert.ToInt32(Application.Current.Properties["logingym"]);
                    }
                    if (loginGym > 0)
                    {
                        Application.Current.Properties["accounts"] = new List<AccountMobile>();
                        Dictionary<string, object> ps = new Dictionary<string, object>();
                        ps.Add("username", Xamarin.Essentials.Preferences.Get("email", ""));
                        ps.Add("password", Xamarin.Essentials.Preferences.Get("password", ""));
                        ps.Add("gymId", loginGym);
                        string accountIdStr = (string)UtilMobile.CallApiGetParams<string>("/api/gym/logingym", ps);
                        int accountId = Convert.ToInt32(accountIdStr.Replace("\"", ""));
                        Application.Current.Properties["account"] = null;
                        if (accountId > 0)
                        {
                            ps = new Dictionary<string, object>();
                            ps.Add("accountId", accountId.ToString());
                            ps.Add("version", VersionTracking.CurrentVersion);
                            AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/readaccount", ps);
                            Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                            if (account.ErrorMessage.Contains("Please Update") == false)
                            {
                                Application.Current.Properties["account"] = account;
                                ps = new Dictionary<string, object>();
                                ps.Add("gymIdReadGym", account.GymId.ToString());
                                GymMobile gym = (GymMobile)UtilMobile.CallApiGetParams<GymMobile>($"/api/gym/readgym", ps);
                                Application.Current.Properties["gym"] = gym;
                                UtilMobile.ReadCamps();
                            }
                            else
                            {
                                Xamarin.Essentials.Preferences.Set("action", "errorpage");
                            }
                            Xamarin.Essentials.Preferences.Set("accountid", account.AccountId.ToString());
                            Xamarin.Essentials.Preferences.Set("gymid", account.GymId.ToString());
                        }
                    }
                    else
                    {
                        Dictionary<string, object> ps = new Dictionary<string, object>();
                        ps.Add("username", Xamarin.Essentials.Preferences.Get("email", ""));
                        Application.Current.Properties["accounts"] = (List<AccountMobile>)UtilMobile.CallApiGetParams<List<AccountMobile>>("/api/gym/loginmultiple", ps);
                        List<AccountMobile> accs = (List<AccountMobile>)Application.Current.Properties["accounts"];
                        if (accs.Count == 0)
                        {
                            Application.Current.Properties["account"] = null;
                            Xamarin.Essentials.Preferences.Set("accountid", "");
                            Xamarin.Essentials.Preferences.Set("gymid", "");
                        }
                        else if (accs.Count == 1)
                        {
                            ps = new Dictionary<string, object>();
                            ps.Add("username", Xamarin.Essentials.Preferences.Get("email", ""));
                            ps.Add("password", Xamarin.Essentials.Preferences.Get("password", ""));
                            string accountIdStr = (string)UtilMobile.CallApiGetParams<string>("/api/gym/login", ps);
                            int accountId = Convert.ToInt32(accountIdStr.Replace("\"", ""));
                            Application.Current.Properties["account"] = null;
                            if (accountId > 0)
                            {
                                ps = new Dictionary<string, object>();
                                ps.Add("accountId", accountId.ToString());
                                ps.Add("version", VersionTracking.CurrentVersion);
                                AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/readaccount", ps);
                                Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                                if (account.ErrorMessage.Contains("Please Update") == false)
                                {
                                    Application.Current.Properties["account"] = account;
                                    ps = new Dictionary<string, object>();
                                    ps.Add("gymIdReadGym", account.GymId.ToString());
                                    GymMobile gym = (GymMobile)UtilMobile.CallApiGetParams<GymMobile>($"/api/gym/readgym", ps);
                                    Application.Current.Properties["gym"] = gym;
                                    UtilMobile.ReadCamps();
                                }
                                else
                                {
                                    Xamarin.Essentials.Preferences.Set("action", "errorpage");
                                }
                                Xamarin.Essentials.Preferences.Set("accountid", account.AccountId.ToString());
                                Xamarin.Essentials.Preferences.Set("gymid", account.GymId.ToString());
                            }
                        }
                    }
                }
                else if (action == "registerconfirm")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gym.Id);
                    AccountMobile a = (AccountMobile)Application.Current.Properties["registeraccount"];
                    ps.Add("parentFirst", a.First);
                    ps.Add("parentLast", a.Last);
                    ps.Add("email", a.Email);
                    ps.Add("childFirst", a.Children[0].First);
                    ps.Add("childLast", a.Children[0].Last);
                    ps.Add("childDOBYear", a.Children[0].DOB.Year.ToString());
                    ps.Add("childDOBMonth", a.Children[0].DOB.Month.ToString());
                    ps.Add("childDOBDay", a.Children[0].DOB.Day.ToString());
                    ps.Add("password", a.Password);
                    Application.Current.Properties["registeraccount"] = null;
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/createaccount", ps);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("accountid", account.AccountId.ToString());

                    ps = new Dictionary<string, object>();
                    ps.Add("gymIdReadGym", gym.Id);
                    gym = (GymMobile)UtilMobile.CallApiGetParams<GymMobile>($"/api/gym/readgym", ps);
                    Application.Current.Properties["gym"] = gym;
                    Xamarin.Essentials.Preferences.Set("gymid", gym.Id.ToString());
                }
                else if (action == "accountupdate")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile account = (AccountMobile)Application.Current.Properties["updateaccount"];
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gym.Id);
                    ps.Add("accountId", account.AccountId);
                    ps.Add("email", account.Email);
                    ps.Add("first", account.First);
                    ps.Add("last", account.Last);
                    ps.Add("address", account.Address);
                    ps.Add("apt", account.Apt);
                    ps.Add("city", account.City);
                    ps.Add("state", account.State);
                    ps.Add("zip", account.Zip);
                    ps.Add("phone", account.Phone1);
                    ps.Add("phoneType", account.Phone1Type);
                    ps.Add("phoneExt", account.Phone1Ext);
                    ps.Add("emergencyFirst", account.ContactFirst);
                    ps.Add("emergencyLast", account.ContactLast);
                    ps.Add("emergencyRelationship", account.ContactRelationshipId);
                    ps.Add("emergencyPhone", account.Phone3);
                    ps.Add("emergencyPhoneType", account.Phone3Type);
                    ps.Add("emergencyPhoneExt", account.Phone3Ext);
                    AccountMobile a = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/updateaccount", ps);
                    Application.Current.Properties["account"] = a;
                }
                else if (action == "accountupdatechild")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    ChildMobile child = (ChildMobile)Application.Current.Properties["updatechild"];
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gym.Id);
                    ps.Add("accountId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "")));
                    ps.Add("childId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", "")));
                    ps.Add("childFirst", child.First);
                    ps.Add("childLast", child.Last);
                    ps.Add("childDOBYear", child.DOB.Year.ToString());
                    ps.Add("childDOBMonth", child.DOB.Month.ToString());
                    ps.Add("childDOBDay", child.DOB.Day.ToString());
                    ps.Add("childGender", child.Gender.ToString());
                    ps.Add("allergies", child.Allergy == null ? "" : child.Allergy);
                    ps.Add("specialNeeds", child.SpecialNeeds == null ? "" : child.SpecialNeeds);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/updateaccountchild", ps);
                    Application.Current.Properties["account"] = account;
                }
                else if (action == "accounttranspayments")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gymId);
                    ps.Add("accountId", accountId);
                    List<TransactionMobile> trans = (List<TransactionMobile>)UtilMobile.CallApiGetParams<List<TransactionMobile>>($"/api/gym/readaccountpayments", ps);
                    Application.Current.Properties["transactions"] = trans;
                }
                else if (action == "deleteaccount")
                {
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("accountId", accountId);
                    UtilMobile.CallApiGetParams<int>($"/api/gym/deleteaccount", ps);
                }
                else if (action == "enroll")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                    int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
                    int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
                    DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
                    int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", ""));
                    string notes = Xamarin.Essentials.Preferences.Get("notes", "");
                    string signature = Xamarin.Essentials.Preferences.Get("signature", "");
                    string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
                    decimal memberCost = Convert.ToDecimal(Xamarin.Essentials.Preferences.Get("membercost", ""));
                    decimal storeCredit = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
                    int includeSocks = Convert.ToBoolean(Application.Current.Properties["includesocks"]) == true ? 1 : 0;
                    int unlimited = Convert.ToBoolean(Application.Current.Properties["unlimited"]) == true ? 1 : 0;
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymIdEnroll", gymId);
                    ps.Add("accountId", accountId);
                    ps.Add("childId", childId);
                    ps.Add("classId", classId);
                    ps.Add("classDate", d.ToShortDateString());
                    ps.Add("billingId", billingId);
                    ps.Add("notes", notes);
                    ps.Add("promoCode", promoCode);
                    ps.Add("membership", memberCost > 0 ? 1 : 0);
                    ps.Add("storeCredit", storeCredit);
                    ps.Add("includeSocks", includeSocks);
                    ps.Add("unlimited", unlimited);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>($"/api/gym/enroll", ps, signature);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                    Application.Current.Properties["creditapply"] = 0;
                    Application.Current.Properties["creditavailable"] = 0;
                }
                else if (action == "trial")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                    int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
                    int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
                    DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
                    int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", ""));
                    decimal storeCredit = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
                    string notes = Xamarin.Essentials.Preferences.Get("notes", "");
                    string signature = Xamarin.Essentials.Preferences.Get("signature", "");
                    string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
                    int includeSocks = Convert.ToBoolean(Application.Current.Properties["includesocks"]) == true ? 1 : 0;
                    int unlimited = Convert.ToBoolean(Application.Current.Properties["unlimited"]) == true ? 1 : 0;
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymIdTrial", gymId);
                    ps.Add("accountIdTrial", accountId);
                    ps.Add("childIdTrial", childId);
                    ps.Add("classIdTrial", classId);
                    ps.Add("classDateTrial", d.ToShortDateString());
                    ps.Add("notesTrial", notes);
                    ps.Add("promoCode", promoCode);
                    ps.Add("billingIdTrial", billingId);
                    ps.Add("storeCredit", storeCredit);
                    ps.Add("includeSocks", includeSocks);
                    ps.Add("unlimited", unlimited);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>($"/api/gym/trial", ps, signature);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                }
                else if (action == "single")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile acc = (AccountMobile)Application.Current.Properties["account"];
                    int childId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("childid", ""));
                    int classId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classid", ""));
                    string signature = Convert.ToString(Xamarin.Essentials.Preferences.Get("signature", ""));
                    int signatureguest = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("signatureguest", ""));
                    DateTime d = Convert.ToDateTime(Xamarin.Essentials.Preferences.Get("classdate", ""));
                    string notes = Xamarin.Essentials.Preferences.Get("notes", "");
                    int accountStatus = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "" || Xamarin.Essentials.Preferences.Get("accountstatus", "0") == "null" ? "0" : Xamarin.Essentials.Preferences.Get("accountstatus", "0"));
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymIdSingle", gym.Id);
                    ps.Add("accountId", acc.AccountId);
                    ps.Add("childId", childId);
                    ps.Add("classId", classId);
                    ps.Add("classDate", string.Format(new CultureInfo(gym.Culture), "{0:d}", d));
                    ps.Add("notes", notes);
                    ps.Add("accountStatus", accountStatus);
                    if (signatureguest == 0)
                    {
                        AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/single", ps);
                        Application.Current.Properties["account"] = account;
                        Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                    }
                    else
                    {
                        AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>($"/api/gym/singleguest", ps, signature);
                        Application.Current.Properties["account"] = account;
                        Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                    }
                    Xamarin.Essentials.Preferences.Set("signature", "");
                    Xamarin.Essentials.Preferences.Set("signatureguest", "0");
                }
                else if (action == "switch")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
                    int enrollId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("enrollid", ""));
                    int classId = Convert.ToInt32(Application.Current.Properties["switchclassid"]);
                    DateTime d = Convert.ToDateTime(Application.Current.Properties["switchdate"]);
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gymId);
                    ps.Add("enrollId", enrollId);
                    ps.Add("date", d);
                    ps.Add("classId", classId);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/switch", ps);
                    Application.Current.Properties["account"] = account;
                }
                else if (action == "upgradeunlimited")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/upgradeunlimited", ps);
                    Application.Current.Properties["account"] = account;
                }
                else if (action == "degradeunlimited")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/degradeunlimited", ps);
                    Application.Current.Properties["account"] = account;
                }
                else if (action == "enrollcamp")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", "0"));
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "0"));
                    int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", "-1"));
                    string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
                    decimal storeCredit = 0;
                    if (Application.Current.Properties.ContainsKey("creditapply"))
                    {
                        storeCredit = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
                    }
                    EventMobile ev = (EventMobile)Application.Current.Properties["camp"];
                    string classIds = "";
                    string childIds = "";
                    foreach (EventDateMobile edm in ev.SelectedDates)
                    {
                        if (classIds != "")
                        {
                            classIds += "|";
                        }
                        classIds += edm.ClassId;
                        if (childIds != "")
                        {
                            childIds += "|";
                        }
                        childIds += edm.ChildId;
                    }
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                    string socksIds = "";
                    if (gym.ShowSocksOnCheckout && gym.SocksPrice > 0)
                    {
                        if (ev.SelectedDates != null && ev.SelectedDates.Count > 0)
                        {
                            foreach (EventDateMobile edm in ev.SelectedDates)
                            {
                                foreach(ChildMobile c in account.Children)
                                {
                                    if (c.ChildId == edm.ChildId && c.IncludeSocks == true)
                                    {
                                        if (socksIds != "")
                                        {
                                            socksIds += "|";
                                        }
                                        socksIds += edm.ChildId;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (ChildMobile c in account.Children)
                            {
                                if (c.IncludeSocks == true)
                                {
                                    if (socksIds != "")
                                    {
                                        socksIds += "|";
                                    }
                                    socksIds += c.ChildId;
                                }
                            }
                        }
                    }
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymIdCamp", gymId);
                    ps.Add("accountIdCamp", accountId);
                    ps.Add("billingId", billingId);
                    ps.Add("promoCode", promoCode == null ? "" : promoCode);
                    ps.Add("storeCredit", storeCredit);
                    ps.Add("eventId", ev.EventId);
                    ps.Add("classIds", classIds);
                    ps.Add("childIds", childIds);
                    ps.Add("includeMembership", ev.EventCost.IncludeMembership == true ? 1 : 0);
                    ps.Add("displayList", ev.EventDiscountMobile == null ? "" : ev.EventDiscountMobile.DisplayList);
                    ps.Add("socksIds", socksIds);
                    ps.Add("session", ev.SelectedDates != null && ev.SelectedDates.Count > 0 ? 0 : 1);
                    string signature = Xamarin.Essentials.Preferences.Get("signature", "");
                    account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/campenroll", ps, signature);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                    Application.Current.Properties["promocode"] = "";
                    Application.Current.Properties["creditapply"] = 0;
                    Application.Current.Properties["creditavailable"] = 0;
                    UtilMobile.ReadCamps();
                }
                else if (action == "purchasesocks")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", "0"));
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", "0"));
                    int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", "-1"));
                    string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
                    decimal storeCredit = 0;
                    if (Application.Current.Properties.ContainsKey("creditapply"))
                    {
                        storeCredit = Convert.ToDecimal(Application.Current.Properties["creditapply"]);
                    }
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                    string socksIds = "";
                    if (gym.ShowSocksOnCheckout && gym.SocksPrice > 0)
                    {
                        foreach (ChildMobile c in account.Children)
                        {
                            if ( c.IncludeSocks == true)
                            {
                                if (socksIds != "")
                                {
                                    socksIds += "|";
                                }
                                socksIds += c.ChildId;
                            }
                        }
                    }
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", gymId);
                    ps.Add("accountId", accountId);
                    ps.Add("billingId", billingId);
                    ps.Add("socksIds", socksIds);
                    account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/purchasesocks", ps);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                }
                else if (action == "cancelclass")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/cancelclass", ps);
                    Application.Current.Properties["account"] = account;
                }
                else if (action == "cancelenroll")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    string lastClass = Xamarin.Essentials.Preferences.Get("lastclass", "");
                    string reason = Xamarin.Essentials.Preferences.Get("cancelreason", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    ps.Add("lastClass", lastClass);
                    ps.Add("reason", reason);
                    string signature = Xamarin.Essentials.Preferences.Get("signature", "");
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/cancelenroll", ps, signature);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                }
                else if (action == "canceltrial")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    string reason = Xamarin.Essentials.Preferences.Get("cancelreason", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    ps.Add("reason", reason);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>("/api/gym/canceltrial", ps);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                }
                else if (action == "freeze")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    DateTime date = Convert.ToDateTime(Application.Current.Properties["freezedate"]);
                    int weeks = Convert.ToInt32(Application.Current.Properties["freezeweeks"]);
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    ps.Add("date", date.ToShortDateString());
                    ps.Add("weeks", weeks);
                    string signature = Xamarin.Essentials.Preferences.Get("signature", "");
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>("/api/gym/freeze", ps, signature);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                }
                else if (action == "markabsent")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    string date = Xamarin.Essentials.Preferences.Get("absentdate", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    ps.Add("date", date);
                    ps.Add("markUnmark", 0);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/markabsent", ps);
                    Application.Current.Properties["account"] = account;
                    Xamarin.Essentials.Preferences.Set("error", account.ErrorMessage);
                }
                else if (action == "unmarkabsent")
                {
                    string enrollId = Xamarin.Essentials.Preferences.Get("enrollid", "");
                    string date = Xamarin.Essentials.Preferences.Get("absentdate", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("enrollId", Convert.ToInt32(enrollId));
                    ps.Add("date", date);
                    ps.Add("markUnmark", 1);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/markabsent", ps);
                    Application.Current.Properties["account"] = account;
                }
                else if (action == "readcamps")
                {
                    UtilMobile.ReadCamps();
                }
                else if (action == "readparty")
                {
                    string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", Convert.ToInt32(gymId));
                    PartyMobile party = (PartyMobile)UtilMobile.CallApiGetParams<PartyMobile>($"/api/gym/readparty", ps);
                    Application.Current.Properties["party"] = party;
                }
                else if (action == "readpartydates")
                {
                    string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", Convert.ToInt32(gymId));
                    PartyDatesMobile party = (PartyDatesMobile)UtilMobile.CallApiGetParams<PartyDatesMobile>($"/api/gym/readpartydates", ps);
                    Application.Current.Properties["partydates"] = party;
                }
                else if (action == "bookparty")
                {
                    int gymId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("gymid", ""));
                    int accountId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("accountid", ""));
                    int billingId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", ""));
                    string promoCode = Convert.ToString(Application.Current.Properties["promocode"]);
                    int numKids = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partynumkids", ""));
                    PartyTimeMobile partyTime = (PartyTimeMobile)Application.Current.Properties["partyselectedtime"];
                    List<ChildMobile> selectedChildren = (List<ChildMobile>)Application.Current.Properties["selectedchildren"];
                    List<int> addOns = (List<int>)Application.Current.Properties["selectedaddons"];
                    List<int> children = new List<int>();
                    foreach (ChildMobile c in selectedChildren)
                    {
                        children.Add(c.ChildId);
                    }
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", Convert.ToInt32(gymId));
                    ps.Add("accountId", Convert.ToInt32(accountId));
                    ps.Add("storeCredit", Convert.ToDecimal(Application.Current.Properties["creditapply"]).ToString());
                    ps.Add("birthdayPackageId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", "")));
                    ps.Add("numKids", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partynumkids", "")));
                    PartyBook b = new PartyBook();
                    b.BillingId = billingId;
                    b.PackageId = Convert.ToInt32(Xamarin.Essentials.Preferences.Get("partypackageid", ""));
                    b.Children = children;
                    b.AddOns = (List<int>)Application.Current.Properties["selectedaddons"];
                    b.HalfHour = Xamarin.Essentials.Preferences.Get("partyhalfhour", "") == "" ? false : true;
                    b.PartyTime = (PartyTimeMobile)Application.Current.Properties["partyselectedtime"];
                    b.PartyTime.HalfHour = b.HalfHour;
                    b.PromoCode = promoCode;
                    string json = JsonConvert.SerializeObject(b);
                    AccountMobile account = (AccountMobile)UtilMobile.CallApiGetParamsSignature<AccountMobile>($"/api/gym/bookparty", ps, json);
                    Application.Current.Properties["account"] = account;
                    Application.Current.Properties["creditapply"] = 0;
                    Application.Current.Properties["creditavailable"] = 0;
                    Application.Current.Properties["deposit"] = 0;
                }
                else if (action == "returntopartypackages")
                {
                    string comments = Convert.ToString(Application.Current.Properties["comments"]);
                }
                else if (action == "contact")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                    string comments = Convert.ToString(Application.Current.Properties["comments"]);
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", Convert.ToInt32(gym.Id));
                    ps.Add("accountId", Convert.ToInt32(account.AccountId));
                    ps.Add("comments", comments);
                    UtilMobile.CallApiGetParams<string>($"/api/gym/contact", ps);
                }
                else if (action == "readclasscardpackages")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", Convert.ToInt32(gym.Id));
                    ps.Add("accountId", Convert.ToInt32(account.AccountId));
                    Application.Current.Properties["classcardpackages"] = UtilMobile.CallApiGetParams<ClassCardMobile>($"/api/gym/readclasscardpackages", ps);
                }
                else if (action == "classcard")
                {
                    GymMobile gym = (GymMobile)Application.Current.Properties["gym"];
                    AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                    Dictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("gymId", Convert.ToInt32(gym.Id));
                    ps.Add("accountId", Convert.ToInt32(account.AccountId));
                    ps.Add("cardPackageId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("classcardpackageid", "")));
                    ps.Add("promoCode", Convert.ToString(Application.Current.Properties["promocode"]));
                    ps.Add("storeCredit", Convert.ToDecimal(Application.Current.Properties["creditapply"]));
                    ps.Add("billingId", Convert.ToInt32(Xamarin.Essentials.Preferences.Get("billingid", "")));
                    account = (AccountMobile)UtilMobile.CallApiGetParams<AccountMobile>($"/api/gym/cardpackage", ps);
                    Application.Current.Properties["account"] = account;
                }
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException != null && string.IsNullOrEmpty(ex.InnerException.Message) == false ? ex.InnerException.Message : "";
                Xamarin.Essentials.Preferences.Set("error", "Message: " + ex.Message + " Inner: " + inner + " Stack:" + ex.StackTrace);
                Xamarin.Essentials.Preferences.Set("action", "errorpage");
            }
        }
        
        private async void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string action = Xamarin.Essentials.Preferences.Get("action", "");
            if (action == "errorpage")
            {
                if (Application.Current.Properties.ContainsKey("account"))
                {
                    AccountMobile account = (AccountMobile)Application.Current.Properties["account"];
                    if (account != null && string.IsNullOrEmpty(account.ErrorMessage) == false)
                    {
                        Xamarin.Essentials.Preferences.Set("error", "Message: " + account.ErrorMessage);
                        account.ErrorMessage = "";
                    }
                }
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//errorpage");
            }
            else if (action == "init")
            {
                string gymId = Xamarin.Essentials.Preferences.Get("gymid", "");
                string accountId = Xamarin.Essentials.Preferences.Get("accountid", "");
                if (gymId == "" || gymId == "null")
                {
                    await Shell.Current.GoToAsync("//gymlogin");
                }
                else if (accountId == "" || accountId == "null")
                {
                    await Shell.Current.GoToAsync("//gymlogin");
                }
                else
                {
                    await Shell.Current.GoToAsync("//accounthome");
                }
            }
            else if (action == "loadgyms")
            {
                await Shell.Current.GoToAsync("//gymlistings");
            }
            else if (action == "loadgymscountries")
            {
                await Shell.Current.GoToAsync("//gymlistings");
            }
            else if (action == "login")
            {
                AccountMobile account = null;
                if (Application.Current.Properties.ContainsKey("account"))
                {
                    account = (AccountMobile)Application.Current.Properties["account"];
                }
                List<AccountMobile> accs = (List<AccountMobile>)Application.Current.Properties["accounts"];
                if (accs.Count > 1)
                {
                    await Shell.Current.GoToAsync("//gymmultiple");
                }
                else if (account == null)
                {
                    Xamarin.Essentials.Preferences.Set("error", "LoginNotVerified");
                    await Shell.Current.GoToAsync("//gymlogin");
                }
                else
                {
                    await Shell.Current.GoToAsync("//accounthome");
                }
            }
            else if (action == "cancelclass")
            {
                await Shell.Current.GoToAsync("//accountupcomingvisits");
            }
            else if (action == "markabsent")
            {
                string source = Xamarin.Essentials.Preferences.Get("source", "");
                await Shell.Current.GoToAsync("//" + source);
            }
            else if (action == "unmarkabsent")
            {
                string source = Xamarin.Essentials.Preferences.Get("source", "");
                await Shell.Current.GoToAsync("//" + source);
            }
            else if (action == "canceltrial" || action == "cancelenroll")
            {
                await Shell.Current.GoToAsync("//accountschedulechild");
            }
            else if (action == "freeze")
            {
                await Shell.Current.GoToAsync("//enrollfreezeconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "accountupdate")
            {
                await Shell.Current.GoToAsync("//accountprofileprofile");
            }
            else if (action == "accountupdatechild")
            {
                await Shell.Current.GoToAsync("//accountprofilechildren");
            }
            else if (action == "accounttranspayments")
            {
                await Shell.Current.GoToAsync("//accounttranspayments");
            }
            else if (action == "deleteaccount")
            {
                Application.Current.Properties.Clear();
                Xamarin.Essentials.Preferences.Set("gymid", "");
                Xamarin.Essentials.Preferences.Set("accountid", "");
                Xamarin.Essentials.Preferences.Set("zip", "");
                Xamarin.Essentials.Preferences.Set("country", "");
                await Shell.Current.GoToAsync("//gymlogin");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "readcamps")
            {
                await Shell.Current.GoToAsync("//gymcamps");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "readparty")
            {
                await Shell.Current.GoToAsync("//partypackages");
            }
            else if (action == "readpartydates")
            {
                await Shell.Current.Navigation.PushAsync(new PartyDate());
            }
            else if (action == "bookparty")
            {
                await Shell.Current.GoToAsync("//partyconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "returntopartypackages")
            {
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("//partypackages");
            }
            else if (action == "enroll" || action == "trial" || action == "single")
            {
                await Shell.Current.GoToAsync("//enrollconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "switch")
            {
                await Shell.Current.GoToAsync("//accountschedulechild");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "upgradeunlimited" || action == "degradeunlimited")
            {
                await Shell.Current.GoToAsync("//accountschedulechild");
            }
            else if (action == "enrollcamp")
            {
                await Shell.Current.GoToAsync("//eventconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "contact")
            {
                await Shell.Current.GoToAsync("//gymcontactconfirm");
            }
            else if (action == "readclasscardpackages")
            {
                await Shell.Current.GoToAsync("//classcardpackages");
            }
            else if (action == "classcard")
            {
                await Shell.Current.GoToAsync("//classcardconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else if (action == "purchasesocks")
            {
                await Shell.Current.GoToAsync("//accountsocksconfirm");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                await Shell.Current.GoToAsync("//accounthome");
            }
            Xamarin.Essentials.Preferences.Set("action", "");
            Xamarin.Essentials.Preferences.Set("actionaux", "");
            Xamarin.Essentials.Preferences.Set("reset", "");
        }

        public void ClearNavigation()
        {
            IEnumerator<Page> existingPages = Shell.Current.Navigation.NavigationStack.GetEnumerator();
            while (existingPages.MoveNext())
            {
                Page p = existingPages.Current;
                if (p != null)
                {
                    Navigation.RemovePage(p);
                    existingPages = Shell.Current.Navigation.NavigationStack.GetEnumerator();
                }
            }
        }
    }
}