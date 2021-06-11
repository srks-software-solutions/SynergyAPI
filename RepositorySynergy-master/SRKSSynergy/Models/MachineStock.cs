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
    public class MachineStock
    {
        public MachineInventory MachineInventory { get; set; }

        public MachineDispatch MachineDispatch { get; set; }
    }

    [Table("MachineInventory")]
    public class MachineInventory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MachineInventoryID { get; set; }

        [Required(ErrorMessage = "Machine Serial No is Required")]
        [Display(Name = "Machine Serial No")]
        [StringLength(40, MinimumLength = 4)]
        public string MachineSerialNo { get; set; }

        [Display(Name = "Type")]
        [StringLength(40, MinimumLength = 3)]
        public string Type { get; set; }

        [Display(Name = "Place Stocked")]
        [StringLength(160, MinimumLength = 4)]
        public string PlaceStocked { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(160, MinimumLength = 4)]
        public string Remarks { get; set; }

        [Display(Name = "Customer Name")]
        [StringLength(160, MinimumLength = 4)]
        public string CustomerName { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        public int IsDispatched { get; set; }

        public int MDBID { get; set; }
        //public virtual MDBGeneralData MDBGeneralData { get; set; }

        public int CPID { get; set; }
        //public virtual ChannelPartners ChannelPartners { get; set; }

        public int ProductModelID { get; set; }

        public virtual ProductModel ProductModel { get; set; }

    }

    [Table("MachineDispatch")]
    public class MachineDispatch
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MachineDispatchID { get; set; }

        //public string CustomerName { get; set; }

        public int MachineInventoryID { get; set; }
        public virtual MachineInventory MachineInventory { get; set; }

        public int OAID { get; set; }
        public virtual OAEquipGeneralData OAEquipGeneralData { get; set; }

        [Display(Name = "OA Number")]
        //[StringLength(160, MinimumLength = 4)]
        public string OANumber { get; set; }

        [Display(Name = "Invoice Number")]
        [StringLength(160, MinimumLength = 4)]
        public string InvoiceNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> InvoiceDate { get; set; }

        //public string InvoiceDate { get; set; }

        [Display(Name = "LR Number")]
        [StringLength(160, MinimumLength = 4)]
        public string LRNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DispatchDate { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> OADate { get; set; }

        //public string OADate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CommissionDate { get; set; }

        //public int HID { get; set; }
        //public virtual Handover Handover { get; set; }

        [Display(Name = "Transporter")]
        [StringLength(160, MinimumLength = 4)]
        public string Transporter { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        public int IsDispatched { get; set; }

        public int MDBID { get; set; }
        public virtual MDBGeneralData MDBGeneralData { get; set; }

        public int CPID { get; set; }
        public virtual ChannelPartners ChannelPartners { get; set; }

        public int ProductModelID { get; set; }

       // public virtual ProductModel ProductModel { get; set; }

    }

}