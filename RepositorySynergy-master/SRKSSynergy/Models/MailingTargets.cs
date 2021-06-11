using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("MailingTargets")]
    public class MailingTargets
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MTID { get; set; }
        public int HalfMonthMail { get; set; }
        public int MonthEndMail { get; set; }
        public string MonthYear { get; set; }
    }
}