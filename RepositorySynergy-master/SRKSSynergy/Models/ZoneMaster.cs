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
    public class ZoneMaster
    {
        public Zone Zone { get; set; }
    }

    [Table("Zone")]
    public class Zone
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ZoneID { get; set; }

        public string ZoneName { get; set; }

        //public int CPID { get; set; }
        //public virtual ChannelPartners ChannelPartners { get; set; }

        public string ZonalMangerName { get; set; }

        public int IsDeactive { get; set; }

        //public Guid UserID { get; set; }

    }
}