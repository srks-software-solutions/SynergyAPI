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
    public class MinimumSpareQuantity
    {
        public MinSpareEquipQuantity MinSpareEquipQuantity { get; set; }

        public AvailSpareQuantity AvailSpareQuantity { get; set; }

    }

    //
    //Minimum Spare Quantity Admin Entry
    //
    [Table("MinSpareEquipQuantity")]
    public class MinSpareEquipQuantity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MSID { get; set; }

        [Display(Name = "Minimum Spare Quantity")]
        public int Minimumstock { get; set; }

        public int ProductModelSparesID { get; set; }

        public int ProductModelID { get; set; }

        public int IsOld { get; set; }
        
        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public Nullable<System.DateTime> MinDate { get; set; }

        public int PresentStock { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }

        public int CPMinStock { get; set; }

    }

    //
    //Available Spare Stock entry by Channel Partner
    //
    [Table("AvailSpareQuantity")]
    public class AvailSpareQuantity 
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ASID { get; set; }

        public int MinCpStock { get; set; }

        public int CPID { get; set; }

        public int ProductModelSparesID { get; set; }

        [Display(Name = "Available Spare Quantity")]
        public int AvailableStock { get; set; }

        public string month { get; set; }

        public string week { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsOld { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }

    }
}