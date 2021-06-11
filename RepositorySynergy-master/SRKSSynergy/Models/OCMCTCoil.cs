using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("OCMCTCoil")]
    public class OCMCTCoil
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CTCoilID { get; set; }

        public string CTCoilDescription { get; set; }

        public string PartNumber { get; set; }

        public int MasterProductID { get; set; }
        public virtual MasterProducts MasterProducts { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
    }
}