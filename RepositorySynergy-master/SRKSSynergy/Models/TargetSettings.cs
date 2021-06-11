using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("TargetSettings")]
    public class TargetSettings
    {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int TSID { get; set; }

            public int CPID { get; set; }

            public string TargetMonth { get; set; }

            public string TargetMonthID { get; set; }

            public int Targets { get; set; }
           
            public Nullable<System.DateTime> CreatedOn { get; set; }

            public string CreatedBy { get; set; }

            public Nullable<System.DateTime> ModifiedOn { get; set; }

            public string ModifiedBy { get; set; }

            public int Status { get; set; }            
    }


    //Target Setting for Lear Report : 28-04-2016
    [Table("TargetSettingsLead")]
    public class TargetSettingsLead
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TSID { get; set; }

        public string MachineType { get; set; }

        public string TargetType { get; set; }

        public int CPID { get; set; }

        public string  Year { get; set; }

        public string  Month { get; set; }

        public int MonthId { get; set; }

        public int Targets { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        public int Status { get; set; }
    }
}