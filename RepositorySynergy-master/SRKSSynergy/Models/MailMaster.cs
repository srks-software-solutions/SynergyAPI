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

    public class MailMasters1
    {
        public MailMasters MailMasters { get; set; }
    }

    [Table("MailMasters")]
    public class MailMasters
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MailID { get; set; }

        [Display(Name = "EMail-Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Mail-Type")]
        public string MailType { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
    }
}