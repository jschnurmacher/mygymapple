using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace mygymmobiledata
{
    [Serializable]
    public class GymMobile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HomeUrl { get; set; }
        public string Culture { get; set; }
        public string Timezone { get; set; }
        public int TimezoneOffset { get; set; }
        public string TimezoneAlternate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string SMSText { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Directions { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Distance { get; set; }
        public string DropInLabel { get; set; }
        public int DropInMaxTotal { get; set; }
        public bool EnrollEnabled { get; set; }
        public bool GuestEnabled { get; set; }
        public bool GuestExpressEnabled { get; set; }
        public bool TrialEnabled { get; set; }
        public bool TrialNoPayment { get; set; }
        public bool TrialUrlEnabled { get; set; }
        public bool TrialUrlEnabledPreOpening { get; set; }
        public decimal TrialCost { get; set; }
        public int TrialWeeks { get; set; }
        public bool TrialUnlimited { get; set; }
        public bool TrialDropIn { get; set; }
        public bool TrialMonthsEnabled { get; set; }
        public int TrialMonths { get; set; }
        public bool MembershipType { get; set; }
        public decimal MembershipFee { get; set; }
        public decimal MembershipInitialFee { get; set; }
        public decimal MembershipPerChildFee { get; set; }
        public bool MembershipEnabled { get; set; }
        public bool UnlimitedEnroll { get; set; }
        public bool Unlimited { get; set; }
        public bool UnlimitedMode { get; set; }
        public System.DateTime UnlimitedStart { get; set; }
        public System.DateTime UnlimitedEnd { get; set; }
        public decimal UnlimitedFee { get; set; }
        public bool MakeupEnabled { get; set; }
        public bool ClassCardEnabled { get; set; }
        public bool EnrollCampEnabled { get; set; }
        public bool CampEnabled { get; set; }
        public bool EventEnabled { get; set; }
        public string ClassCardIndexDescription { get; set; }
        public string CampMenu { get; set; }
        public string CampTitle { get; set; }
        public string CampText1 { get; set; }
        public string CampText2 { get; set; }
        public bool CampVideo { get; set; }
        public string EventMenu { get; set; }
        public string EventTitle { get; set; }
        public bool CancelEnabled { get; set; }
        public bool FreezeEnabled { get; set; }
        public decimal FreezeFee { get; set; }
        public bool MoveEnabled { get; set; }
        public bool AllowStoreCreditOnWebsite { get; set; }
        public bool OfferUnlimitedOnline { get; set; }
        public bool EnableOnlineAbsences { get; set; }
        public int EnableOnlineAbsencesWeeks { get; set; }
        public bool EnableOnlinePartyAddOns { get; set; }
        public int OnlinePartyAddOnBufferDays { get; set; }
        public decimal BirthdayDeposit { get; set; }
        public bool HideTransactionTab { get; set; }
        public int BirthdayMaxAge { get; set; }
        public decimal ClassTax { get; set; }
        public decimal EventTax { get; set; }
        public decimal BirthdayTax { get; set; }
        public decimal MembershipTax { get; set; }
        public decimal ProductTax { get; set; }
        public int MaxBirthdayKids { get; set; }
        public bool ShowBirthdayNumKids { get; set; }
        public bool WaiverEnabled { get; set; }
        public bool NoShowAlert { get; set; }
        public bool BirthdayEnabled { get; set; }
        public int MinimumFreezeWeeks { get; set; }
        public int MinimumFreezeMonths { get; set; }
        public bool MinimumFreezeMonthsEnabled { get; set; }
        public string UnlimitedLabel { get; set; }
        public bool ShowSocksOnCheckout { get; set; }
        public string ShowSocksText { get; set; }
        public int ShowSocksAgeMin { get; set; }
        public decimal SocksPrice { get; set; }
        public decimal SocksTax { get; set; }
        public string TrialNoPaymentText { get; set; }
        public int BirthdayDays { get; set; }
        public bool ShowSocksAlert { get; set; }
    }

    [Serializable]
    public class GymsMobile
    {
        public ObservableCollection<GymMobile> gyms { get; set; }
    }

    public class ScheduleViewMobile
    {
        public ObservableCollection<ClassListView_ResultMobile> ClassList { get; set; }
    }

    [Serializable()]
    public partial class ClassListView_ResultMobile : ViewModelBase
    {
        public string Display { get; set; }
        public string Category { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public string ClassNameUrl { get; set; }
        public string ClassImage { get; set; }
        public int ClassTemplateId { get; set; }
        public string Photo { get; set; }
        public string AgeDesc { get; set; }
        public string Desc { get; set; }
        private string descShort = "";
        public string DescShort
        {
            get { return descShort; }
            set { this.ChangeAndNotify(ref this.descShort, value, "DescShort"); }
        }
        private string descLong = "";
        public string DescLong
        {
            get { return descLong; }
            set { this.ChangeAndNotify(ref this.descLong, value, "DescLong"); }
        }
        public decimal Cost { get; set; }
        public decimal CostTemplate { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ObservableCollection<CustomListItemMobile> TimesListDisplay { get; set; }
        public int TimesListHeight { get; set; }
        public ObservableCollection<CustomListItemTimeMobile> TimesList { get; set; }
        public ObservableCollection<ClassView_ResultMobile> Classes { get; set; }
        public ObservableCollection<CustomListItemMobile> ClassDates { get; set; }
        public ObservableCollection<CustomListItemMobile> Sessions { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool chooseTime = false;
        public bool ChooseTime
        {
            get { return chooseTime; }
            set { this.ChangeAndNotify(ref this.chooseTime, value, "ChooseTime"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool chooseDate = false;
        public bool ChooseDate
        {
            get { return chooseDate; }
            set { this.ChangeAndNotify(ref this.chooseDate, value, "ChooseDate"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool programFull1 = false;
        public bool ProgramFull1
        {
            get { return programFull1; }
            set { this.ChangeAndNotify(ref this.programFull1, value, "ProgramFull1"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool programFull2 = false;
        public bool ProgramFull2
        {
            get { return programFull2; }
            set { this.ChangeAndNotify(ref this.programFull1, value, "ProgramFull2"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool membersOnly1 = false;
        public bool MembersOnly1
        {
            get { return membersOnly1; }
            set { this.ChangeAndNotify(ref this.membersOnly1, value, "MembersOnly1"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool membersOnly2 = false;
        public bool MembersOnly2
        {
            get { return membersOnly2; }
            set { this.ChangeAndNotify(ref this.membersOnly2, value, "MembersOnly2"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool membersOnly3 = false;
        public bool MembersOnly3
        {
            get { return membersOnly3; }
            set { this.ChangeAndNotify(ref this.membersOnly3, value, "MembersOnly3"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool Contact { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private int buttonsHeight = 0;
        public int ButtonsHeight
        {
            get { return buttonsHeight; }
            set { this.ChangeAndNotify(ref this.buttonsHeight, value, "ButtonsHeight"); }
        }
        private ObservableCollection<CustomListItemMobile> buttons = null;
        public ObservableCollection<CustomListItemMobile> Buttons
        {
            get { return buttons; }
            set { this.ChangeAndNotify(ref this.buttons, value, "Buttons"); }
        }
    }

    public partial class ClassView_ResultMobile
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public int ClassTemplateId { get; set; }
        public int ClassTemplateId1 { get; set; }
        public int ClassTemplateId2 { get; set; }
        public bool ClassTemplateStandard { get; set; }
        public int RoomId { get; set; }
        public string Display { get; set; }
        public string DisplayFull { get; set; }
        public string ClassNameUrl { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public string AgeDesc { get; set; }
        public int Max { get; set; }
        public System.DateTime Start { get; set; }
        public System.DateTime End { get; set; }
        public string StartStr { get; set; }
        public string EndStr { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public int DayOfWeek { get; set; }
        public int Hours { get; set; }
        public int Mins { get; set; }
        public decimal Cost { get; set; }
        public decimal CostTemplate { get; set; }
        public decimal Deposit { get; set; }
        public System.DateTime StartFuture { get; set; }
        public System.DateTime EndFuture { get; set; }
        public System.DateTime StartTimeFuture { get; set; }
        public System.DateTime EndTimeFuture { get; set; }
        public decimal CostFuture { get; set; }
        public bool Online { get; set; }
        public bool Booking { get; set; }
        public bool BookingTrial { get; set; }
        public bool BookingGuest { get; set; }
        public bool BookingMakeup { get; set; }
        public bool BookingUnlimited { get; set; }
        public bool BookingDropIn { get; set; }
        public bool BookingCard { get; set; }
        public int BookingBuffer { get; set; }
        public int Weeks { get; set; }
        public int Weeks2 { get; set; }
        public string Color { get; set; }
        public string Photo { get; set; }
        public string Video { get; set; }
        public string Desc { get; set; }
        public string DescShort { get; set; }
        public string DescLong { get; set; }
        public int Enrolled { get; set; }
        public int Reserved { get; set; }
        public int Wait { get; set; }
        public int Guest { get; set; }
        public int Makeup { get; set; }
        public int DropIns { get; set; }
        public int Absent { get; set; }
        public int Frozen { get; set; }
        public int EnrolledHistory { get; set; }
        public string DateTitle { get; set; }
        public string Events { get; set; }
        public string EventsClosed { get; set; }
        public string Date { get; set; }
        public string DateJS { get; set; }
        public int StaffIdLead { get; set; }
        public int StaffIdAssist1 { get; set; }
        public int StaffIdAssist2 { get; set; }
        public string Room { get; set; }
        public int Type { get; set; }
        public bool EventType { get; set; }
        public int WeekDay { get; set; }
        public bool PartialDates { get; set; }
        public int ClassesPlanned { get; set; }
        public int ClassesHeld { get; set; }
        public int StaffId { get; set; }
        public string Waiver { get; set; }
        public string Description { get; set; }
        public int LeadQuality { get; set; }
        public string Notes { get; set; }
        public bool MembersOnly { get; set; }
        public bool EnrolledOnly { get; set; }
        public bool DropIn { get; set; }

        public int Tab { get; set; }
        public int StartDay { get; set; }
        public List<DateTime> ClassDates { get; set; }
        public List<ClassButtonMobile> ClassButtons { get; set; }
        public int Attendance { get; set; }
    }

    [Serializable]
    public class ClassButtonMobile
    {
        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public string DayStr { get; set; }
        public string TimeStr { get; set; }
        public string MobileStr { get; set; }
        public int Attendance { get; set; }
        public int Max { get; set; }
        public int DayOfWeek { get; set; }
        public int ClassId { get; set; }
        public bool Closed { get; set; }
    }

    [Serializable]
    public class ClassCardMobile
    {
        public ObservableCollection<ClassCardPackageMobile> ClassCardPackages { get; set; }
    }

    [Serializable]
    public class ClassCardPackageMobile
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int Credits { get; set; }
        public int ValidDays { get; set; }
        public bool Online { get; set; }
        public bool MemberRequired { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string Ages { get; set; }
        public string AmountStr { get; set; }
        public string CreditsStr { get; set; }
        public string ValidDaysStr { get; set; }
        public decimal Tax { get; set; }
    }
}
