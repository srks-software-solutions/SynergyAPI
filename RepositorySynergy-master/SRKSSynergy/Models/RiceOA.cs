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
    public class RiceOA
    {
        public RiceOAEquipPayment RiceOAEquipPayment { get; set; }

        public RiceOAEquipGeneralData RiceOAEquipGeneralData { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData1 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData2 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData3 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData4 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData5 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData6 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData7 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData8 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData9 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData10 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData11 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData12 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData13 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData14 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData15 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData16 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData17 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData18 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData19 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData20 { get; set; }

        public RiceOAEquipTableData RiceOAEquipTableData21 { get; set; }
    }

    [Table("RiceOAEquipGeneralData")]
    public class RiceOAEquipGeneralData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ROAID { get; set; }

        public int RQGID { get; set; }
        [Display(Name = "Quotation Number")]
        public string QuotationNumber { get; set; }

        [Display(Name = "Order Acknowledgement Number")]
        public string OANumber { get; set; }
        public string CPQuotationNumber { get; set; }

        [Required(ErrorMessage = "Salutation is Required")]
        [Display(Name = "Salutation")]
        [StringLength(320, MinimumLength = 4)]
        public string KindAttention { get; set; }

        [Required(ErrorMessage = "Subject is Required")]
        [Display(Name = "Subject")]
        [StringLength(160, MinimumLength = 4)]
        public string Subjectinfo { get; set; }

        public string CompanyUniqueID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //public string QuotationDate { get; set; }
        public Nullable<System.DateTime> QuotationDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //public string OADate { get; set; }
        public Nullable<System.DateTime> OADate { get; set; }

        public int MDBID { get; set; }
        public virtual MDBGeneralData MDBGeneralData { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int Islatest { get; set; }

        public int OAStatus { get; set; }

        [Display(Name = "Rejection Comment")]
        //[StringLength(160, MinimumLength = 4, ErrorMessage = "Minimum Length should be 4 and Maximum is 160")]
        //[StringLength(160)]
        public string OARejectComm { get; set; }

        public int ApprovalStatus { get; set; }

        public string Approvaldate { get; set; }

        public int IsDispatch { get; set; }

        public string MailDetails { get; set; }

        public string MailDate { get; set; }

        public string SalesManager { get; set; }

        public string Business { get; set; }

        public string BusinessArea { get; set; }

        public string BusinessUnit { get; set; }

        public string BusinessUnitForSAP { get; set; }

        public string MarketSegment { get; set; }

        public string CustomerSAPIdNo { get; set; }
        public string CustomerSAPIdDelvryNo { get; set; }

        public string PackingAndForwarding { get; set; }

        public string TransistInsurance { get; set; }

        public string Freight { get; set; }

        public string IncoTerms { get; set; }

        public string commitedDelivery { get; set; }

         [DefaultValue("Advance Payment \n For Cheque Transfer --> Cheque No. / Date / Bank Name / Amount \n For RTGS / NEFT / UTR / TT --> Transfer No. / Date / Amount \n Balance Agreed Payment terms \n XX% against proforma prior to dispatch \n L/C / Bank Guarantee")]
        public string PaymentTerms { get; set; }

        public int CPID { get; set; }

        public string CPName { get; set; }

        public virtual ChannelPartners ChannelPartners { get; set; }

        public virtual ICollection<RiceOAEquipPayment> RiceOAEquipPayment { get; set; }

        public virtual ICollection<RiceOAEquipTableData> RiceOAEquipTableData { get; set; }

        [Display(Name = "TIN")]
        [StringLength(20, MinimumLength = 11)]
        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        public string TinNumber { get; set; }

        //new Added On 07-12-2016

        //[RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        //[Required(ErrorMessage = "PAN Number is Required")]
        //[Display(Name = "PAN Number")]
        //[StringLength(13, MinimumLength = 10)]




        public string PANCardNo { get; set; }

        public string ProductVariety { get; set; }

        // Not required bcoz stored in Table data // need to delete Later // 07-12-2016
        public string TypeRice { get; set; } // also known as Process

        public string PaddySize { get; set; } // also known as Grain Type

        public string Pass { get; set; }

        public string Capacity { get; set; }

        public string PolishRequirement { get; set; }

        public string MotorQ { get; set; }

        public string MotorType { get; set; }

        public string MotorRating { get; set; }

        //public int IsMacineDispatch { get; set; }

        public virtual QGEquipTableData QGEquipTableData { get; set; }

        public virtual QGEquipGeneralData QGEquipGeneralData { get; set; }
        //public int IsHOD { get; set; }

    }

    [Table("RiceOAReportDBSheet")]
    public class RiceOAReportDBSheet
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ReportROAID { get; set; }
        public int ROAID { get; set; }

        public int RQGID { get; set; }
        public string QuotationNumber { get; set; }

        public string OANumber { get; set; }
        public string CPQuotationNumber { get; set; }


        public string KindAttention { get; set; }

        public string Subjectinfo { get; set; }

        public string CompanyUniqueID { get; set; }

        public Nullable<System.DateTime> QuotationDate { get; set; }

        public Nullable<System.DateTime> OADate { get; set; }

        public int MDBID { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int Islatest { get; set; }

        public int OAStatus { get; set; }

        public string OARejectComm { get; set; }

        public int ApprovalStatus { get; set; }

        public string Approvaldate { get; set; }

        public int IsDispatch { get; set; }

        public string MailDetails { get; set; }

        public string MailDate { get; set; }

        public string SalesManager { get; set; }

        public string Business { get; set; }

        public string BusinessArea { get; set; }

        public string BusinessUnit { get; set; }

        public string BusinessUnitForSAP { get; set; }

        public string MarketSegment { get; set; }

        public string CustomerSAPIdNo { get; set; }

        public string CustomerSAPIdDelvryNo { get; set; }

        public string PackingAndForwarding { get; set; }

        public string TransistInsurance { get; set; }

        public string Freight { get; set; }

        public string IncoTerms { get; set; }

        public string commitedDelivery { get; set; }

        public string PaymentTerms { get; set; }

        public int CPID { get; set; }

        public string CPName { get; set; }

        public string TinNumber { get; set; }

        public string PANCardNo { get; set; }

        public string ProductVariety { get; set; }

        public string TypeRice { get; set; } // also known as Process

        public string PaddySize { get; set; } // also known as Grain Type

        public string Pass { get; set; }

        public string Capacity { get; set; }

        public string PolishRequirement { get; set; }

        public string MotorQ { get; set; }

        public string MotorType { get; set; }

        public string MotorRating { get; set; }
        public string billingaddress { get; set; }
        public string deliveraddress { get; set; }
        public string customercontactperson {get;set;}
        public string customertelephonenumber {get;set;}
        public string customerfaxno {get;set;}
        public string customermobilenumber {get;set;}
        public string customermailaddress { get; set; }
        public string comments { get; set; }
        public string Price { get; set; }

    }

    [Table("RiceOAReportDataTable")]
    public class RiceOAReportDataTable
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ReportTableID { get; set; }

        public string Slno { get; set; }
        public string Machine { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string TotalPrice { get; set; }
    }

    [Table("RiceOAEquipPayment")]
    public class RiceOAEquipPayment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ROAPID { get; set; }

        public int ROAID { get; set; }

        [Required(ErrorMessage = "Payment Terms is Required")]
        [Display(Name = "Payment Terms")]
        [StringLength(100, MinimumLength = 7)]
        [DefaultValue("30% advance & balance 70% against pro-forma invoice before dispatch")]
        public string PaymentTerms { get; set; }

        [Required(ErrorMessage = "Delivery is Required")]
        [Display(Name = "Delivery")]
        [StringLength(50, MinimumLength = 7)]
        [DefaultValue("FCA, Bangalore")]
        public string Delivery { get; set; }

        [Required(ErrorMessage = "Date of Dispatch is Required")]
        [Display(Name = "Date of Dispatch")]
        [StringLength(50, MinimumLength = 7)]
        [DefaultValue("8 - 10 weeks after receipt of advance payment")]
        public string DateofDispatch { get; set; }

        [Required(ErrorMessage = "Transport is Required")]
        [Display(Name = "Transport")]
        [StringLength(20, MinimumLength = 4)]
        [DefaultValue("By Road")]
        public string Transport { get; set; }

        [Required(ErrorMessage = "Frieght is Required")]
        [Display(Name = "Frieght")]
        [StringLength(30, MinimumLength = 4)]
        [DefaultValue("To Pay basis")]
        public string Freight { get; set; }

        [Required(ErrorMessage = "CST is Required")]
        [Display(Name = "CST")]
        [StringLength(400, MinimumLength = 4)]
        [DefaultValue("CST extra @ 2% against Form C or as applicable at the time of invoicing, to be issued within one month from the end of each quarter of dispatch for dispatch within 30th June 2017. GST (including IGST & SGST) would be charged extra for any dispatch beyond 30th June 2017 as applicable at rate as specified in the GST / IGST / SGST ACT")]
        public string CST { get; set; }

        [Required(ErrorMessage = "Transsit Insurance is Required")]
        [Display(Name = "Transit Insurance")]
        [StringLength(30, MinimumLength = 4)]
        [DefaultValue("Extra as applicable")]
        public string TransitInsu { get; set; }

        [Required(ErrorMessage = "Commodity is Required")]
        [Display(Name = "Commodity")]
        [StringLength(20, MinimumLength = 3)]
        [DefaultValue("Rice")]
        public string Commodity { get; set; }

        [StringLength(320, MinimumLength = 10, ErrorMessage = "Minimum Length should be 4 and Maximum is 320")]
        public string annexure { get; set; }

        public string overallprice { get; set; }

        [Required(ErrorMessage = "Validity is Required")]
        [Display(Name = "Validity")]
        [StringLength(70, MinimumLength = 4)]
        [DefaultValue("2 months from the date of this offer")]
        public string Validity { get; set; }

        //public virtual RiceOAEquipGeneralData RiceOAEquipGeneralData { get; set; }

    }

    [Table("RiceOAEquipTableData")]
    public class RiceOAEquipTableData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ROATBID { get; set; }

        public int ROAID { get; set; }

        public string ModelNum { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }

        public string TotalPrice { get; set; }

        public string ModelDesc { get; set; }

        [Display(Name = "Exclusion")]
        [StringLength(160, MinimumLength = 4)]
        [DefaultValue("Air compressor, dryer, machine stand, cabin room,  hopper, air dryer, C.V Transformer, dust suction fan, AC and Bucket elevators.")]
        public string Exclusion { get; set; }

        public int ProductModelID { get; set; }

        public int ProductID { get; set; }

        public int MasterProductID { get; set; }

        public string MasterProductName { get; set; }

        public string ProductName { get; set; }

        //added on 25-07-2016

        public string Pass { get; set; }

        public string MotorType { get; set; }

        public string PolishRequirement { get; set; }

        public string TypeRice { get; set; } // also known as Process

        public string PaddySize { get; set; } // also known as Grain Type

        public string Capacity { get; set; }

        public string MotorQ { get; set; }

        public string MotorRating { get; set; }

        public int IsSOTStatus { get; set; } // to check the individual status of line item and If IsSOTStatus is 1 then BYJT Chances 100%, If 2 then BYJT 0%, if 0 then NO SOT.

        //for foreign keys
        public virtual RiceOAEquipGeneralData RiceOAEquipGeneralData { get; set; }

        public virtual ProductModel ProductModel { get; set; }

        public virtual Products Products { get; set; }

        public virtual MasterProducts MasterProducts { get; set; }
    }

    public class RRepModel
    {

        public int RQGID { get; set; }

        public string CompanyUniqueID { get; set; }

        public string CPQuotationNumber { get; set; }

        public string OrganizationName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string City { get; set; }

        public string Pincode { get; set; }

        public string State { get; set; }

        public string ContactPersonName { get; set; }

        public string KindAttention { get; set; }

        public string Subjectinfo { get; set; }

        public string QuotationDate { get; set; }

        public string PaymentTerms { get; set; }

        public string Delivery { get; set; }

        public string DateofDispatch { get; set; }

        public string Transport { get; set; }

        public string Freight { get; set; }

        public string CST { get; set; }

        public string TransitInsu { get; set; }

        public string Commodity { get; set; }

        public string Validity { get; set; }

        public string QuotationNumber { get; set; }

        public string OANmber { get; set; }

        public string LoginName { get; set; }

        public string Designation { get; set; }

        public string MDBID { get; set; }

        public string Logo { get; set; }

        public string footaddress { get; set; }

        public string overallprice { get; set; }

        public string annexure { get; set; }

        public string Approvaldate { get; set; }

        public string Tinnumber { get; set; }

        public string ProductVariety { get; set; }

        public string Type { get; set; }

        public string PaddySize { get; set; }

        public int ProductModelID { get; set; }

        public int ProductID { get; set; }

        public int MasterProductID { get; set; }

        public string MasterProductName { get; set; }

        public string ProductName { get; set; }


        public IList<QGEquipTableData> QGEquipTableData { get; set; }
        public IList<QGEquipGeneralData> QGEquipGeneralData { get; set; }
        public IList<RiceOAEquipTableData> RiceOAEquipTableData { get; set; }
    }


    [Table("RiceMillHOD")]
    public class RiceMillHOD
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RMHODID { get; set; }

        [Display(Name = "ROAID")]
        public string ROAID { get; set; }
        public virtual ICollection<RiceOAEquipGeneralData> RiceOAEquipGeneralData { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> HODDate { get; set; }


        [Display(Name = "RemarkDetails")]
        public string RemarkDetails { get; set; }

        public int CPID { get; set; }
        public virtual ChannelPartners ChannelPartners { get; set; }

        public int MDBID { get; set; }
        public virtual MDBGeneralData MDBGeneralData { get; set; }


        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int IsStatus { get; set; }



    }


}