using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel;

namespace SRKSSynergy.Models
{
    public class MainDataBase
    {
        public MDBGeneralData MDBGeneralData { get; set; }

        public MDBContactPersonData MDBContactPersonData1 { get; set; }

        public MDBContactPersonData MDBContactPersonData2 { get; set; }

        public MDBContactPersonData MDBContactPersonData3 { get; set; }

        public MDBContactPersonData MDBContactPersonData4 { get; set; }

        public MDBContactPersonData MDBContactPersonData5 { get; set; }

        public MDBContactPersonData MDBContactPersonData6 { get; set; }

        public MDBStatutoryNumber MDBStatutoryNumber { get; set; }

        public MDBBankDetail MDBBankDetail1 { get; set; }

        public MDBBankDetail MDBBankDetail2 { get; set; }
    }

    public class QuickGenerateMDB
    {
        //public MDBGeneralData MDBGeneralData { get; set; }

        //public MDBContactPersonData MDBContactPersonData1 { get; set; }

        [Display(Name = "Customer Unique ID")]
        public string CompanyUniqueID { get; set; }

        [Required(ErrorMessage = "Organization Name is Required")]
        [Display(Name = "Organization Name")]
        //[RegularExpression(@"^[a-zA-Z0-9'' ']+$", ErrorMessage = "Special Characters are not allowed!")]
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

        //[Required(ErrorMessage = "City is Required")]
        [Display(Name = "City")]
        //[StringLength(20, MinimumLength = 3)]
        public string City { get; set; }

        //[Required(ErrorMessage = "Pincode is Required")]
        [Display(Name = "Pincode")]
        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(9, MinimumLength = 6)]
        public string Pincode { get; set; }

        //[Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
        //[StringLength(30, MinimumLength = 4)]
        public string State { get; set; }

        //[Required(ErrorMessage = "Country is Required")]
        //[StringLength(20, MinimumLength = 4)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(3, MinimumLength = 2)]
        public string Isd1 { get; set; }

        //[Required]
        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        //[Required]
        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(8, MinimumLength = 5, ErrorMessage = "Phone1 length Min is 5 and Max is 8")]
        public string PhoneLL1 { get; set; }

        [Display(Name = "Email address")]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

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

        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }

    [Table("MDBGeneralData")]
    public class MDBGeneralData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MDBID { get; set; }

        [Display(Name = "Customer Unique ID")]
        public string CompanyUniqueID { get; set; }

        //public string RelatedCompanyUniqueID { get; set; }

        //public string RelationshipType { get; set; }

        public string RelationshipTypeothers { get; set; }

        [Required(ErrorMessage = "Organization Name is Required")]
        [Display(Name = "Organization Name")]
        [RegularExpression(@"^[a-zA-Z0-9'' ''.'.]+$", ErrorMessage = "Special Characters are not allowed!")]
        //[StringLength(40, MinimumLength = 5)]
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Organization Type is Required")]
        [Display(Name = "Organization Type")]
        public string OrganizationType { get; set; }

        public string OrganizationTypeothers { get; set; }

        [Display(Name = "Search Customer")]
        //[StringLength(20, MinimumLength = 3)]
        public string SearchTerm { get; set; }

        //[Required(ErrorMessage = "Address Line1 is Required")]
        [Display(Name = "Address(Line1)")]
        //[StringLength(50, MinimumLength = 1)]
        public string AddressLine1 { get; set; }

        //[Required(ErrorMessage = "Address Line2 is Required")]
        [Display(Name = "Address(Line2)")]
        //[StringLength(50, MinimumLength = 1)]
        public string AddressLine2 { get; set; }

        [Display(Name = "Address(Line3)")]
        //[StringLength(50, MinimumLength = 4)]
        public string AddressLine3 { get; set; }

        [Display(Name = "Address(Line4)")]
        //[StringLength(50, MinimumLength = 4)]
        public string AddressLine4 { get; set; }

        //[Required(ErrorMessage = "City is Required")]
        [Display(Name = "City")]
        //[StringLength(20, MinimumLength = 1)]
        public string City { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[Required(ErrorMessage = "Pincode is Required")]
        [Display(Name = "Pincode")]
        [StringLength(6, MinimumLength = 6)]
        public string Pincode { get; set; }

        //[Required(ErrorMessage = "State is Required")]
        [Display(Name = "State")]
        //[StringLength(30, MinimumLength = 2)]
        public string State { get; set; }

        //[Required(ErrorMessage = "Country is Required")]
        //[StringLength(20, MinimumLength = 2)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        //[Required]
        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(3, MinimumLength = 2)]
        public string Isd1 { get; set; }

        //[Required]
        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        //[Required]
        //[RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL1 { get; set; }

        //[StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        public string Isd2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string ContactNumLL2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isdf { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Stdf { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        //[StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string FAX { get; set; }

        [Display(Name = "Email address")]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID2 { get; set; }

        [Display(Name = "Website")]
        //[StringLength(40, MinimumLength = 11)]
        public string Website { get; set; }

        [Display(Name = "Postal/Courier")]
        public string PostalCourier { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int CPID { get; set; }

        public int IsDeleted { get; set; }

        public string Address
        {
            get
            {
                return AddressLine1 + "\n" + AddressLine2 + "\n" + AddressLine3 + "\n" + AddressLine4;
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
                return Isd2 + "-" + Std2 + "-" + ContactNumLL2;
            }
        }

        public string ContactName { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        //public virtual ChannelPartners ChannelPartners { get; set; }

        public virtual ICollection<MDBBankDetail> MDBBankDetails { get; set; }

        public virtual ICollection<MDBContactPersonData> MDBContactPersonDatas { get; set; }

        public virtual ICollection<MDBStatutoryNumber> MDBStatutoryNumbers { get; set; }

        //New Added On 14-12-2016
        public string BillingAddress { get; set; }

        public string DeleveryAddress { get; set; }
    }

    [Table("MDBContactPersonData")]
    public class MDBContactPersonData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MDBCPDID { get; set; }

        public Nullable<int> MDBID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(15, MinimumLength = 2)]
        public string Titleothers { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(30, MinimumLength = 1)]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(30, MinimumLength = 1)]
        public string LastName { get; set; }

        [Display(Name = "Designation")]
        [StringLength(40, MinimumLength = 2)]
        public string Designation { get; set; }

        [Display(Name = "Department")]
        [StringLength(40, MinimumLength = 2)]
        public string Department { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isd1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isd2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2)]
        public string Isdm1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile length is 10")]
        public string Mobile1 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(5, MinimumLength = 2)]
        public string Isdm2 { get; set; }

        [RegularExpression("^([0-9]+$)", ErrorMessage = "Enter only Numbers")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile length is 10")]
        public string Mobile2 { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }

        [Display(Name = "Key Activity")]
        [StringLength(100, MinimumLength = 2)]
        public string KeyActivity { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        [StringLength(100, MinimumLength = 4)]
        public string Comments { get; set; }

        public virtual MDBGeneralData MDBGeneralData { get; set; }
    }

    [Table("MDBStatutoryNumber")]
    public class MDBStatutoryNumber
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MDBSNID { get; set; }

        public int MDBID { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Required(ErrorMessage = "Company PAN Number is Required")]
        [Display(Name = "Company PAN")]
        [StringLength(13, MinimumLength = 10)]
        public string CompanyPAN { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Required(ErrorMessage = "GST Number is Required")]
        [Display(Name = "GST")]
        [StringLength(20, MinimumLength = 11)]
        public string TIN { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Display(Name = "Registration Number")]
        [StringLength(20, MinimumLength = 5)]
        public string RegistrationNumber { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Display(Name = "Service Tax Number")]
        [StringLength(20, MinimumLength = 9)]
        public string ServiceTaxNumber { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Display(Name = "Importer Exporter Code")]
        [StringLength(20, MinimumLength = 5)]
        public string ImporterExporterCode { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Display(Name = "CST Number")]
        [StringLength(20, MinimumLength = 5)]
        public string CSTNumber { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Display(Name = "Tax Deduction Number")]
        [StringLength(20, MinimumLength = 5)]
        public string TaxDeductionAccountNumber { get; set; }

        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        [Display(Name = "Others")]
        [DataType(DataType.MultilineText)]
        [StringLength(40, MinimumLength = 5)]
        public string Others { get; set; }

        //public virtual MDBGeneralData MDBGeneralData { get; set; }
    }

    [Table("MDBBankDetail")]
    public class MDBBankDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MDBBDID { get; set; }

        public int MDBID { get; set; }

        [Display(Name = "Bank Name")]
        [StringLength(40, MinimumLength = 4)]
        public string BankName { get; set; }

        [Display(Name = "Branch Name")]
        [StringLength(40, MinimumLength = 4)]
        public string BranchName { get; set; }

        [Display(Name = "Account Type")]
        public string Accounttype { get; set; }

        [Display(Name = "Account Number")]
        [StringLength(30, MinimumLength = 7)]
        public string AccountNumber { get; set; }

        [Display(Name = "IFSC Code")]
        [StringLength(11, MinimumLength = 11)]
        public string IFSCCode { get; set; }

        [Display(Name = "Address(Line1)")]
        [StringLength(50, MinimumLength = 4)]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address(Line2)")]
        [StringLength(50, MinimumLength = 4)]
        public string AddressLine2 { get; set; }

        [Display(Name = "Address(Line3)")]
        [StringLength(50, MinimumLength = 4)]
        public string AddressLine3 { get; set; }

        [Display(Name = "City")]
        [StringLength(20, MinimumLength = 3)]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Pincode")]
        [StringLength(6, MinimumLength = 6)]
        public string PinCode { get; set; }

        [StringLength(20, MinimumLength = 4)]
        [Display(Name = "Country")]
        [DefaultValue("INDIA")]
        public string Country { get; set; }

        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isd1 { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std1 { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL1 { get; set; }

        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isd2 { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Std2 { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "Phone length Min is 5 and Max is 8")]
        public string PhoneLL2 { get; set; }

        [Display(Name = "Website")]
        [StringLength(40, MinimumLength = 11)]
        public string Website { get; set; }

        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [StringLength(3, MinimumLength = 2)]
        [DefaultValue(91)]
        public string Isdf { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "STD length Min is 2 and Max is 5")]
        public string Stdf { get; set; }

        [StringLength(8, MinimumLength = 5, ErrorMessage = "FAX length Min is 5 and Max is 8")]
        public string FAX { get; set; }

        [Display(Name = "Cheque in favor of")]
        [StringLength(100, MinimumLength = 5)]
        public string BankChequeinfavor { get; set; }

        //public virtual MDBGeneralData MDBGeneralData { get; set; }
    }
}