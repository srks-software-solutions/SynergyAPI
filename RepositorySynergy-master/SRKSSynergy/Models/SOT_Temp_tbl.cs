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
    public class SOT_Temp_tbl
    {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int TSOTID { get; set; }

            public int QGID { get; set; }

            //public int QGTBID { get; set; }

            public int CPID { get; set; }

            //[Required(ErrorMessage = "BYJT Chances is Required")]
            [Display(Name = "BYJTChances")]
            public Nullable<int> BYJTChances { get; set; }

            //[Required(ErrorMessage = "TOP Competitors is Required")]
            [Display(Name = "TOP Competitors")]
            public string TOPCompetitors { get; set; }

            ////[Required(ErrorMessage = "Competitors Chances is Required")]
            //[Display(Name = "Competitors Chances")]
            //public Nullable<int> CompetitorsChances { get; set; }

            [Display(Name = "Comments")]
            [StringLength(160, MinimumLength = 4)]
            public string AdditionalComments { get; set; }

            //[DataType(DataType.Date)]
            //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
            //public Nullable<System.DateTime> UpdationDate { get; set; }

            //public string Commodity { get; set; }

            public string Equipment { get; set; }

            public int Islatestquo { get; set; }

            public int Quantity { get; set; }

            public string Orderactive { get; set; }

            public string Expectedorder {get; set;}

            public int Status { get; set; }

            //public Nullable<int> Status { get; set; }

            public Nullable<System.DateTime> Expecteddate { get; set; }

            public Nullable<System.DateTime> CreatedOn { get; set; }

            public string CreatedBy { get; set; }

            public Nullable<System.DateTime> ModifiedOn { get; set; }

            public string ModifiedBy { get; set; }

            public int ProductID { get; set; }

            public int MasterProductID { get; set; }

            public int ProductModelID { get; set; }

            //public virtual Products Products { get; set; }

            //public virtual MasterProducts MasterProducts { get; set; }

            //public virtual ChannelPartners ChannelPartners { get; set; }

           // public virtual QGEquipGeneralData QGEquipGeneralData { get; set; }

            //public virtual QGEquipTableData QGEquipTableData { get; set; }
    }
}