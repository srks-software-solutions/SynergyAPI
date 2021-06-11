using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("AutoMailer")]
    public class AutoMailer
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AMID { get; set; }

        public int OTPeriod { get; set; }

        public string Module { get; set; }

        public int NullPeriod { get; set; }      

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        public int IsStatus { get; set; }       

    }
}