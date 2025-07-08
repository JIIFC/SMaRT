using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class DataCard
    {
        public DataCard()
        {
            DatacardKpis = new HashSet<DatacardKpi>();
            Conplans = new HashSet<Conplan>();
            Operations = new HashSet<Operation>();
        }

        public int Id { get; set; }
        public bool? DataCardComplete { get; set; }
        public int? ForceElementId { get; set; }
        public string? Division { get; set; }
        public string? Brigade { get; set; }
        public string? Unit { get; set; }
        public string? Subunit { get; set; }
        public int? SrStatusId { get; set; }
        public int? CommandOverideStatusId { get; set; }
        public string? CommandOverrideAuthority { get; set; }
        public string? CommandOverrideComments { get; set; }
        public DateTime? ReadinessFromDate { get; set; }
        public DateTime? ReadinessToDate { get; set; }
        public DateTime Validitydate { get; set; }
        public int? DeployedStatusId { get; set; }
        public int? DesignationId { get; set; }
        public int? ServiceId { get; set; }
        public int? EchelonId { get; set; }
        public int? CapabilityId { get; set; }
        public int? CategoryId { get; set; }
        public int? NoticeToMoveId { get; set; }
        public string? Ntmdetails { get; set; }
        public bool NatoActive { get; set; }
        public string? NatoLocation { get; set; }
        public string? NatoCoordinates { get; set; }
        public string? NatoMajorEquipmentComments { get; set; }
        public string? NatoCavets { get; set; }
        public string? NatoGeneralComments { get; set; }
        public int? NatoStratLiftCapacityId { get; set; }
        public bool NatoStratLiftCapacity { get; set; }
        public string? NatoStratLiftCapacityComments { get; set; }
        public int? NatoNationalDeployId { get; set; }
        public bool NatoNationalDeploy { get; set; }
        public string? NatoNationalDeployComments { get; set; }
        public bool NatoFph { get; set; }
        public string? NatoNationalAssesmentComments { get; set; }
        public bool NatoAfstraining { get; set; }
        public int? PersonnelStatusId { get; set; }
        public int PersonnelDesignatedStrength { get; set; }
        public int PersonnelActualStrength { get; set; }
        public bool PersonnelLob { get; set; }
        public bool PersonnelMedical { get; set; }
        public bool PersonnelDental { get; set; }
        public bool PersonnelAps { get; set; }
        public bool PersonnelLightDuties { get; set; }
        public bool PersonnelFitness { get; set; }
        public bool PersonnelIt { get; set; }
        public string? PersonnelComments { get; set; }
        public int? EquipmentStatusId { get; set; }
        public int? EquipmentCombatVehicleStatusId { get; set; }
        public int? EquipmentSupportVehicleStatusId { get; set; }
        public int? EquipmentWeaponsServiceRateStatusId { get; set; }
        public int? EquipmentCommunicationsEquipmentStatusId { get; set; }
        public int? EquipmentSpecialEquipmentStatusId { get; set; }
        public bool EquipmentVehicalsReadinessFactor { get; set; }
        public bool EquipmentWeaponsReadinessFactor { get; set; }
        public bool EquipmentCommunicationsReadinessFactor { get; set; }
        public bool EquipmentSpecialEquimentReadinessFactor { get; set; }
        public bool EquipmentTechniciansReadinessFactor { get; set; }
        public bool EquipmentLifeEntensionDelays { get; set; }
        public bool EquipmentProtectionSuites { get; set; }
        public bool EquipmentOther { get; set; }
        public string? EquipmentComments { get; set; }
        public int? TrainingStatusId { get; set; }
        public DateTime? TrainingProjectedCrevaldate { get; set; }
        public int? TrainingCreval { get; set; }
        public DateTime? TrainingCrevaldate { get; set; }
        public bool TrainingAmmoReadinessFactor { get; set; }
        public bool TrainingFuelReadinessFactor { get; set; }
        public bool TrainingRationsReadinessFactor { get; set; }
        public bool TrainingWeaponsReadinessFactor { get; set; }
        public bool TrainingRadiosReadinessFactor { get; set; }
        public bool TrainingEquipmentReadinessFactor { get; set; }
        public bool TrainingSparePartssReadinessFactor { get; set; }
        public bool TrainingItreadinessFactor { get; set; }
        public bool TrainingRangeAvailabilityReadinessFactor { get; set; }
        public bool TrainingCtreadinessFactor { get; set; }
        public bool TrainingSpecialQuals { get; set; }
        public bool TrainingTrgFleets { get; set; }
        public string? TrainingComments { get; set; }
        public int? TrainingCollectiveTrainingStatusId { get; set; }
        public int? TrainingIndividualTrainingStatusId { get; set; }
        public int? TrainingSpecialtySkillsId { get; set; }
        public int? SustainmentStatusId { get; set; }
        public int? SustainmentCombatRationsStatusId { get; set; }
        public int? SustainmentPersonalEquipmentStatusId { get; set; }
        public int? SustainmentPetrolStatusId { get; set; }
        public int? SustainmentAmmunitionStatusId { get; set; }
        public int? SustainmentOtherStatusId { get; set; }
        public int? SustainmentSparesStatusId { get; set; }
        public bool SustainmentRationsReadinessFactor { get; set; }
        public bool SustainmentUniformsReadinessFactor { get; set; }
        public bool SustainmentPpereadinessFactor { get; set; }
        public bool SustainmentFuelReadinessFactor { get; set; }
        public bool SustainmentAmmunitionReadinessFactor { get; set; }
        public bool SustainmentOtherReadinessFactor { get; set; }
        public bool SustainmentSparePartsReadinessFactor { get; set; }
        public string? SustainmentComments { get; set; }
        public int? LastEditUser { get; set; }
        public DateTime? LastEditDate { get; set; }
        public bool Concurrency { get; set; }
        public string? ConcurrencyCommnets { get; set; }
        public int? NatoAssetsDeclared { get; set; }
        public bool Rds { get; set; }
        public bool EquipmentSensorsReadinessFactor { get; set; }
        public bool PersonnelTradeInsufficienciesReadinessFactor { get; set; }
        public string? NatoRequirementName { get; set; }
        public int? NatoNoticeToEffect { get; set; }
        public int? NatoFphyesNoBlank { get; set; }
        public int? NatoAfstrainingPercentage { get; set; }
        public int? Nato12Sdos { get; set; }
        public int? Nato18Sdos { get; set; }
        public int? NatoCurrentSdos { get; set; }
        public int? NatoNatSupplyPlan { get; set; }
        public int? NatoNatSupportElem { get; set; }
        public string? NatoNationalName { get; set; }
        public string? NatoNationalTrainingRemarks { get; set; }
        public int? NatoEvalCompleted { get; set; }
        public DateTime? NatoPlannedEvalDate { get; set; }
        public int? NatoCertProgramCoord { get; set; }
        public int? NatoCertCompleted { get; set; }
        public bool FwdNato { get; set; }
        public bool NFMActive { get; set; }
        public string? ForceElementDesc { get; set; }

        public virtual Capability? Capability { get; set; }
        public virtual Category? Category { get; set; }
        public virtual CommandOverideStatus? CommandOverideStatus { get; set; }
        public virtual DeployedStatus? DeployedStatus { get; set; }
        public virtual Designation? Designation { get; set; }
        public virtual Echelon? Echelon { get; set; }
        public virtual PetsoverallStatus? EquipmentCombatVehicleStatus { get; set; }
        public virtual PetsoverallStatus? EquipmentCommunicationsEquipmentStatus { get; set; }
        public virtual PetsoverallStatus? EquipmentSpecialEquipmentStatus { get; set; }
        public virtual PetsoverallStatus? EquipmentStatus { get; set; }
        public virtual PetsoverallStatus? EquipmentSupportVehicleStatus { get; set; }
        public virtual PetsoverallStatus? EquipmentWeaponsServiceRateStatus { get; set; }
        public virtual ForceElement? ForceElement { get; set; }
        public virtual User? LastEditUserNavigation { get; set; }
        public virtual YesNoNaBlank? Nato12SdosNavigation { get; set; }
        public virtual YesNoNaBlank? Nato18SdosNavigation { get; set; }
        public virtual AfsTrainingPercentage? NatoAfstrainingPercentageNavigation { get; set; }
        public virtual YesNoNaBlank? NatoCertCompletedNavigation { get; set; }
        public virtual YesNoNaBlank? NatoCertProgramCoordNavigation { get; set; }
        public virtual YesNoNaBlank? NatoEvalCompletedNavigation { get; set; }
        public virtual YesNoBlank? NatoFphyesNoBlankNavigation { get; set; }
        public virtual YesNoNaBlank? NatoNatSupplyPlanNavigation { get; set; }
        public virtual YesNoNaBlank? NatoNatSupportElemNavigation { get; set; }
        public virtual NatoNationalDeploy? NatoNationalDeployNavigation { get; set; }
        public virtual NatoStratLiftCapacity? NatoStratLiftCapacityNavigation { get; set; }
        public virtual NoticeToMove? NoticeToMove { get; set; }
        public virtual PetsoverallStatus? PersonnelStatus { get; set; }
        public virtual Service? Service { get; set; }
        public virtual PetsoverallStatus? SrStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentAmmunitionStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentCombatRationsStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentOtherStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentPersonalEquipmentStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentPetrolStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentSparesStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentStatus { get; set; }
        public virtual PetsoverallStatus? TrainingCollectiveTrainingStatus { get; set; }
        public virtual Creval? TrainingCrevalNavigation { get; set; }
        public virtual PetsoverallStatus? TrainingIndividualTrainingStatus { get; set; }
        public virtual SpecialtySkill? TrainingSpecialtySkills { get; set; }
        public virtual PetsoverallStatus? TrainingStatus { get; set; }
        public virtual ICollection<DatacardKpi> DatacardKpis { get; set; }

        public virtual ICollection<Conplan> Conplans { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
    }
}
