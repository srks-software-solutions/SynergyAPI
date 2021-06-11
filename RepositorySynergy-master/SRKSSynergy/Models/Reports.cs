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
    public class Reports
    {
    }

    //Equipment Sales By Volume Report Tables
    [Table("EquipSalesByVol")]
    public class EquipSalesByVol
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ESBVOLID { get; set; }

        public int CPID { get; set; }

        [Display(Name = "Channel Partner Name")]
        public string CPName { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int ProductModelID { get; set; }

        public int qty { get; set; }
    }

    //Equipment Sales By Volume Order Acknowledgement Report Tables
    [Table("EquipSalesByVolAck")]
    public class EquipSalesByVolAck
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ESBVOLACKID { get; set; }

        public int CPID { get; set; }

        [Display(Name = "Channel Partner Name")]
        public string CPName { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int ProductModelID { get; set; }

        public int qty { get; set; }
    }

    //Machine Invoiced(Machine Dispatch) Report Tables
    [Table("MachineInvoiced")]
    public class MachineInvoiced
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MIID { get; set; }

        [Display(Name = "Channel Partner Name")]
        public string CPName { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int qty { get; set; }
    }

    //Sales Oppurtunity Tracker Report By Volume Tables
    [Table("SOTByVol")]
    public class SOTByVol
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SOTBVOLID { get; set; }

        public int CPID { get; set; }

        [Display(Name = "Channel Partner Name")]
        public string CPName { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int ProductModelID { get; set; }

        public int qty { get; set; }
    }

    //Channel Partner Performance Report Tables
    [Table("EquipSalesByVal")]
    public class EquipSalesByVal
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ESBVALID { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int value { get; set; }

        public String valuecur { get; set; }

        public int qty { get; set; }
    }

    [Table("SOTByVolCP")]
    public class SOTByVolCP
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SOTBVOL3ID { get; set; }

        [Display(Name = "Equipment Model")]
        public string EquipModel { get; set; }

        public int qty { get; set; }

        public String Month { get; set; }
    }

    [Table("CPPerformance")]
    public class CPPerformance
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CPPID { get; set; }

        [Display(Name = "Orders Won")]
        public string OrdersWon { get; set; }

        [Display(Name = "Orders Lost")]
        public string OrdersLost { get; set; }

        [Display(Name = "Project Delayed")]
        public string ProjectDelayed { get; set; }

        [Display(Name = "Project Dropped")]
        public string ProjectDropped { get; set; }

        [Display(Name = "OnGoing Projects")]
        public string OnGoingProjects { get; set; }

        public int OrdersWonint { get; set; }
        public int OrdersLostint { get; set; }
        public int ProjectDelayedint { get; set; }
        public int ProjectDroppedint { get; set; }
        public int OnGoingProjectsint { get; set; }
        public String performancepercent { get; set; }
        public int totalvalue { get; set; }
        public String totalvalueCur { get; set; }
    }
    //Lost Order Analysis Report Tables
    [Table("LOA")]
    public class LOA
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
    [Table("LOACCD240")]
    public class LOACCD240
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LOAID { get; set; }

        [Display(Name = "Reason For Losing")]
        public string ReasonForLosing { get; set; }

        public int TotalNumbers { get; set; }
    }
    [Table("LOACCD320")]
    public class LOACCD320
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LOAID { get; set; }

        [Display(Name = "Reason For Losing")]
        public string ReasonForLosing { get; set; }

        public int TotalNumbers { get; set; }
    }
    [Table("LOACMOS160")]
    public class LOACMOS160
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LOAID { get; set; }

        [Display(Name = "Reason For Losing")]
        public string ReasonForLosing { get; set; }

        public int TotalNumbers { get; set; }
    }
    [Table("LOACMOS240")]
    public class LOACMOS240
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LOAID { get; set; }

        [Display(Name = "Reason For Losing")]
        public string ReasonForLosing { get; set; }

        public int TotalNumbers { get; set; }
    }
    [Table("LOACMOS320")]
    public class LOACMOS320
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LOAID { get; set; }

        [Display(Name = "Reason For Losing")]
        public string ReasonForLosing { get; set; }

        public int TotalNumbers { get; set; }
    }

    //OCM Polisher Report
    [Table("OCMPolisherReport")]
    public class OCMPolisherReport
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PRID { get; set; }

        public string FilledBy { get; set; }

        public string Branch { get; set; }

        public Nullable<System.DateTime> Date { get; set; }

        public string CustomerName { get; set; }

        public string Location { get; set; }

        public string Country { get; set; }

        public int Quantity { get; set; }

        public string Product { get; set; }

        public string Capacity { get; set; }

        public string Process { get; set; }

        public string SievePlate { get; set; }

        public string ReducerRing { get; set; }

        public string Drive { get; set; }

        public string Motor { get; set; }

        public string CTCoil { get; set; }

        public string Accessories { get; set; }

        // to store part No &
        public string SievePlatePartNo { get; set; }

        public string ReducerRingPartNo { get; set; }

        public string DrivePartNo { get; set; }

        public string MotorPartNo { get; set; }

        public string CTCoilPartNo { get; set; }

        public string AccessoriesPartNo { get; set; }

        public string ProductModel { get; set; }
    }


    //OCM Polisher Report
    [Table("OCMWhitnerReport")]
    public class OCMWhitnerReport
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int WRID { get; set; }

        public string FilledBy { get; set; }

        public string Branch { get; set; }

        public Nullable<System.DateTime> Date { get; set; }

        public string CustomerName { get; set; }

        public string Location { get; set; }

        public string Country { get; set; }

        public int Quantity { get; set; }

        public string GrainType { get; set; }

        public string Product { get; set; }

        public string Capacity { get; set; }

        public string Process { get; set; }

        public string Stone { get; set; }

        public string SievePlate { get; set; }

        public string Brake { get; set; }

        public string Drive { get; set; }

        public string Motor { get; set; }

        public string CTCoil { get; set; }

        public string StickerPassage { get; set; }

        public string Accessories { get; set; }

        // to store part No &
        public string StonePartNo { get; set; }

        public string SievePlatePartNo { get; set; }

        public string BrakePartNo { get; set; }

        public string DrivePartNo { get; set; }

        public string MotorPartNo { get; set; }

        public string CTCoilPartNo { get; set; }

        public string StickerPassagePartNo { get; set; }

        public string AccessoriesPartNo { get; set; }

        public string MandBStone { get; set; }

        public string Sievemm { get; set; }

        public string ProductModel { get; set; }

        public string WorWdMTR { get; set; }
    }

}