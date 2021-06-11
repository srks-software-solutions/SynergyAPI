using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace SRKSSynergy.Models
{
    public class MainChannelPartner
    {
        public ChannelPartners ChannelPartners { get; set; }
        public CPBankDetails CPBankDetails1 { get; set; }
        public CPBankDetails CPBankDetails2 { get; set; }
        public CPContactPersonData CPContactPersonData1 { get; set; }
        public CPContactPersonData CPContactPersonData2 { get; set; }
    }

    [Table("ChannelPartners")]
    public class ChannelPartners
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CPID { get; set; }

        [Display(Name = "Channel Partner Unique ID")]
        public string CPUniqueID { get; set; }

        [Required(ErrorMessage = "Channel Partner Name is Required")]
        [Display(Name = "Channel Partner Name")]
        [StringLength(40, MinimumLength = 5)]
        public string CPName { get; set; }

        [Display(Name = "Organization Type")]
        public string CPOrgType { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string CPOrgTypeothers { get; set; }

        [Required(ErrorMessage = "Address Line1 is Required")]
        [Display(Name = "Address(Line1)")]
        [StringLength(50, MinimumLength = 4)]
        public string AddressLine1 { get; set; }

        [Required(ErrorMessage = "Address Line2 is Required")]
        [Display(Name = "Address(Line2)")]
        [StringLength(50, MinimumLength = 4)]
        public string AddressLine2 { get; set; }

        [Display(Name = "Address(Line3)")]
        [StringLength(50, MinimumLength = 4)]
        public string AddressLine3 { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [Display(Name = "City")]
        [StringLength(20, MinimumLength = 3)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Pincode is Required")]
        [Display(Name = "Pincode")]
        [StringLength(6, MinimumLength = 6)]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Country is Required")]
        [StringLength(20, MinimumLength = 3)]
        [Display(Name = "Country")]
        [DefaultValue("INDIA")]
        public string Country { get; set; }

        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isd1 { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string ContactNumLL1 { get; set; }

        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isd2 { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std2 { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string ContactNumLL2 { get; set; }

        [Required(ErrorMessage = "TIN Number is Required")]
        [Display(Name = "TIN")]
        [StringLength(20, MinimumLength = 11)]
        public string TIN { get; set; }

        [Required(ErrorMessage = "Company PAN Number is Required")]
        [Display(Name = "Company PAN")]
        [StringLength(13, MinimumLength = 10)]
        public string CompanyPAN { get; set; }

        [Display(Name = "CST Number")]
        [StringLength(13, MinimumLength = 11)]
        public string CSTNumber { get; set; }

        [Display(Name = "Others")]
        [StringLength(30, MinimumLength = 4)]
        public string Others { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public Nullable<int> IsDeleted { get; set; }

        public string Logo { get; set; }

        //public HttpPostedFileBase PhotoUpload { get; set; }

        [Display(Name = "Website")]
        [StringLength(40, MinimumLength = 10)]
        public string Website { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter Valid email-id")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter Valid email-id")]
        [Display(Name = "Email")]
        public string Email2 { get; set; }

        //[Display(Name = "FAX")]
        //[StringLength(11, MinimumLength = 11)]
        //public string Fax { get; set; }

        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isdf { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Stdf { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "FAX length Min is 5 and Max is 8")]
        public string FAX { get; set; }

        [Required(ErrorMessage = "Postal/Courier is required")]
        [Display(Name = "Postal/Courier")]
        public string postcour { get; set; }

        public int ZoneID { get; set; }
        public virtual Zone Zone { get; set; }

        public string LandLine1
        {
            get
            {
                return Isd1 + "-" + Std1 + "-" + ContactNumLL1 ;
            }
        }

        public string LandLine2
        {
            get
            {
                return Isd2 + "-" + Std2 + "-" + ContactNumLL2;
            }
        }

        public string Address
        {
            get
            {
                return AddressLine1 + "\n" + AddressLine2 + "\n" + AddressLine3;
            }
        }

        public string footaddress
        {
            get
            {
                return CPName + " " + AddressLine1 + ", " + AddressLine2 + " " + AddressLine3 + ", " + City + " - " + PinCode;
            }
        }


        public virtual ICollection<CPBankDetails> CPBankDetails { get; set; }
        public virtual ICollection<CPContactPersonData> CPContactPersonData { get; set; }
    }

    [Table("CPBankDetails")]
    public class CPBankDetails
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CPBDID { get; set; }

        public int CPID { get; set; }

        [Display(Name = "Bank Name")]
        [StringLength(30, MinimumLength = 4)]
        public string BankName { get; set; }

        [Display(Name = "Branch Name")]
        [StringLength(30, MinimumLength = 4)]
        public string BranchName { get; set; }

        [Display(Name = "Account Type")]
        public string Accounttype { get; set; }

        [Display(Name = "Account Number")]
        [StringLength(20, MinimumLength = 8)]
        public string AccountNumber { get; set; }

        [Display(Name = "IFSC Code")]
        [StringLength(11, MinimumLength = 11)]
        public string IFSCCode { get; set; }

        [Display(Name = "Cheque in favor of")]
        [StringLength(40, MinimumLength = 4)]
        public string BankChequeinfavor { get; set; }

        public virtual ChannelPartners ChannelPartners { get; set; }
    }

    [Table("CPContactPersonData")]
    public class CPContactPersonData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CPCPDID { get; set; }

        public int CPID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(15, MinimumLength = 2)]
        public string Titleothers { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Middle Name Min is 1 & Max is 20")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Last Name Min is 1 & Max is 20")]
        public string LastName { get; set; }

        [Display(Name = "Designation")]
        [StringLength(20, MinimumLength = 2)]
        public string Designation { get; set; }

        [Display(Name = "Department")]
        [StringLength(20, MinimumLength = 2)]
        public string Department { get; set; }

        [StringLength(3, MinimumLength = 2)]
        public string Isd1 { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL1 { get; set; }

        [StringLength(3, MinimumLength = 2)]
        public string Isd2 { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std2 { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL2 { get; set; }

        [StringLength(5, MinimumLength = 2)]
        public string Isdm1 { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile length is 10")]
        public string Mobile1 { get; set; }

        [StringLength(5, MinimumLength = 2)]
        public string Isdm2 { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile length is 10")]
        public string Mobile2 { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter Valid email-id")]
        [Display(Name = "Email")]
        public string EmailID { get; set; }

        [Display(Name = "Key Activity")]
        [StringLength(20, MinimumLength = 2)]
        public string KeyActivity { get; set; }

        [Display(Name = "Comments")]
        [StringLength(50, MinimumLength = 2)]
        public string Comments { get; set; }

        public byte[] PHOTO { get; set; }

        public string Name
        {
            get
            {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }

        public string LandLine1
        {
            get
            {
                return Isd1 + "-" + Std1 + "-" + PhoneLL1;
            }
        }

        public string LandLine2
        {
            get
            {
                return Isd2 + "-" + Std2 + "-" + PhoneLL2;
            }
        }

        public string MobileNo1
        {
            get
            {
                return Isd1 + "-" + Mobile1;
            }
        }

        public string MobileNo2
        {
            get
            {
                return Isd2 + "-" + Mobile2;
            }
        }

        public virtual ChannelPartners ChannelPartners { get; set; }
    }
}