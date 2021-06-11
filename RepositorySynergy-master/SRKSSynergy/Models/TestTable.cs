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
    [Table("TestTable")]
    public class TestTable
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TestTableID { get; set; }

        public string TestName1 { get; set; }

        public string TestName2 { get; set; }

        public string TestName3 { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }

        //public int IsStatus { get; set; }

        public int IsDrop { get; set; }
    }
}