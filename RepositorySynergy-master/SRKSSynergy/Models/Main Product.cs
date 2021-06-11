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
    public class Main_Product
    {
        public MasterProducts MasterProducts { get; set; }

        public Products Products { get; set; }

        public ProductModel ProductModel { get; set; }

        public ProductModelSpare ProductModelSpare { get; set; }
    }

    [Table("Products")]
    public class Products
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Commodity Name is Required")]
        [Display(Name = "Commodity Name")]
        [StringLength(20, MinimumLength = 4)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product Descriptor is Required")]
        [Display(Name = "Product Descriptor")]
        [StringLength(160, MinimumLength = 4)]
        public string ProductDescriptor { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        public int MasterProductID { get; set; }

        public virtual ICollection<ProductModel> ProductModel { get; set; }

        public virtual MasterProducts MasterProducts { get; set; }

    }

    [Table("ProductModel")]
    public class ProductModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProductModelID { get; set; }

        [Required(ErrorMessage = "Equipment Model Name is Required")]
        [Display(Name = "Equipment Model Name")]
        [StringLength(40, MinimumLength = 4)]
        public string ProductModelName { get; set; }

        //[Required(ErrorMessage = "Equipment Model Descriptor is Required")]
        [Display(Name = "Equipment Model Descriptor")]
        //[StringLength(160, MinimumLength = 4)]
        public string ProductModelDesc { get; set; }

        //[Required(ErrorMessage = "Equipment Model Exclusion is Required")]
        [Display(Name = "Equipment Model Descriptor")]
        //[StringLength(160, MinimumLength = 4)]
        public string ProductModelExclusion { get; set; }

        //[RegularExpression(@"[0-9]*\.?[0-9]+", ErrorMessage = "{0} must be a Number.")]
        [Required(ErrorMessage = "Unit Price is Required")]
        public string UnitPrice { get; set; }

        public string prmodeldesc { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string QuoteVaildTill { get; set; }

        [Required(ErrorMessage = "Delivery Time is Required")]
        [Display(Name = "Delivery Time")]
        [StringLength(40, MinimumLength = 4)]
        public string DeliveryTime { get; set; }

        public int ProductID { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        public virtual Products Products { get; set; }

        public int MachineCount { get; set; }

        public int ApprovedCount { get; set; }

        public int PR { get; set; }

    }

    [Table("ProductModelSpare")]
    public class ProductModelSpare
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProductModelSparesID { get; set; }

        [Required(ErrorMessage = "Spare Model Name is Required")]
        [Display(Name = "Spare Model Name")]
        [StringLength(40, MinimumLength = 4)]
        public string ProductModelSparesName { get; set; }

        [Required(ErrorMessage = "Spare Model Descriptor is Required")]
        [Display(Name = "Spare Model Descriptor")]
        [StringLength(160, MinimumLength = 4)]
        public string ProductModelSparesDesc { get; set; }

        public string ProductSpareNameDesc { get; set; }

        [Required(ErrorMessage = "Agent Price is Required")]
        public string AgentPrice { get; set; }

        [Required(ErrorMessage = "Customer Price is Required")]
        public string CustomerPrice { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string QuoteVaildTill { get; set; }

        [Required(ErrorMessage = "Delivery Time is Required")]
        [Display(Name = "Delivery Time")]
        [StringLength(40, MinimumLength = 4)]
        public string DeliveryTime { get; set; }

        public int BuhlerPresentStock { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

    }

    [Table("MasterProducts")]
    public class MasterProducts
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MasterProductID { get; set; }

        [Required(ErrorMessage = "Master Product Name is Required")]
        [Display(Name = "Master Product Name")]
        [StringLength(20, MinimumLength = 4)]
        public string MasterProductName { get; set; }

        [Required(ErrorMessage = "Master Product Descriptor is Required")]
        [Display(Name = "Master Product Descriptor")]
        [StringLength(160, MinimumLength = 4)]
        public string MasterProductDescriptor { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        //public virtual ICollection<Products> Products { get; set; }

    }

}