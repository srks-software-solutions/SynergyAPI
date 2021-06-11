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
    [Table("LeadEnquiryRevised")]
    public class LeadEnquiryRevised
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LERID { get; set; }
        
        public string NameofCollector { get; set; }

        public System.DateTime LeadDate { get; set; }

        public System.DateTime VisitDate { get; set; }
                
        public string MillName { get; set; }

        public string MillType { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }
       
        public string Pincode { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Prefix { get; set; }
                        
        public string OwnerName { get; set; }
        
        public string MobNo { get; set; }
                
        public string EmailId { get; set; }

        public string Isd { get; set; }

        public string Std { get; set; }
        
        public string TelNo { get; set; }

        public bool EnquiryTypeExistingMill { get; set; }

        public bool EnquiryTypeNewMill { get; set; }

        public bool EnquiryTypeCustomerServiceUpgrade { get; set; }

        public bool TypeOfMillPaddytoRice { get; set; }

        public bool TypeOfMillBrownRiceToWhiteRice { get; set; }

        public bool TypeOfMillReprocess { get; set; }

        public bool TypeOfPaddyProcessLongGrain { get; set; }

        public bool TypeOfPaddyProcessMediumGrain { get; set; }

        public bool TypeOfPaddyProcessRountGrain { get; set; }

        public bool ConditioningRaw { get; set; }

        public bool ConditioningSteamed { get; set; }

        public bool ConditioningParBoiled { get; set; }

        public string ConditioningParBoiledPaddyMoisture { get; set; }

        public string ConditioningParBoiledBulkDensity { get; set; }

        public bool PaddyVarietyBasmati { get; set; }

        public bool PaddyVarietyNonBasmati { get; set; }

        public string PaddyVarietyVarietyName { get; set; }

        //existing mill details also known as EMD

        public bool EMDProcessingCapacityMill { get; set; }

        public string EMDProcessingCapacityMillTPH { get; set; }

        public string EMDMillOperatingDetailsHoursPerDay { get; set; }

        public bool PreCleaningCapacity { get; set; }

        public bool TPH2025 { get; set; }

        public bool TPH4050 { get; set; }

        public bool Others { get; set; }

        public string OthersTPH { get; set; }

        public string PaddyStorageMTTotal { get; set; }

        public string TotalMT { get; set; }

        public string NoOfSilos { get; set; }


        public string MillDetailsSuppliersName { get; set; }

        public string YearInstalled { get; set; }

        public bool MillSectionHuller { get; set; }

        public string MillSectionNos { get; set; }

        public string MillSectionKW { get; set; }

        public bool MillSectionHullSeperate { get; set; }

        public string MillSectionHullSeperateNos { get; set; }

        public bool PaddyTable { get; set; }

        public string PaddyTableNos { get; set; }

        public bool ThinThickGrader { get; set; }

        public string ThinThickGraderNos { get; set; }

        public string ThinThickGraderDumps { get; set; }


        //for whitener
        public bool Whitner { get; set; }

        public string WhitnerNos { get; set; }

        public string WhitnerKW { get; set; }

        public string WhitnerRPM { get; set; }

        public string WhitnerYear { get; set; }

        //for Plosiher
        public bool Polisher { get; set; }

        public string PolisherNos { get; set; }

        public string PolisherKW { get; set; }

        public string PolisherRPM { get; set; }

        public string PolisherYear { get; set; }

        public bool LengthGrader { get; set; }

        public string LengthGraderNos { get; set; }

        public string GradeSize1 { get; set; }

        public string GradeSize2 { get; set; }

        public string GradeSize3 { get; set; }

        public string GradeSize4 { get; set; }

        public bool ColorSorter { get; set; }

        public string ColorSorterMake { get; set; }

        public string ColorSorterChannels { get; set; }

        public string ColorSorterPrimary { get; set; }

        public string ColorSorterSecondary { get; set; }

        public bool PackingMachine { get; set; }

        public string PackingMachineMake { get; set; }

        public string PackingMachineNos { get; set; }

        public string PackingMachineKgs1 { get; set; }

        public string PackingMachineKgs2 { get; set; }

        public string PackingMachineBrandName { get; set; }

        public bool PlantAutomationTypeRelayOrPLCBased { get; set; }

        public string PlantAutomationTypeRelayOrPLCBasedMake { get; set; }
        
        public string CustomerRequirement1 { get; set; }
        
        public string CustomerRequirement2 { get; set; }
        
        public string CustomerRequirement3 { get; set; }
        
        public string CustomerRequirement4 { get; set; }
        
        public string CustomerRequirement5 { get; set; }
        
        public string CommentsActionExpected { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
        // 0 is open and No update and 1 is Closed and Moved to Quotation
        public int IsStatus { get; set; }

        public int IsDrop { get; set; }
        
        public int CPID { get; set; }
        
        public String LeadTime { get; set; }

        public int IsTime { get; set; }

        public Nullable<int> IsCount { get; set; }

        public Nullable<int> IsHOD { get; set; }

        public Nullable<System.DateTime> NotifyDate { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        //Updated on 19-03-2016

        public bool Cleaner { get; set; }

        public int CleanerNo { get; set; }

        public bool Magnet { get; set; }

        public int MagnetNo { get; set; }

        public bool DeStoner { get; set; }

        public int DeStonerNo { get; set; }

        public bool ColorSorter1 { get; set; }

        public string ColorSorterMake1 { get; set; }

        public string ColorSorterChannels1 { get; set; }

        public string ColorSorterPrimary1 { get; set; }

        public string ColorSorterSecondary1 { get; set; }

        //updatedon 06-07-2016
        public string ColorSorterTertiary { get; set; }

        public string ColorSorterTertiary1 { get; set; }

        public string CustReqYeorsNo { get; set; }     
        
        public string GMCompitator { get; set; }

        public string GLCompitator { get; set; }

        public string Capacity { get; set; }

        public string TypeOfReq { get; set; }

        public bool MachineClassifier { get; set; }

        public bool MachineDestoner { get; set; }

        public bool MachineHullerSeperator { get; set; }

        public bool MachinePaddySeperator { get; set; }

        public bool MachineThickThinGrader { get; set; }

        public bool MachineWhitner { get; set; }

        public bool MachinePolisher { get; set; }

        public bool MachineSorter { get; set; }

        public string TimeFrame { get; set; }

        public string AssignTo { get; set; }

        public bool MachineClassifierMTRA { get; set; }
        public bool MachineDestonerMTSC { get; set; }
        public bool MachineScourerMHXF { get; set; }
        public bool MachineAspirationChannelMVSE { get; set; }
        public bool MachineDampenerMOZJ { get; set; }
        public bool MachineDampenerMOZL { get; set; }
        public bool MachineAutomaticMoistureControlMYFE { get; set; }
        public bool MachineRollerMillMDDK { get; set; }
        public bool MachinePurifierMQRF { get; set; }
        public bool PreCleanerClassifierTAS152 { get; set; }
        public bool DrumSieveMKZM9510 { get; set; }
        public bool PreCleanerClassifierLKGA20 { get; set; }

        //new Fields Added On 12-12-2016

        //for Husker
        public bool Huller { get; set; }

        public string HullerNos { get; set; }

        public string HullerKW { get; set; }

        public string HullerYear { get; set; }

        //for Commodity Type
        public string CommodityType { get; set; }

        //for Enquiry Handled By
        public string EnquiryHandledBy { get; set; }

        public string WhitenerSupplierName { get; set; }

        public string PolisherSupplierName { get; set; }

        public string HullerSupplierName { get; set; }

        public string ColorSorterSupplierName { get; set; }

        public string ColorSorter1SupplierName { get; set; }

        //only Yes/No
        public string QuotesToBeSubmitted { get; set; }

        public string ColorSorter1Year { get; set; }

        public string ColorSorter2Year { get; set; }

        public string SorterModel { get; set; }
        public string leadtype { get; set; }
    }

    [Table("LeadEnquiryRevisedTemp")]
    public class LeadEnquiryRevisedTemp
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LERIDPK { get; set; }

        public int LERID { get; set; }

        public string NameofCollector { get; set; }

        public System.DateTime LeadDate { get; set; }

        public System.DateTime VisitDate { get; set; }

        public string MillName { get; set; }

        public string MillType { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Pincode { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Prefix { get; set; }

        public string OwnerName { get; set; }

        public string MobNo { get; set; }

        public string EmailId { get; set; }

        public string Isd { get; set; }

        public string Std { get; set; }

        public string TelNo { get; set; }

        public bool EnquiryTypeExistingMill { get; set; }

        public bool EnquiryTypeNewMill { get; set; }

        public bool EnquiryTypeCustomerServiceUpgrade { get; set; }

        public bool TypeOfMillPaddytoRice { get; set; }

        public bool TypeOfMillBrownRiceToWhiteRice { get; set; }

        public bool TypeOfMillReprocess { get; set; }

        public bool TypeOfPaddyProcessLongGrain { get; set; }

        public bool TypeOfPaddyProcessMediumGrain { get; set; }

        public bool TypeOfPaddyProcessRountGrain { get; set; }

        public bool ConditioningRaw { get; set; }

        public bool ConditioningSteamed { get; set; }

        public bool ConditioningParBoiled { get; set; }

        public string ConditioningParBoiledPaddyMoisture { get; set; }

        public string ConditioningParBoiledBulkDensity { get; set; }

        public bool PaddyVarietyBasmati { get; set; }

        public bool PaddyVarietyNonBasmati { get; set; }

        public string PaddyVarietyVarietyName { get; set; }

        //existing mill details also known as EMD

        public bool EMDProcessingCapacityMill { get; set; }

        public string EMDProcessingCapacityMillTPH { get; set; }

        public string EMDMillOperatingDetailsHoursPerDay { get; set; }

        public bool PreCleaningCapacity { get; set; }

        public bool TPH2025 { get; set; }

        public bool TPH4050 { get; set; }

        public bool Others { get; set; }

        public string OthersTPH { get; set; }

        public string PaddyStorageMTTotal { get; set; }

        public string TotalMT { get; set; }

        public string NoOfSilos { get; set; }


        public string MillDetailsSuppliersName { get; set; }

        public string YearInstalled { get; set; }

        public bool MillSectionHuller { get; set; }

        public string MillSectionNos { get; set; }

        public string MillSectionKW { get; set; }

        public bool MillSectionHullSeperate { get; set; }

        public string MillSectionHullSeperateNos { get; set; }

        public bool PaddyTable { get; set; }

        public string PaddyTableNos { get; set; }

        public bool ThinThickGrader { get; set; }

        public string ThinThickGraderNos { get; set; }

        public string ThinThickGraderDumps { get; set; }


        //for whitener
        public bool Whitner { get; set; }

        public string WhitnerNos { get; set; }

        public string WhitnerKW { get; set; }

        public string WhitnerRPM { get; set; }

        public string WhitnerYear { get; set; }

        //for Plosiher
        public bool Polisher { get; set; }

        public string PolisherNos { get; set; }

        public string PolisherKW { get; set; }

        public string PolisherRPM { get; set; }

        public string PolisherYear { get; set; }

        public bool LengthGrader { get; set; }

        public string LengthGraderNos { get; set; }

        public string GradeSize1 { get; set; }

        public string GradeSize2 { get; set; }

        public string GradeSize3 { get; set; }

        public string GradeSize4 { get; set; }

        public bool ColorSorter { get; set; }

        public string ColorSorterMake { get; set; }

        public string ColorSorterChannels { get; set; }

        public string ColorSorterPrimary { get; set; }

        public string ColorSorterSecondary { get; set; }

        public bool PackingMachine { get; set; }

        public string PackingMachineMake { get; set; }

        public string PackingMachineNos { get; set; }

        public string PackingMachineKgs1 { get; set; }

        public string PackingMachineKgs2 { get; set; }

        public string PackingMachineBrandName { get; set; }

        public bool PlantAutomationTypeRelayOrPLCBased { get; set; }

        public string PlantAutomationTypeRelayOrPLCBasedMake { get; set; }

        public string CustomerRequirement1 { get; set; }

        public string CustomerRequirement2 { get; set; }

        public string CustomerRequirement3 { get; set; }

        public string CustomerRequirement4 { get; set; }

        public string CustomerRequirement5 { get; set; }

        public string CommentsActionExpected { get; set; }

        public Nullable<System.DateTime> CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public int IsDeleted { get; set; }
        // 0 is open and No update and 1 is Closed and Moved to Quotation
        public int IsStatus { get; set; }

        public int IsDrop { get; set; }

        public int CPID { get; set; }

        public String LeadTime { get; set; }

        public int IsTime { get; set; }

        public Nullable<int> IsCount { get; set; }

        public Nullable<int> IsHOD { get; set; }

        public Nullable<System.DateTime> NotifyDate { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        //Updated on 19-03-2016

        public bool Cleaner { get; set; }

        public int CleanerNo { get; set; }

        public bool Magnet { get; set; }

        public int MagnetNo { get; set; }

        public bool DeStoner { get; set; }

        public int DeStonerNo { get; set; }

        public bool ColorSorter1 { get; set; }

        public string ColorSorterMake1 { get; set; }

        public string ColorSorterChannels1 { get; set; }

        public string ColorSorterPrimary1 { get; set; }

        public string ColorSorterSecondary1 { get; set; }

        //updatedon 06-07-2016
        public string ColorSorterTertiary { get; set; }

        public string ColorSorterTertiary1 { get; set; }

        public string CustReqYeorsNo { get; set; }

        public string GMCompitator { get; set; }

        public string GLCompitator { get; set; }

        public string Capacity { get; set; }

        public string TypeOfReq { get; set; }

        public bool MachineClassifier { get; set; }

        public bool MachineDestoner { get; set; }

        public bool MachineHullerSeperator { get; set; }

        public bool MachinePaddySeperator { get; set; }

        public bool MachineThickThinGrader { get; set; }

        public bool MachineWhitner { get; set; }

        public bool MachinePolisher { get; set; }

        public bool MachineSorter { get; set; }

        public string TimeFrame { get; set; }

        public string AssignTo { get; set; }

        public bool MachineClassifierMTRA { get; set; }
        public bool MachineDestonerMTSC { get; set; }
        public bool MachineScourerMHXF { get; set; }
        public bool MachineAspirationChannelMVSE { get; set; }
        public bool MachineDampenerMOZJ { get; set; }
        public bool MachineDampenerMOZL { get; set; }
        public bool MachineAutomaticMoistureControlMYFE { get; set; }
        public bool MachineRollerMillMDDK { get; set; }
        public bool MachinePurifierMQRF { get; set; }
        public bool PreCleanerClassifierTAS152 { get; set; }
        public bool DrumSieveMKZM9510 { get; set; }
        public bool PreCleanerClassifierLKGA20 { get; set; }

        //new Fields Added On 12-12-2016

        //for Husker
        public bool Huller { get; set; }

        public string HullerNos { get; set; }

        public string HullerKW { get; set; }

        public string HullerYear { get; set; }

        //for Commodity Type
        public string CommodityType { get; set; }

        //for Enquiry Handled By
        public string EnquiryHandledBy { get; set; }

        public string WhitenerSupplierName { get; set; }

        public string PolisherSupplierName { get; set; }

        public string HullerSupplierName { get; set; }

        public string ColorSorterSupplierName { get; set; }

        public string ColorSorter1SupplierName { get; set; }

        //only Yes/No
        public string QuotesToBeSubmitted { get; set; }

        public string ColorSorter1Year { get; set; }

        public string ColorSorter2Year { get; set; }

        public string SorterModel { get; set; }
        public string leadtype { get; set; }
    }

}