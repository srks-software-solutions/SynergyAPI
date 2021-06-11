using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("OASAPDetails")]
    public class OASAPDetails
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SAPID { get; set; }

        public int OrderID { get; set; }

        public string CustID { get; set; }

        public string SAPNO { get; set; }

        public Nullable<System.DateTime> SAPDate { get; set; }

        public string SAPComments { get; set; }

        public Nullable<System.DateTime> OADate { get; set; }

        public int CPID { get; set; }

        public int IsRice { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        //updated on 6-7-2016
        public Nullable<System.DateTime> DispatchedDate { get; set; }
     
    }
}