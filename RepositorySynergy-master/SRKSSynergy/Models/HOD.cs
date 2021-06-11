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
    public class HOD
    {
        public Handover Handover { get; set; }
    }

    [Table("Handover")]
    public class Handover
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int HID { get; set; }

        [Display(Name = "HOD Number")]
        public string HODNumber { get; set; }

        public string OAnum { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Custpurorderno { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Buhlercustorderno { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Project { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Buhlerrep { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string Custrep { get; set; }

        public bool Handing { get; set; }

        public bool Commis { get; set; }

        public bool Termi { get; set; }

        public bool Erection { get; set; }

        public bool Trail { get; set; }

        public bool Wimaterial { get; set; }

        public bool Womaterial { get; set; }

        public bool Wcarr { get; set; }

        public bool Wnotcarr { get; set; }

        public bool Completionerection { get; set; }

        public bool Capacity { get; set; }

        public bool Reached { get; set; }
        
        public bool Notreached { get; set; }

        public bool Machinehanded { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //public string Handeddate { get; set; }Nullable<System.DateTime>
        public Nullable<System.DateTime> Handeddate { get; set; }

        public bool Buyer  { get; set; }

        public bool Followcomm { get; set; }

        public bool Seperate { get; set; }

        [StringLength(160, MinimumLength = 3)]
        public string Comments { get; set; }

        public bool Yes { get; set; }

        public bool No { get; set; }

        public string MacSlNo { get; set; }

        public string modelno { get; set;  }

        public int Quantity { get; set; }

        public int CPID { get; set; }

        public int MDBID { get; set; }
        public virtual MDBGeneralData MDBGeneralData { get; set; }
        public virtual ChannelPartners ChannelPartners { get; set; }

    }
}