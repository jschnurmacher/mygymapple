using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace mygymmobiledata
{
    public class PartyMobile
    {
        public ObservableCollection<PartyPackageMobile> PartyPackages { get; set; }
    }

    public class PartyPackageMobile
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public string Name { get; set; }
        public decimal Member { get; set; }
        public decimal NonMember { get; set; }
        public decimal Extra { get; set; }
        public int Max { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public int IdOrig { get; set; }
        public bool Online { get; set; }
        public bool Booking { get; set; }
        public int Order { get; set; }
        public string PackageOption { get; set; }
        public ObservableCollection<PartyCaptionMobile> BirthdayPackageCaptions { get; set; }
        public int BirthdayPackageCaptionsHeight { get; set; }
        public string Cost { get; set; }
        public string Choose { get; set; }
        public bool PartyBookingAvailable { get; set; }
    }

    public class PartyCaptionMobile
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public int BirthdayPackageId { get; set; }
        public string Caption { get; set; }
        public int CaptionOrder { get; set; }
        public ObservableCollection<PartyItemMobile> BirthdayCaptionItems { get; set; }
        public int BirthdayCaptionItemsHeight { get; set; }
    }

    public class PartyItemMobile
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public int BirthdayPackageCaptionId { get; set; }
        public string Item { get; set; }
        public int ItemOrder { get; set; }
    }

    public class PartyDatesMobile
    {
        public ObservableCollection<PartyMonthMobile> Months { get; set; }
    }

    public class PartyMonthMobile
    {
        public string Month { get; set; }
        public int MonthInt { get; set; }
        public ObservableCollection<PartyDateMobile> Dates { get; set; }
    }

    public class PartyDateMobile
    {
        public string Date { get; set; }
        public DateTime DateDate { get; set; }
        public ObservableCollection<PartyTimeMobile> Times { get; set; }
    }

    public class PartyTimeMobile
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Time { get; set; }
        public bool HalfHour { get; set; }
        public decimal HalfHourFee { get; set; }
    }

    public class PartyOptionsMobile : ViewModelBase
    {
        public ObservableCollection<PartyOptionMobile> PartyOptions { get; set; }
        public bool HalfHour { get; set; }
        public bool _halfHourchecked = false;
        public bool HalfHourChecked
        {
            get { return _halfHourchecked; }
            set { this.ChangeAndNotify(ref this._halfHourchecked, value, "HalfHourChecked"); }
        }
        public int NumKids {get; set;}
    }

    public class PartyOptionMobile : ViewModelBase
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public int VendorId { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public decimal Retail { get; set; }
        public int Quantity { get; set; }
        public int Reorder { get; set; }
        public bool Taxable { get; set; }
        public bool AddOn { get; set; }
        public string Theme { get; set; }
        public int IdOrig { get; set; }
        public bool Online { get; set; }
        public bool PerChild { get; set; }
        public int Include { get; set; }
        public string Display { get; set; }

        public bool _checked = false;
        public bool Checked
        {
            get { return _checked; }
            set { this.ChangeAndNotify(ref this._checked, value, "Checked"); }
        }
    }

    public class PartyBook
    {
        public int BillingId { get; set; }
        public int PackageId { get; set; }
        public List<int> Children { get; set; }
        public List<int> AddOns { get; set; }
        public bool HalfHour { get; set; }
        public PartyTimeMobile PartyTime { get; set; }
        public string PromoCode { get; set; }
    }
}