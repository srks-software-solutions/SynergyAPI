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
    [Table("LOASum")]
    public class LOASum
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LOAID { get; set; }

        [Display(Name = "Reason For Losing")]
        public string ReasonForLosing { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int TotalNumbers { get; set; }
    }
}