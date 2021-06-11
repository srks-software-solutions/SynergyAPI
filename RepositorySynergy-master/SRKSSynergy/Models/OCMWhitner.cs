using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    [Table("OCMWhitner")]
    public class OCMWhitner
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OWID { get; set; }

        public string ModelName { get; set; }

        public string GrainType { get; set; }
        public string Process { get; set; }
        public string Pass { get; set; }
        public string Capacity { get; set; }
        public string MotorQ { get; set; }
        public string MotorType { get; set; }
        public string MotorRating { get; set; }


        public string Drive { get; set; }
        public string Motor { get; set; }
        public string Stone { get; set; }
        public string Sieve { get; set; }
        public string Brake { get; set; }        
        public string CTCoil { get; set; }
        public string PassageSticker { get; set; }
        public string AccessoriesToolBox { get; set; }
        

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
    }
}