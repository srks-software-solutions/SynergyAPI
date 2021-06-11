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
    public class Outward
    {
        public OutwardSpare OutwardSpare1 { get; set; }
        public OutwardSpare OutwardSpare2 { get; set; }
        public OutwardSpare OutwardSpare3 { get; set; }
        public OutwardSpare OutwardSpare4 { get; set; }
        public OutwardSpare OutwardSpare5 { get; set; }
        public OutwardMFR OutwardMFR{ get; set; }
    }

    //
    //Minimum Spare Quantity Admin Entry
    //
    [Table("OutwardSpare")]
    public class OutwardSpare
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OutwardID { get; set; }

        [Display(Name = "Outward-Month")]
        //[StringLength(50, MinimumLength = 15)]
        public string OutwardMonth { get; set; }

        [Display(Name = "Outward-OrderNo")]
        //[StringLength(50, MinimumLength = 15)]
        public string OrderNo { get; set; }

        [Display(Name = "Customer Name")]
        //[StringLength(50, MinimumLength = 15)]
        public string CustomerName { get; set; }

        public int ProductModelSparesID { get; set; }

        //public int InwardID { get; set; }

        public int Quantity { get; set; }

        public int QuantityOrdered { get; set; }

        [Display(Name = "Total Value")]
        //[StringLength(50, MinimumLength = 15)]
        public string TotalValue { get; set; }

        [Display(Name = "InvoiceNo")]
        //[StringLength(50, MinimumLength = 15)]
        public string InvoiceNo { get; set; }

        [Display(Name = "InvoiceDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[StringLength(50, MinimumLength = 15)]
        public DateTime InvoiceDate { get; set; }

        [Display(Name = "DispatchDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[StringLength(50, MinimumLength = 15)]
        public DateTime DispatchDate { get; set; }

        public int CPID { get; set; }
        public virtual ChannelPartners ChannelPartners { get; set; }

        [Display(Name = "Remarks")]
        //[StringLength(50, MinimumLength = 15)]
        public String Remarks { get; set; }

        public int IsDeleted { get; set; }

        public int IsDispatch { get; set; }

        public int OutMonthNo { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }

        //public virtual InwardSpare InwardSpare { get; set; }

    }

    [Table("OutwardMFR")]
    public class OutwardMFR
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MFRId { get; set; }

        [Display(Name = "Outward-Month")]
        //[StringLength(50, MinimumLength = 15)]
        public string OutwardMonth { get; set; }

        [Display(Name = "MFRNo")]
        //[StringLength(50, MinimumLength = 15)]
        public string MFRNo { get; set; }

        [Display(Name = "Customer Name")]
        //[StringLength(50, MinimumLength = 15)]
        public string CustomerName { get; set; }

        public int ProductModelSparesID { get; set; }

        //public int InwardID { get; set; }

        public int Quantity { get; set; }

        public int IsDispatch { get; set; }

        public int CPID { get; set; }

        [Display(Name = "Total Value")]
        public string TotalValue { get; set; }

        [Display(Name = "DispatchDate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DispatchDate { get; set; }

        [Display(Name = "Remarks")]
        public String Remarks { get; set; }

        public int IsDeleted { get; set; }

        public int QuantityOrdered { get; set; }

        public int OutMonthNo { get; set; }



        public int IsSparesRecieved { get; set; }
        public Nullable<System.DateTime> SparesRecievedDate { get; set; }

        public Nullable<System.DateTime> MachineIssueDate { get; set; }
        public string MachineIssueDescription { get; set; }
        public int IsMachineIssueOpen { get; set; }

        public Nullable<System.DateTime> FaultSpareDate { get; set; }
        public string FaultSpareDescription { get; set; }

        public Nullable<System.DateTime> FaultSpareRecivedDateAdmin { get; set; }

        public virtual ChannelPartners ChannelPartners { get; set; }

        //public virtual InwardSpare InwardSpare { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }

    }



}