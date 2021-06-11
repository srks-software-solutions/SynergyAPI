using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
     [Table("AutoMailSystem")]
    public class AutoMailSystem
    {     
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int AMSID { get; set; }

            public int CPID { get; set; }                  
           
            public Nullable<System.DateTime> MailSentDate { get; set; }

            public int IsSent { get; set; }

        }
    }
