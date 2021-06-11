using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("OCMDrive")]
    public class OCMDrive
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DriveID { get; set; }

        public string DriveDescription { get; set; }

        public string PartNumber { get; set; }

        public string DriveMotorType { get; set; }

        public int MasterProductID { get; set; }
        public virtual MasterProducts MasterProducts { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
    }
}



