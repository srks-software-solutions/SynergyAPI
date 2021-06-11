using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SRKSSynergy.Models
{
    public class SRKS_Synergy : DbContext
    {
        public SRKS_Synergy()
            : base("SRKS_Synergy")
        {
        }
        public DbSet<MDBGeneralData> MDBGeneralData { get; set; }
        public DbSet<SOT> SOT { get; set; }
        public DbSet<SOTRM> SOTRM { get; set; }

        public DbSet<QGEquipGeneralData> QGEquipGeneralData { get; set; }
        public DbSet<QGEquipPayment> QGEquipPayment { get; set; }
        public DbSet<QGEquipTableData> QGEquipTableData { get; set; }
        public DbSet<QGSpareGeneralData> QGSpareGeneralData { get; set; }
        public DbSet<QGSparePayment> QGSparePayment { get; set; }
        public DbSet<QGSpareTableData> QGSpareTableData { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<MasterProducts> MasterProducts { get; set; }
        public DbSet<ProductModel> ProductModel { get; set; }
        public DbSet<ProductModelSpare> ProductModelSpare { get; set; }
        public DbSet<MDBContactPersonData> MDBContactPersonData { get; set; }
        public DbSet<MDBStatutoryNumber> MDBStatutoryNumber { get; set; }
        public DbSet<MDBBankDetail> MDBBankDetail { get; set; }
        public DbSet<ChannelPartners> ChannelPartners { get; set; }
        public DbSet<CPBankDetails> CPBankDetails { get; set; }
        public DbSet<CPContactPersonData> CPContactPersonData { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<LoginData> LoginData { get; set; }
        public DbSet<RiceOAEquipGeneralData> RiceOAEquipGeneralData { get; set; }
        public DbSet<RiceOAReportDBSheet> RiceOAReportDBSheet { get; set; }
        public DbSet<RiceOAEquipPayment> RiceOAEquipPayment { get; set; }
        public DbSet<RiceOAEquipTableData> RiceOAEquipTableData { get; set; }
        public DbSet<OAEquipGeneralData> OAEquipGeneralData { get; set; }
        public DbSet<OAEquipPayment> OAEquipPayment { get; set; }
        public DbSet<OAEquipTableData> OAEquipTableData { get; set; }
        public DbSet<MinSpareEquipQuantity> MinSpareEquipQuantity { get; set; }
        public DbSet<AvailSpareQuantity> AvailSpareQuantity { get; set; }
        public DbSet<Handover> Handover { get; set; }
        public DbSet<LOABarReport> LostOrderAnalysis { get; set; }
        public DbSet<MFR> MFR { get; set; }
        public DbSet<MFRParts> MFRParts { get; set; }
        public DbSet<MFRBBAN> MFRBBAN { get; set; }
        public DbSet<Warranty> Warranty { get; set; }
        public DbSet<EquipSalesByVol> EquipSalesByVol { get; set; }
        public DbSet<EquipSalesByVal> EquipSalesByVal { get; set; }
        public DbSet<SOTByVol> SOTByVol { get; set; }
        public DbSet<SOTByVolCP> SOTByVolCP { get; set; }
        public DbSet<CPPerformance> CPPerformance { get; set; }
        public DbSet<LOASum> LOASum { get; set; }
        public DbSet<LOA> LOA { get; set; }
        public DbSet<LOACCD240> LOACCD240 { get; set; }
        public DbSet<LOACCD320> LOACCD320 { get; set; }
        public DbSet<LOACMOS160> LOACMOS160 { get; set; }
        public DbSet<LOACMOS240> LOACMOS240 { get; set; }
        public DbSet<LOACMOS320> LOACMOS320 { get; set; }
        public DbSet<EquipSalesByVolAck> EquipSalesByVolAck { get; set; }
        public DbSet<MachineInventory> MachineInventory { get; set; }
        public DbSet<MachineInvoiced> MachineInvoiced { get; set; }
        public DbSet<MachineDispatch> MachineDispatch { get; set; }
        public DbSet<UserLoginData> InwardSpare { get; set; }
        public DbSet<OutwardSpare> OutwardSpare { get; set; }
        public DbSet<OutwardMFR> OutwardMFR { get; set; }
        public DbSet<RiceOAReportDataTable> RiceOAReportDataTable { get; set; }
        public DbSet<MailMasters> MailMasters { get; set; }
        public DbSet<Zone> Zone { get; set; }
        public DbSet<LeadEnquiry> LeadEnquiry { get; set; }
        public DbSet<MailSendCredentials> MailSendCredentials { get; set; }
        public DbSet<BuhlerAdminCC> BuhlerAdminCC { get; set; }
        public DbSet<CPMailIdTO> CPMailIdTO { get; set; }
        public DbSet<TargetSettings> TargetSettings { get; set; }
        public DbSet<AutoMailer> AutoMailer { get; set; }
        public DbSet<AutoMailSystem> AutoMailSystem { get; set; }
        public DbSet<SOT_Temp_tbl> SOT_Temp_tbl { get; set; }
        public DbSet<OverAllLeadStatus> OverAllLeadStatus { get; set; }
        public DbSet<MailingTargets> MailingTargets { get; set; }
        public DbSet<RiceMillHOD> RiceMillHOD { get; set; }
        public DbSet<LeadEnquiryRevised> LeadEnquiryRevised { get; set; }
        public DbSet<LeadEnquiryRevisedTemp> LeadEnquiryRevisedTemp { get; set; }
        public DbSet<OCMMotor> OCMMotor { get; set; }
        public DbSet<OCMDrive> OCMDrive { get; set; }
        public DbSet<OCMStone> OCMStone { get; set; }
        public DbSet<OCMSieve> OCMSieve { get; set; }
        public DbSet<OCMBrake> OCMBrake { get; set; }
        public DbSet<OCMAccessories> OCMAccessories { get; set; }
        public DbSet<OCMPassageSticker> OCMPassageSticker { get; set; }
        public DbSet<OCMCTCoil> OCMCTCoil { get; set; }
        public DbSet<OCMReducerRing> OCMReducerRing { get; set; }

        public DbSet<OCMPolisher> OCMPolisher { get; set; }
        public DbSet<OCMWhitner> OCMWhitner { get; set; }

        public DbSet<OCMPolisherReport> OCMPolisherReport { get; set; }
        public DbSet<OCMWhitnerReport> OCMWhitnerReport { get; set; }

        public DbSet<OCMGrainType> OCMGrainType { get; set; }
        public DbSet<OCMProcess> OCMProcess { get; set; }
        public DbSet<OCMPass> OCMPass { get; set; }
        public DbSet<OCMCapacityTPH> OCMCapacityTPH { get; set; }
        public DbSet<OCMCapacityKW> OCMCapacityKW { get; set; }
        public DbSet<OCMDriveMS> OCMDriveMS { get; set; }
        public DbSet<OCMStoneGrit> OCMStoneGrit { get; set; }
        public DbSet<OCMSieveslot> OCMSieveslot { get; set; }
        public DbSet<OCMBrakechamfer> OCMBrakechamfer { get; set; }
        public DbSet<OCMAccessoriesNew> OCMAccessoriesNew { get; set; }
        public DbSet<OCMProcessname> OCMProcessname { get; set; }
        public DbSet<OCMPassname> OCMPassname { get; set; }
        public DbSet<OCMReducerRingNew> OCMReducerRingNew { get; set; }
        public DbSet<OCMCTCoilNew> OCMCTCoilNew { get; set; }

        public DbSet<OASAPDetails> OASAPDetails { get; set; }

        public DbSet<OverallReport> OverallReport { get; set; }

        public DbSet<TargetSettingsLead> TargetSettingsLead { get; set; }

        public DbSet<TestTable> TestTable { get; set; }  //only when need to update db

        public DbSet<LeadFollowUptbl> LeadFollowUptbl { get; set; }

        public DbSet<DistrictPinCodeDetails> DistrictPinCodeDetails_tbl { get; set; }

        public DbSet<States_tbl> States_tbl { get; set; }


        //Add new method to use stored procedure to insert data
        public void QgtoOAGen(int qgid)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoOA_General @QGID", new SqlParameter("@QGID", qgid));
        }
        public void QgtoOAPay(int qgid, int oaid1)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoOA_Payment @QGID,@OAID", new SqlParameter("@QGID", qgid), new SqlParameter("@OAID", oaid1));
        }
        public void QgtoOATAb(int qgid, int oaid1)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoOA_Table @QGID,@OAID", new SqlParameter("@QGID", qgid), new SqlParameter("@OAID", oaid1));
        }
        public void MFRadmin(int mfrid)
        {
            this.Database.ExecuteSqlCommand("exec sp_mfr_admin @MFRID", new SqlParameter("@MFRID", mfrid));
        }
        //Add new method to use stored procedure to insert data
        public void QgtoRiceOAGen(int qgid)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoRiceOA_General @QGID", new SqlParameter("@QGID", qgid));
        }
        public void QgtoRiceOAPay(int qgid, int oaid2)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoRiceOA_Payment @QGID,@ROAID", new SqlParameter("@QGID", qgid), new SqlParameter("@ROAID", oaid2));
        }
        public void QgtoRiceOATAb(int qgid, int oaid2,int productid,int prodmodelid,int masterprodid)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoRiceOA_Table @QGID,@ROAID,@ProductID,@ProductModelID,@MasterProductID", new SqlParameter("@QGID", qgid), new SqlParameter("@ROAID", oaid2), new SqlParameter("@ProductID", productid), new SqlParameter("@ProductModelID", prodmodelid), new SqlParameter("@MasterProductID", masterprodid));
        }

        //For storing the Whitiner and Polisher individually
        public void QgtoRiceOATAbOCM(int qgid, int oaid2, int productid, int prodmodelid, int masterprodid,int qty,string UnitPrice)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtoRiceOA_TableOCM @QGID,@ROAID,@ProductID,@ProductModelID,@MasterProductID,@Quantity,@UnitPrice,@TotalPrice", new SqlParameter("@QGID", qgid), new SqlParameter("@ROAID", oaid2), new SqlParameter("@ProductID", productid), new SqlParameter("@ProductModelID", prodmodelid), new SqlParameter("@MasterProductID", masterprodid), new SqlParameter("@Quantity", qty), new SqlParameter("@UnitPrice", UnitPrice), new SqlParameter("@TotalPrice", UnitPrice));
        }

        public void QgtoLost(int qgid, int oaid3)
        {
            this.Database.ExecuteSqlCommand("exec sp_move_QGtolostorder @QGID,@QGID", new SqlParameter("@QGID", qgid), new SqlParameter("@QGID", oaid3));
        }
        //Report for Channel Partner Performance
        public int CpperOrderlost(int cpid, DateTime fromdat, DateTime todat)
        {
            try
            {
                SqlParameter outp = new SqlParameter("@TotalPrice", System.Data.SqlDbType.Int);
                outp.Direction = System.Data.ParameterDirection.Output;
                this.Database.ExecuteSqlCommand("exec sp_cpper_orderlost @CPID,@Fromdat,@Todat,@TotalPrice OUTPUT", new SqlParameter("@CPID", cpid), new SqlParameter("@Fromdat", fromdat), new SqlParameter("@Todat", todat), outp);
                return Convert.ToInt32(outp.Value);
            }
            catch
            {
            }
            return 0;
        }
        public int CpperProdeldrop(int cpid, DateTime fromdat, DateTime todat, string ordact)
        {
            try
            {
                SqlParameter outp = new SqlParameter("@TotalPrice", System.Data.SqlDbType.Int);
                outp.Direction = System.Data.ParameterDirection.Output;
                this.Database.ExecuteSqlCommand("exec sp_cpper_projdeldrop @CPID,@Fromdat,@Todat,@Orderactive,@TotalPrice OUTPUT", new SqlParameter("@CPID", cpid), new SqlParameter("@Fromdat", fromdat), new SqlParameter("@Todat", todat), new SqlParameter("@Orderactive", ordact), outp);
                return Convert.ToInt32(outp.Value);
            }
            catch
            {
            }
            return 0;
        }
        public int CpperExistquot(int cpid, DateTime fromdat, DateTime todat)
        {
            try
            {
                SqlParameter outp = new SqlParameter("@TotalPrice", System.Data.SqlDbType.Int);
                outp.Direction = System.Data.ParameterDirection.Output;
                int value = this.Database.ExecuteSqlCommand("exec sp_cpper_existquotsum @CPID,@Fromdat,@Todat,@TotalPrice OUTPUT", new SqlParameter("@CPID", cpid), new SqlParameter("@Fromdat", fromdat), new SqlParameter("@Todat", todat), outp);
                return Convert.ToInt32(outp.Value);
            }
            catch
            {
            }
            return 0;
        }
        //
        //Report for ALL Channel Partner Performance
        public int CpperOrderlostall(DateTime fromdat, DateTime todat)
        {
            try
            {
                SqlParameter outp = new SqlParameter("@TotalPrice", System.Data.SqlDbType.Int);
                outp.Direction = System.Data.ParameterDirection.Output;
                this.Database.ExecuteSqlCommand("exec sp_cpper_orderlost_all @Fromdat,@Todat,@TotalPrice OUTPUT", new SqlParameter("@Fromdat", fromdat), new SqlParameter("@Todat", todat), outp);
                return Convert.ToInt32(outp.Value);
            }
            catch
            {
            }
            return 0;
        }
        public int CpperProdeldropall(DateTime fromdat, DateTime todat, string ordact)
        {
            try
            {
                SqlParameter outp = new SqlParameter("@TotalPrice", System.Data.SqlDbType.Int);
                outp.Direction = System.Data.ParameterDirection.Output;
                this.Database.ExecuteSqlCommand("exec sp_cpper_projdeldrop_all @Fromdat,@Todat,@Orderactive,@TotalPrice OUTPUT", new SqlParameter("@Fromdat", fromdat), new SqlParameter("@Todat", todat), new SqlParameter("@Orderactive", ordact), outp);
                return Convert.ToInt32(outp.Value);
            }
            catch
            {
            }
            return 0;
        }
        public int CpperExistquotall(DateTime fromdat, DateTime todat)
        {
            try
            {
                SqlParameter outp = new SqlParameter("@TotalPrice", System.Data.SqlDbType.Int);
                outp.Direction = System.Data.ParameterDirection.Output;
                int value = this.Database.ExecuteSqlCommand("exec sp_cpper_existquotsum_all @Fromdat,@Todat,@TotalPrice OUTPUT", new SqlParameter("@Fromdat", fromdat), new SqlParameter("@Todat", todat), outp);
                return Convert.ToInt32(outp.Value);
            }
            catch
            {
            }
            return 0;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

    }
}