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
    public class LostOrder
    {
      
    }

    [Table("LostOrderAnalysis")]
    public class LOABarReport
    {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int LOID { get; set; }

         public int QGID { get; set; }

         public Nullable<System.DateTime> LOADate { get; set; }

         public string Competitor { get; set; }

         public string ReasonForLosing { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }

         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }

         public string ModifiedBy { get; set; }

         public int CPID { get; set; }

         public string comments { get; set; }

         public string EquipModel { get; set; }

         public string qty { get; set; }

         public int TSOTID { get; set; }

         public virtual ProductModel ProductModel { get; set; }

         //public virtual SOT SOT { get; set; }
         public virtual QGEquipGeneralData QGEquipGeneralData { get; set; }
    }
}