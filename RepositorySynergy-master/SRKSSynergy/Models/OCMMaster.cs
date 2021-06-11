using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SRKSSynergy.Models
{
    public class OCMMaster {
        public OCMGrainType OCMGrainType { get; set; }
        public OCMProcess OCMProcess { get; set; }
        public OCMPass OCMPass { get; set; }
        public OCMCapacityTPH OCMCapacityTPH { get; set; }
        public OCMCapacityKW OCMCapacityKW { get; set; }
        public OCMDriveMS OCMDriveMS { get; set; }
        public OCMStoneGrit OCMStoneGrit { get; set; }
        public OCMSieveslot OCMSieveslot { get; set; }
        public OCMBrakechamfer OCMBrakechamfer { get; set; }
        public OCMAccessoriesNew OCMAccessoriesNew { get; set; }
        public OCMProcessname OCMProcessname { get; set; }
        public OCMPassname OCMPassname { get; set; }
        public OCMReducerRingNew OCMReducerRingNew { get; set; }
        public OCMCTCoilNew OCMCTCoilNew { get; set; }
    }
     [Table("OCMGrainType")]
    public class OCMGrainType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GrainTypeId { get; set; }

        public string GrainName { get; set; }

        public string GrainShortName { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
    }

     [Table("OCMProcess")]
     public class OCMProcess
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int ProcessId { get; set; }

         public string ProcessName { get; set; }

         public string ProcessShortName { get; set; }

         public string PathName { get; set; }

         public int  GrainTypeId { get; set; }

         public string GrainName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMPass")]
     public class OCMPass
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PassShortName { get; set; }

         public string PathName { get; set; }

         public int GrainTypeId { get; set; }

         public string GrainName { get; set; }

         public int ProcessId { get; set; }

         public string ProcessName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }

         public int ProductId { get; set; }

         public string ProductName { get; set; }
     }

     [Table("OCMCapacityTPH")]
     public class OCMCapacityTPH
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int CapacityTPHId { get; set; }

         public string CapacityTPHName { get; set; }

         public string CapacityTPHShortName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMCapacityKW")]
     public class OCMCapacityKW
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int CapacityKWId { get; set; }

         public string CapacityKWName { get; set; }

         public string PathName { get; set; }

         public int GrainTypeId { get; set; }

         public string GrainName { get; set; }

         public int ProcessId { get; set; }

         public string ProcessName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public int CapacityTPHId { get; set; }

         public string CapacityTPHName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }

         public string ProductModelID { get; set; }
         public string ProductModelName { get; set; }
     }

     [Table("OCMDriveMS")]
     public class OCMDriveMS
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int DriveMSId { get; set; }

         public string DriveMSName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMStoneGrit")]
     public class OCMStoneGrit
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int StoneGritId { get; set; }

         public string StoneGritName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMSieveslot")]
     public class OCMSieveslot
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int SieveslotId { get; set; }

         public string SieveslotName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMBrakechamfer")]
     public class OCMBrakechamfer
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int BrakechamferId { get; set; }

         public string BrakechamferName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMAccessoriesNew")]
     public class OCMAccessoriesNew
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int AccessoriesId { get; set; }

         public string AccessoriesName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMProcessname")]
     public class OCMProcessname
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int ProcessNameId { get; set; }

         public string ProcessName { get; set; }
         public string ProcessShortName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMPassname")]
     public class OCMPassname
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int PassNameId { get; set; }

         public string PassName { get; set; }
         public string PassShortName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }

     }

     [Table("OCMReducerRingNew")]
     public class OCMReducerRingNew
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int OCMReducerRingId { get; set; }

         public string OCMReducerRingName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }

     [Table("OCMCTCoilNew")]
     public class OCMCTCoilNew
     {
         [Key]
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
         public int OCMCTCoilId { get; set; }

         public string OCMCTCoilName { get; set; }

         public int PassId { get; set; }

         public string PassName { get; set; }

         public string PathName { get; set; }

         public Nullable<System.DateTime> CreatedOn { get; set; }
         public string CreatedBy { get; set; }

         public Nullable<System.DateTime> ModifiedOn { get; set; }
         public string ModifiedBy { get; set; }

         public int IsDeleted { get; set; }
     }
}