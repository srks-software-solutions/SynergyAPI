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
    public class logindata
    {
        public LoginData LoginData { get; set; }
    }

    [Table("LoginData")]
    public class LoginData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserLoginID { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> LoginDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]

        public Nullable<System.DateTime> LogOutDate { get; set; }

        public String LoginTime { get; set; }

        public String LogOutTime { get; set; }

        public String Duration { get; set; }

       public string channelpartnerid { get; set; }
       public string ZoneId { get; set; }

        public int IsStatus { get; set; }

        public Int64 ValidityPeriodTicks { get; set; }

        public Int64 VlidityPeriodTicks { get; set; }

        [NotMapped]
        public TimeSpan ValidityPeriod1
        {
            get { return TimeSpan.FromTicks(VlidityPeriodTicks); }
            set { VlidityPeriodTicks = value.Ticks; }
        }
        public Int64 ValidityPeriodTicks1 { get; set; }

        [NotMapped]
        public TimeSpan ValidityPeriod
        {
            get { return TimeSpan.FromTicks(ValidityPeriodTicks); }
            set { ValidityPeriodTicks = value.Ticks; }
        }



    }
}