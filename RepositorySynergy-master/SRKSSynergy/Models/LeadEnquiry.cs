using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel;
using System.Data.SqlClient;

namespace SRKSSynergy.Models
{
    [Table("LeadEnquiry")]
    public class LeadEnquiry
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LEID { get; set; }

        [Required(ErrorMessage = "Organization Name is Required")]
        [Display(Name = "Organization Name")]
        [RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessage = "Special Characters are not allowed!")]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage ="Numbers and special characters are not allowed in the name.")]
        [StringLength(50, MinimumLength = 5)]
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Organization Type is Required")]
        [Display(Name = "Organization Type")]
        public string OrganizationType { get; set; }

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

        [Required(ErrorMessage = "Pincode is Required")]
        [Display(Name = "Pincode")]
        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(9, MinimumLength = 6)]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
       //[StringLength(30, MinimumLength = 4)]
        public string State { get; set; }

        [Required(ErrorMessage = "Country is Required")]
        [StringLength(20, MinimumLength = 4)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(3, MinimumLength = 2)]
        public string Isd1 { get; set; }

        [Required]
        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        [Required]
        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone1 length Min is 5 and Max is 8")]
        public string PhoneLL1 { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }

        [Display(Name = "Title")]
        public string Prefix { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(20, MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 1)]
        public string LastName { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isdc1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Stdc1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLLc1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2)]
        public string Isdm1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(10, MinimumLength = 10)]
        public string Mobile1 { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailIDContact { get; set; }


        //
        [Display(Name = "SuggestedModel")]
        public string SuggestedModel { get; set; }
       
        public Nullable<System.DateTime> DateOfMeeting { get; set; }

        [Display(Name = "LeadSource")]
        //[StringLength(30, MinimumLength = 4)]
        public string LeadSource { get; set; }

        public string MeetingDesc { get; set; }

        //
        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
        
        public int IsDeleted { get; set; }
        // 0 is open and No update and 1 is Closed and Moved to Quotation
        public int IsStatus { get; set; }

        public int IsDrop { get; set; }

        // 
        public int CPID { get; set; }
        //public virtual ChannelPartners ChannelPartners { get; set; }

        public String LeadTime { get; set; }

        public int IsTime { get; set; }

        public Nullable <int> IsCount { get; set; }

        public Nullable<int> IsHOD { get; set; }

        public Nullable<System.DateTime> NotifyDate { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }
}