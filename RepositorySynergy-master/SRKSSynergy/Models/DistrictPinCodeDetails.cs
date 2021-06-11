using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("DistrictPinCodeDetails_tbl")]
    public class DistrictPinCodeDetails
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DPID { get; set; }

        public string State { get; set; }

        public string District { get; set; }

        public string PostalDivision { get; set; }

        public string PostalRegion { get; set; }

        public string PostalCircle { get; set; }

        public string Taluk { get; set; }

        public int PINCode { get; set; }

        public string OfficeName { get; set; }

        public string OfficeStatus { get; set; }

        public string TelePhone { get; set; }

        public int IsDeleted { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public int StateID { get; set; }
      
    }
}