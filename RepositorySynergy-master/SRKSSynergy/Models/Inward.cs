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
    public class Inward
    {
        public UserLoginData InwardSpare1 { get; set; }
        public UserLoginData InwardSpare2 { get; set; }
        public UserLoginData InwardSpare3 { get; set; }
        public UserLoginData InwardSpare4 { get; set; }
        public UserLoginData InwardSpare5 { get; set; }
    }

    [Table("InwardSpare")]
    public class UserLoginData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int InwardID { get; set; }

        [Display(Name = "Inward-Month")]
        //[StringLength(50, MinimumLength = 15)]
        public string InwardMonth { get; set; }

        //[Display(Name = "Present-Stock")]
        //public int PresentStock { get; set; }

        [Display(Name = "Inward-Type")]
        //[StringLength(50, MinimumLength = 20)]
        public string InwardType { get; set; }

        [Display(Name = "PO Number")]
        //[StringLength(50, MinimumLength =25)]
        public string PONumber { get; set; }

        public int ProductModelSparesID { get; set; }

      //  public int MFRID { get; set; }

        //[Display(Name = "MFR Number")]
        //public string MFRNo { get; set; }

        public int QuantityOrdered { get; set; }

        [Display(Name = "Total Value")]
        public string TotalValue { get; set; }

        public int QuantityRemaining { get; set; }

        public int IsDeleted { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }

        //public virtual MFR MFR { get; set; }


    }

}