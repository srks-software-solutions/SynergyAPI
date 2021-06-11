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
    [Table("Warranty")]
    public class Warranty
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WID { get; set; }

        public string CustomerNumber { get; set; }

        public string CustomerName { get; set; }

        public string OrderNumber { get; set; }

        public string BuhlerOrderConfirm { get; set; }

        public string Model { get; set; }

        public string MachineNummber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> HandoverDate { get; set; }

        public string WarrantyExpiry { get; set; }

        public int CPID { get; set; }

        public virtual ChannelPartners ChannelPartners { get; set; }
    }
}