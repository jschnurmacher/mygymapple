using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Telerik.XamarinForms.Input;

namespace mygymmobiledata
{
    [Serializable]
    public class EventMobile : ViewModelBase
    {
        public int EventId { get; set; }
        public string Display { get; set; }
        public string DescLong { get; set; }
        public string AgeDesc { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime StartMonth { get; set; }
        public DateTime EndMonth { get; set; }
        public string DateStr { get; set; }
        public string ListOfDates { get; set; }
        public int ClassTemplateId1 { get; set; }
        public bool AvailableBookings { get; set; }
        public bool LCF { get; set; }
        public bool IncludeSchedOnly { get; set; }
        public string EventDiscountsStr { get; set; }
        public EventCost EventCost { get; set; }
        public EventDiscountMobile EventDiscountMobile { get; set; }
        public ObservableCollection<EventItemMobile>  EventItems { get; set; }
        public ObservableCollection<EventDiscountMobile> EventDiscounts { get; set; }
        public ObservableCollection<EventDiscountMobile> EventDiscountsAdd { get; set; }
        public ObservableCollection<EventInstanceMobile> EventInstances { get; set; }
        public ObservableCollection<EventDateMobile> SelectedDates { get; set; }
        public ObservableCollection<EventDateMobile> EventDates { get; set; }
        [IgnoreDataMember]
        public ObservableCollection<Appointment> Appointments { get; set; }
        public string Book { get; set; }
        public bool BookVisible { get; set; }
        public bool BookingNotAvailable { get; set; }
        public bool CampEnrollmentNotAvailable { get; set; }
        public int GymId { get; set; }
        public int AccountId { get; set; }
        public int BillingId { get; set; }
        public string PromoCode { get; set; }
        public decimal StoreCredit { get; set; }
        public bool MembersOnly { get; set; }
        public bool EnrolledOnly { get; set; }
        public string Bullets { get; set; }
    }

    public class EventCost
    {
        public int EventId { get; set; }
        public string Desc { get; set; }
        public decimal Total { get; set; }
        public string TotalStr { get; set; }
        public decimal SubTotal { get; set; }
        public string SubTotalStr { get; set; }
        public decimal MembershipFee { get; set; }
        public string MembershipFeeStr { get; set; }
        public decimal Tax { get; set; }
        public string TaxStr { get; set; }
        public int Sessions { get; set; }
        public int Credits { get; set; }
        public int CreditsAvailable { get; set; }
        public int CreditsApplied { get; set; }
        public string DiscountSummary { get; set; }
        public bool IncludeMembership { get; set; }
        public decimal Credit { get; set; }
        public decimal SocksPrice { get; set; }
        public decimal SocksTax { get; set; }
    }

    public class EventInstanceMobile
    {
        public int ClassId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public bool IsFull { get; set; }
    }

    public class EventDateMobile
    {
        public int ClassId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateStart { get; set; }
        public string DateStr { get; set; }
        public string TimeStr { get; set; }
        public string SessionStr { get; set; }
        public int ChildId { get; set; }
    }

    public class EventItemMobile
    {
        public string Item { get; set; }
    }

    public class EventDiscountMobile
    {
        public int Id { get; set; }
        public string Desc { get; set; }
        public string DisplayList { get; set; }
    }

}
