using System;
using System.Collections.ObjectModel;

namespace mygymmobiledata
{
    [Serializable]
    public class AccountMobile : ViewModelBase
    {
        public int AccountId { get; set; }
        public int GymId { get; set; }
        public string First { get; set; }
        public string First2 { get; set; }
        public string Last { get; set; }
        public string Last2 { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Phone1 { get; set; }
        public int Phone1Type { get; set; }
        public string Phone1Ext { get; set; }
        public string Phone3 { get; set; }
        public int Phone3Type { get; set; }
        public string Phone3Ext { get; set; }
        public string Address { get; set; }
        public string Apt { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string BillingAddress { get; set; }
        public string BillingApt { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public int CCType { get; set; }
        public byte[] CCNum { get; set; }
        public int CCExpYear { get; set; }
        public int CCExpMonth { get; set; }
        public decimal StoreCredit { get; set; }
        public string ContactFirst { get; set; }
        public string ContactLast { get; set; }
        public int ContactRelationshipId { get; set; }
        public int ContactedById { get; set; }
        public string Chaperone { get; set; }
        public int ChaperoneRelationshipId { get; set; }
        public int HowHeardId { get; set; }
        public string Interest { get; set; }
        public string Password { get; set; }
        public int NumClassCards { get; set; }
        public bool Marketing { get; set; }
        public bool TextOptIn { get; set; }
        public bool Unsubscribed { get; set; }
        public bool Waiver { get; set; }
        public bool Overdue { get; set; }
        public string SignatureWaiver { get; set; }
        public string SignatureWaivera { get; set; }
        public string SignatureWaiverb { get; set; }
        public string Parent1 { get; set; }
        public string Parent2 { get; set; }
        public string ChaperoneWaiver { get; set; }
        public ObservableCollection<ChildMobile> Children { get; set; }
        public string ErrorMessage { get; set; }
        private ObservableCollection<AccountBillingMobile> billing;
        public ObservableCollection<AccountBillingMobile> Billing
        {
            get { return billing; }
            set { this.ChangeAndNotify(ref this.billing, value, "Billing"); }
        }
        public ObservableCollection<AccountCampCard> CampCards { get; set; }
        public ObservableCollection<AccountCampCard> ClassCards { get; set; }
        public ObservableCollection<AccountParty> Parties { get; set; }
        public ObservableCollection<NoShowAlertMobile> NoShowAlerts { get; set; }
    }

    [Serializable]
    public class AccountBillingMobile : ViewModelBase
    {
        public int Id { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public string BillingName { get; set; }
        public string BillingAddress { get; set; }
        public string BillingEmail { get; set; }
        public string BillingApt { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public int CCType { get; set; }
        public byte[] CCNum { get; set; }
        public int CCExpYear { get; set; }
        public int CCExpMonth { get; set; }
        public string CCExpMonthStr { get; set; }
        public string CardDesc { get; set; }
        public decimal StoreCredit { get; set; }
        public string BillingCityStateZip { get; set; }
        public string BillingFullAddress { get; set; }
        public bool Checked { get; set; }
        public bool Delete { get; set; }
        public bool MakePrimary { get; set; }
        public ObservableCollection<AccountBillingMobile> Billings { get; set; }
    }

    [Serializable]
    public class TransactionMobile
    {
        public int Id { get; set; }
        public string TransDate { get; set; }
        public string EntityTypeDesc { get; set; }
        public string Amount { get; set; }
    }

    [Serializable]
    public class ChildMobile
    {
        public int ChildId { get; set; }
        public int GymId { get; set; }
        public int AccountId { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public DateTime DOB { get; set; }
        public int Gender { get; set; }
        public string Allergy { get; set; }
        public string SpecialNeeds { get; set; }
        public string AgeStr { get; set; }
        public int Age { get; set; }
        public bool Member { get; set; }
        public ObservableCollection<EnrollMobile> Enrolls { get; set; }
        public bool EnrollNow { get; set; }
        public bool ScheduleClasses { get; set; }
        public bool ScheduleGuest { get; set; }
        public bool ScheduleEnroll { get; set; }
        public bool ScheduleTrial { get; set; }
        public bool ScheduleDropIn { get; set; }
        public bool ScheduleUnlimited { get; set; }
        public bool ScheduleMakeup { get; set; }
        public int NumMakeupsAvailable { get; set; }
        public bool VIMANotSigned { get; set; }
        public string UnlimitedAvailable { get; set; }
        public string MakeupsAvailable { get; set; }
        public string DropInAvailable { get; set; }
        public string VIMANotSignedMessage { get; set; }
        public string VIMAColor { get; set; }
        public bool AllowUnlimited { get; set; }
        public bool ChildEnabled { get; set; }
        public bool ChildNotEnabled { get; set; }
        public bool ChildNotEnabledMember { get; set; }
        public bool ChildNotEnabledEnrolled{ get; set; }
        public bool DropInMax { get; set; }
        public bool UnlimitedMax { get; set; }
        public decimal SocksPrice { get; set; }
        public decimal SocksTax { get; set; }
        public bool SocksPurchased { get; set; }
        public bool SocksReceived { get; set; }
        public bool IncludeSocks { get; set; }
        public string SocksText { get; set; }
        public string SocksReceivedOrPurchasedText { get; set; }
    }

    [Serializable]
    public class EnrollMobile : ViewModelBase
    {
        public int EnrollId { get; set; }
        public int BirthdayId { get; set; }
        public string Invitation { get; set; }
        public int Aux { get; set; }
        public int ClassId { get; set; }
        public int EnrollMoveToId { get; set; }
        public int ClassTemplateId { get; set; }
        public string ChildName { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Display { get; set; }
        public string DisplayClass { get; set; }
        public DateTime Start { get; set; }
        public DateTime EndReal { get; set; }
        public string FirstClass { get; set; }
        public string LastClass { get; set; }
        public string NextClass { get; set; }
        public string TrialDates { get; set; }
        public string TrialConvert { get; set; }
        public bool TrialConvertVisible { get; set; }
        public bool TrialCancelledVisible { get; set; }
        public bool FirstClassVisible { get; set; }
        public bool LastClassVisible { get; set; }
        public bool TrialDatesVisible { get; set; }
        public bool Unlimited { get; set; }
        public bool ViewVIMA { get; set; }
        public bool SignVIMA { get; set; }
        public string SignatureVIMA { get; set; }
        public string SignatureVIMAa { get; set; }
        public string SignatureVIMAb { get; set; }
        public string Parent1 { get; set; }
        public string Parent2 { get; set; }
        public string Chaperone { get; set; }
        public bool SignatureVIMAVisible { get; set; }
        public bool SignatureVIMAaVisible { get; set; }
        public bool SignatureVIMAbVisible { get; set; }
        public bool ViewLiability { get; set; }
        public bool CancelClass { get; set; }
        public bool CancelEnrollment { get; set; }
        public bool FreezeEnrollment { get; set; }
        public bool UnFreezeEnrollment { get; set; }
        public string FreezeEnrollmentStr { get; set; }
        public bool UpgradeUnlimited { get; set; }
        public bool RemoveUnlimited { get; set; }
        public bool MarkAbsent { get; set; }
        public bool UnMarkAbsent { get; set; }
        public bool SwitchClass { get; set; }
        public bool ScheduleClasses { get; set; }
        public bool PartyAddOns { get; set; }
        public bool IsParty { get; set; }
        public string Absences { get; set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool absencesExists = false;
        public bool AbsencesExists
        {
            get { return absencesExists; }
            set { this.ChangeAndNotify(ref this.absencesExists, value, "AbsencesExists"); }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool Overdue { get; set; }
        public bool ContinueToScheduleVisible { get; set; }
        public string ContinueToSchedule { get; set; }
        public bool FreezeUnavailableVisible { get; set; }
        public bool FreezeDeny { get; set; }
        public string UpgradeUnlimitedText { get; set; }
        public string RemoveUnlimitedText { get; set; }
        public string UnlimitedEnrollmentText { get; set; }
    }

    [Serializable]
    public class AccountCampCard
    {
        public int ClassTemplateId { get; set; }
        public DateTime Start { get; set; }
        public string StartStr { get; set; }
        public DateTime End { get; set; }
        public string EndStr { get; set; }
        public string StartEndStr { get; set; }
        public int Sessions { get; set; }
        public DateTime Created { get; set; }
        public int SessionsCreated { get; set; }
        public int StaffId { get; set; }
        public string Display { get; set; }
        public string SessionsStr { get; set; }
    }

    [Serializable]
    public class AccountParty
    {
        public int PartyId { get; set; }
        public string Children { get; set; }
        public string Package { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Booked { get; set; }
        public string Invitation { get; set; }
        public bool PartyAddOnsVisible { get; set; }
    }

    [Serializable]
    public class EnrollAttendMobile
    {
        public int EnrollAttendId { get; set; }
        public int EnrollId { get; set; }
        public int ChildId { get; set; }
        public DateTime Date { get; set; }
    }

    [Serializable]
    public class WaiverMobile
    {
        public string Waiver { get; set; }
        public bool WaiverOnFile { get; set; }
    }

    [Serializable]
    public class NoShowAlertMobile
    {
        public int EnrollId { get; set; }
        public string ChildFirst { get; set; }
        public string Class { get; set; }
        public string DateStr { get; set; }
        public DateTime Date { get; set; }
        public string Desc { get; set; }
    }
}
