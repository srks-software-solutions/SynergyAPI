using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("States_tbl")]
    public class States_tbl
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StateID { get; set; }

        public string State { get; set; }

        public int IsDeleted { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public int CreatedBy { get; set; }  
    }
}