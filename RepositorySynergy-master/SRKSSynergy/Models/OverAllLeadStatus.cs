using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    public class OverAllLeadStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OLSID { get; set; }

        public int LEID { get; set; }

        public int ID { get; set; }

        public int IsIdStatus { get; set; }
        
        public Nullable<System.DateTime> Date { get; set; }

        public Nullable<System.DateTime> Time { get; set; }

        public int  CPID { get; set; }
    }
}