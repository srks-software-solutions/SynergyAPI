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
    public class OA
    {
        public OAEquipGeneralData OAEquipGeneralData { get; set; }

        public OAEquipPayment OAEquipPayment { get; set; }

        public OAEquipTableData OAEquipTableData1 { get; set; }

        public OAEquipTableData OAEquipTableData2 { get; set; }

        public OAEquipTableData OAEquipTableData3 { get; set; }

        public OAEquipTableData OAEquipTableData4 { get; set; }

    }
    //
    //Start
    //Equipment Tables
    //
    [Table("OAEquipGeneralData")]
    public class OAEquipGeneralData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OAID { get; set; }

        public int QGID { get; set; }

        [Display(Name = "Quotation Number")]
        public string QuotationNumber { get; set; }

        [Display(Name = "Order Acknowledgement Number")]
        public string OANumber { get; set; }
        
        public string CPQuotationNumber { get; set; }

        [Required(ErrorMessage = "Salutation is Required")]
        [Display(Name = "Salutation")]
        [StringLength(320, MinimumLength = 4)]
        public string KindAttention { get; set; }

        //[Required(ErrorMessage = "Subject is Required")]
        //[Display(Name = "Subject")]
        //[StringLength(160, MinimumLength = 4)]
        //public string Subject { get; set; }

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
        [StringLength(160)]
        public string OARejectComm { get; set; }

        public int ApprovalStatus { get; set; }

        //public Nullable<System.DateTime> Approvaldate { get; set; }
        public string Approvaldate { get; set; }

        public int CPID { get; set; }

        public string CPName { get; set; }

        public virtual ChannelPartners ChannelPartners { get; set; }

        public virtual ICollection<OAEquipPayment> OAEquipPayments { get; set; }

        public virtual ICollection<OAEquipTableData> OAEquipTableDatas { get; set; }

        [Display(Name = "TIN")]
        [StringLength(20, MinimumLength = 11)]
        [RegularExpression("^([a-zA-Z0-9]+$)", ErrorMessage = "Enter only alphabets and numbers")]
        public string TinNumber { get; set; }

        public int IsHOD { get; set; }

        public int IsMacineDispatch { get; set; }

    }

    [Table("OAEquipPayment")]
    public class OAEquipPayment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OAPID { get; set; }

        public int OAID { get; set; }

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
        [DefaultValue("2% extra against FORM C or 5.5% without FORM C")]
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

        //public virtual OAEquipGeneralData OAEquipGeneralData { get; set; }
    }

    [Table("OAEquipTableData")]
    public class OAEquipTableData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OATBID { get; set; }

        public int OAID { get; set; }

        ////[Required(ErrorMessage = "Model Number is Required")]
        //public string ModelNum { get; set; }

        //[Required(ErrorMessage = "Quantity is Required")]
        public int Quantity { get; set; }

        //[RegularExpression(@"^\d+.\d{0,4}$", ErrorMessage = "Price must can't have more than 4 decimal places")]
        public string UnitPrice { get; set; }

        public string TotalPrice { get; set; }

        //public string Description { get; set; }

        public string ModelDesc { get; set; }    

        [Display(Name = "Exclusion")]
        [StringLength(160, MinimumLength = 4)]
        [DefaultValue("Air compressor, dryer, machine stand, cabin room,  hopper, air dryer, C.V Transformer, dust suction fan, AC and Bucket elevators.")]
        public string Exclusion { get; set; }

        public int ProductModelID { get; set; }

        public int IsQuantity { get; set; }

        public int IsModelHOD { get; set; }

        //public virtual OAEquipGeneralData OAEquipGeneralData { get; set; }

        public virtual ProductModel ProductModel { get; set; }
    }


    public class RepModel
    {

        public int QGID { get; set; }

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

        public IList<OAEquipTableData> OAEquipTableData { get; set; }
    }

}