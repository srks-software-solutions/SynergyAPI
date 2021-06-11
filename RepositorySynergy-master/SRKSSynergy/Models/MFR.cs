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
    public class MFRALL
    {
        public MFR MFR { get; set; }

        public MFRParts MFRParts1 { get; set; }

        public MFRParts MFRParts2 { get; set; }

        public MFRParts MFRParts3 { get; set; }

        public MFRParts MFRParts4 { get; set; }

        public MFRParts MFRParts5 { get; set; }

        
        public MFRBBAN MFRBBAN { get; set; }
    }
    [Table("MFR")]
    public class MFR
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MFRID { get; set; }

        [Display(Name = "MFR Number")]
        public string MFRNumber { get; set; }

        public string OrderNum { get; set; }

        [StringLength(40, MinimumLength = 3,ErrorMessage="Minimum is 3 and Maximum is 20")]
        public string MfrTo { get; set; }

        public string MfrDate { get; set; }

        public bool MacBreakDown { get; set; }

        public bool MacOperatTemp { get; set; }

        public string MacSlNo { get; set; }

        [StringLength(40, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string CommissionedBy { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //public Nullable<System.DateTime> CommissionedDate { get; set; }
        public string CommissionedDate { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Fault { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Ask1 { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Ask2 { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Ask3 { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Ask4 { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Ask5 { get; set; }

        //[StringLength(160, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 160")]
        public string Remedial { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int CPID { get; set; }

        public int MDBID { get; set; }
        public virtual MDBGeneralData MDBGeneralData { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string MfrEnteredBy { get; set; }

        public string MfrModelNo { get; set; }

        public int IsMFR { get; set; }

        public int ApprovalStatus { get; set; }

        [StringLength(160, MinimumLength = 4, ErrorMessage = "Minimum is 4 and Maximum is 20")]
        public string MFRComment { get; set; }

        public int IsSpareTakenFromExistingCPStock { get; set; }

        public int IsSpareNeededFromBBAN { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNo { get; set; }

        public virtual ICollection<MFRParts> MFRParts { get; set; }

       

    }

    [Table("MFRParts")]
    public class MFRParts
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MFRPID { get; set; }

        public int MFRID { get; set; }

        public int ProductModelSparesID { get; set; }

        public int Quantity { get; set; }

        public string ModelDesc { get; set; }

        public virtual ProductModelSpare ProductModelSpare { get; set; }

    }

    [Table("MFRBBAN")]
    public class MFRBBAN
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int MFRBBID { get; set; }

        public int MFRID { get; set; }

        public int CPID { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string RecdOn { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string MFRProcessedBy { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string MFRNoYear { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string TANoDat { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Minimum is 3 and Maximum is 20")]
        public string Code { get; set; }

        public string AODDat { get; set; }

        public string DispatchDat { get; set; }

        public string MfrAdminDat { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }


    }

    public class ReportModelMFR
    {
        public int MFRID { get; set; }

        public string MFREnteredBy { get; set; }

        public string MFRTo { get; set; }

        public string ChannelPartner { get; set; }

        public string OrganizationName { get; set; }

        public string City { get; set; }

        public string ContactPersonName { get; set; }

        public string Mobile { get; set; }

        public string Modelno { get; set; }

        public string MacSlNo { get; set; }

        public string CommissionedBy { get; set; }

        public string CommDate { get; set; }

        public bool McBrkDwn { get; set; }

        public bool McOpTem { get; set; }

        public string MfrDate { get; set; }

        public string Fault1 { get; set; }

        public string Ask1 { get; set; }

        public string Ask2 { get; set; }

        public string Ask3 { get; set; }

        public string Ask4 { get; set; }

        public string Ask5 { get; set; }

        public string Remedial { get; set; }

        public string Logo { get; set; }

        public string MFRNumber { get; set; }

        public string RecdOn { get; set; }

        public string MFRProcessedBy { get; set; }

        public string MFRNoYear { get; set; }

        public string TANoDat { get; set; }

        public string Code { get; set; }

        public string AODDat { get; set; }

        public string DispatchDat { get; set; }

        public string HODNumber { get; set; }

        public IList<MFRParts> MFRParts { get; set; }
    }
    
}