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
    public class QuotationSpares
    {
        public QGSpareGeneralData QGSpareGeneralData { get; set; }

        public QGSparePayment QGSparePayment { get; set; }

        public QGSpareTableData QGSpareTableData1 { get; set; }

        public QGSpareTableData QGSpareTableData2 { get; set; }

        public QGSpareTableData QGSpareTableData3 { get; set; }

        public QGSpareTableData QGSpareTableData4 { get; set; }

        public QGSpareTableData QGSpareTableData5 { get; set; }

        public QGSpareTableData QGSpareTableData6 { get; set; }

        public QGSpareTableData QGSpareTableData7 { get; set; }

        public QGSpareTableData QGSpareTableData8 { get; set; }
    }
    public class ReportModel1
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

        public string Subject { get; set; }

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

        public string LoginName { get; set; }

        public string Designation { get; set; }

        public string MDBID { get; set; }

        public string Logo { get; set; }

        public string footaddress { get; set; }

        public string overallprice { get; set; }

        public string annexure { get; set; }

        public IList<QGSpareTableData> QGSpareTableData { get; set; }
    }
    //
    //Start
    //Spare Tables
    //
    [Table("QGSpareGeneralData")]
    public class QGSpareGeneralData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int QGID { get; set; }

        [Display(Name = "Quotation Number")]
        public string QuotationNumber { get; set; }

        //[Required(ErrorMessage = "Organization Name is Required")]
        //[Display(Name = "Channel Partner Quotation Number")]
        //[StringLength(30, MinimumLength = 2)]
        public string CPQuotationNumber { get; set; }

        [Required(ErrorMessage = "Salutation is Required")]
        [Display(Name = "Salutation")]
        [StringLength(320, MinimumLength = 4)]
        public string KindAttention { get; set; }

        [Required(ErrorMessage = "Subject is Required")]
        [Display(Name = "Subject")]
        [StringLength(160, MinimumLength = 4)]
        public string Subject { get; set; }

        public string CompanyUniqueID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> QuotationDate { get; set; }

        public int MDBID { get; set; }
        public virtual MDBGeneralData MDBGeneralData { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int Islatest { get; set; }

        public int CPID { get; set; }

        public virtual ICollection<QGSparePayment> QGSparePayment { get; set; }

        public virtual ICollection<QGSpareTableData> QGSpareTableData { get; set; }
    }

    [Table("QGSparePayment")]
    public class QGSparePayment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int QGP { get; set; }

        public int QGID { get; set; }

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
        [StringLength(70, MinimumLength = 4)]
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

        public virtual QGSpareGeneralData QGSpareGeneralData { get; set; }
    }

    [Table("QGSpareTableData")]
    public class QGSpareTableData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int QGTBID { get; set; }

        public int QGID { get; set; }

        ////[Required(ErrorMessage = "Model Number is Required")]
        //public string Sparecode { get; set; }

        //[Required(ErrorMessage = "Quantity is Required")]
        public int Quantity { get; set; }

        //[RegularExpression(@"[1-9]+", ErrorMessage = "Price must can't have more than 4 decimal places")]
        public string UnitPrice { get; set; }

        public string TotalPrice { get; set; }

        public string Description { get; set; }

        //[Display(Name = "Exclusion")]
        //[StringLength(160, MinimumLength = 4)]
        //[DefaultValue("Air compressor, dryer, machine stand, cabin room,  hopper, air dryer, C.V Transformer, dust suction fan, AC and Bucket elevators.")]
        //public string Exclusion { get; set; }

        public int ProductModelSparesID { get; set; }

        public virtual QGSpareGeneralData QGSpareGeneralData { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }
    }
    //
    //End
    //
}