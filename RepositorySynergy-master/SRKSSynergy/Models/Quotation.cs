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
    public class Quotation
    {
        public QGEquipGeneralData QGEquipGeneralData { get; set; }

        public QGEquipPayment QGEquipPayment { get; set; }

        public QGEquipTableData QGEquipTableData1 { get; set; }

        public QGEquipTableData QGEquipTableData2 { get; set; }

        public QGEquipTableData QGEquipTableData3 { get; set; }

        public QGEquipTableData QGEquipTableData4 { get; set; }

        public QGEquipTableData QGEquipTableData5 { get; set; }

        public QGEquipTableData QGEquipTableData6 { get; set; }

        public QGEquipTableData QGEquipTableData7 { get; set; }

        public QGEquipTableData QGEquipTableData8 { get; set; }

        public QGEquipTableData QGEquipTableData9 { get; set; }

        public QGEquipTableData QGEquipTableData10 { get; set; }

        public QGEquipTableData QGEquipTableData11 { get; set; }

        public QGEquipTableData QGEquipTableData12 { get; set; }

        public QGEquipTableData QGEquipTableData13 { get; set; }

        public QGEquipTableData QGEquipTableData14 { get; set; }

        public QGEquipTableData QGEquipTableData15 { get; set; }

        public QGEquipTableData QGEquipTableData16 { get; set; }

        public QGEquipTableData QGEquipTableData17 { get; set; }

        public QGEquipTableData QGEquipTableData18 { get; set; }

        public QGEquipTableData QGEquipTableData19 { get; set; }

        public QGEquipTableData QGEquipTableData20 { get; set; }

        public QGEquipTableData QGEquipTableData21 { get; set; }

    }
    //
    //Start
    //Equipment Tables
    //
    [Table("QGEquipGeneralData")]
    public class QGEquipGeneralData
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

        public int Ordergenerated { get; set; }

        public int QuotStatus { get; set; }

        [Required(ErrorMessage = "Salutation is Required")]
        [Display(Name = "Salutation")]
        [StringLength(320, MinimumLength = 4)]
        public string KindAttention { get; set; }

        [Display(Name = "Sales Engg Name")]
        [StringLength(30, MinimumLength = 4)]
        public string SalesName { get; set; }

        [Required(ErrorMessage = "Subject is Required")]
        [Display(Name = "Subject")]
        [StringLength(160, MinimumLength = 4)]
        public string Subjectinfo { get; set; }

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

        public int IsRiceMill { get; set; }

        public int CPID { get; set; }

        public string ProductVariety { get; set; }

        public string TypeRice { get; set; } // also known as Process

        public string PaddySize { get; set; } // also known as Grain Type

        public string Pass { get; set; }

        public string Capacity { get; set; }

        public string PolishRequirement { get; set; }

        public string MotorQ { get; set; }

        public string MotorType { get; set; }

        public string MotorRating { get; set; }

        //public virtual RiceOAEquipGeneralData RiceOAEquipGeneralData { get; set; }
        //public virtual ChannelPartners ChannelPartners { get; set; }

        public virtual ICollection<QGEquipPayment> QGEquipPayments { get; set; }

        public virtual ICollection<QGEquipTableData> QGEquipTableDatas { get; set; }

        public String LeadTime { get; set; }

        public int IsTime { get; set; }
    }

    [Table("QGEquipPayment")]
    public class QGEquipPayment
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

        [StringLength(320, MinimumLength = 4,ErrorMessage = "Minimum Length should be 4 and Maximum is 320")]
        public string annexure { get; set; }
        
        public string overallprice { get; set; }

        [Required(ErrorMessage = "Validity is Required")]
        [Display(Name = "Validity")]
        [StringLength(70, MinimumLength = 4)]
        [DefaultValue("2 months from the date of this offer")]
        public string Validity { get; set; }

        public virtual QGEquipGeneralData QGEquipGeneralData { get; set; }
    }

    [Table("QGEquipTableData")]
    public class QGEquipTableData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int QGTBID { get; set; }

        public int QGID { get; set; }

        public string ModelNum { get; set; }
        

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

        public int ProductID { get; set; }

        public int MasterProductID { get; set; }

        public string MasterProductName { get; set; }

        public string ProductName { get; set; }

        public virtual QGEquipGeneralData QGEquipGeneralData { get; set; }

       //public virtual Products Products { get; set; }

       // public virtual MasterProducts MasterProducts { get; set; }

      public virtual ProductModel ProductModel { get; set; }

        //New Added Columns On 07-12-2016

      public string TypeRice { get; set; } // also known as Process

      public string PaddySize { get; set; } // also known as Grain Type

      public string Pass { get; set; }

      public string Capacity { get; set; }

      public string PolishRequirement { get; set; }

      public string MotorQ { get; set; }

      public string MotorType { get; set; }

      public string MotorRating { get; set; }

      public int IsSOTStatus { get; set; } // to check the individual status of line item and If IsSOTStatus is 1 then BYJT Chances 100%, If 2 then BYJT 0%, if 0 then NO SOT.

        //[ForeignKey("ProductID")]
        //public virtual Products Products { get; set; }

        //[ForeignKey("MasterProductID")]
        //public virtual MasterProducts MasterProducts { get; set; }
    }

    public class ReportModel
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

        public string ProductVariety { get; set; }

        public string Type { get; set; }

        public string PaddySize { get; set; }

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

        public IList<QGEquipTableData> QGEquipTableData { get; set; }
    }


    //
    //End
    //

    
}