using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("OverallReport")]
    public class OverallReport
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ORID { get; set; }

        public int SLC { get; set; }

        public int BLC { get; set; }

        public int CPID { get; set; }

        public Nullable<System.DateTime> StartDate  { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }

        public int WeekNumber { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public int SQS { get; set; }

        public int BQS { get; set; }

        public int SOB { get; set; }

        public int BOB { get; set; }

        public int BMD { get; set; }


        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
    }
}