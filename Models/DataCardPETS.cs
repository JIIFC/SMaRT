namespace SMARTV3.Models;

public partial class DataCardPETS
{
    public int DataCardId { get; set; }
    public int CapabilityId { get; set; }

    // Personnel
    public int? PersonnelStatusId { get; set; }
    public int PersonnelDesignatedStrength { get; set; }
    public int PersonnelActualStrength { get; set; }
    public bool PersonnelLOB { get; set; }
    public bool PersonnelMedical { get; set; }
    public bool PersonnelDental { get; set; }
    public bool PersonnelAPS { get; set; }
    public bool PersonnelLightDuties { get; set; }
    public bool PersonnelFitness { get; set; }
    public bool PersonnelIT { get; set; }
    public string? PersonnelComments { get; set; }
    public bool PersonnelTradeInsufficienciesReadinessFactor { get; set; }

    // Equipment
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
    public bool EquipmentSensorsReadinessFactor { get; set; }
    public bool EquipmentLifeEntensionDelays { get; set; }
    public bool EquipmentProtectionSuites { get; set; }
    public bool EquipmentOther { get; set; }
    public string? EquipmentComments { get; set; }

    // Training
    public int? TrainingStatusId { get; set; }
    public DateTime? TrainingProjectedCREVALDate { get; set; }
    public int? TrainingCREVAL { get; set; }
    public DateTime? TrainingCREVALDate { get; set; }
    public bool TrainingAmmoReadinessFactor { get; set; }
    public bool TrainingFuelReadinessFactor { get; set; }
    public bool TrainingRationsReadinessFactor { get; set; }
    public bool TrainingWeaponsReadinessFactor { get; set; }
    public bool TrainingRadiosReadinessFactor { get; set; }
    public bool TrainingEquipmentReadinessFactor { get; set; }
    public bool TrainingSparePartssReadinessFactor { get; set; }
    public bool TrainingITReadinessFactor { get; set; }
    public bool TrainingRangeAvailabilityReadinessFactor { get; set; }
    public bool TrainingCTReadinessFactor { get; set; }
    public string? TrainingComments { get; set; }
    public int? TrainingCollectiveTrainingStatusId { get; set; }
    public int? TrainingIndividualTrainingStatusId { get; set; }
    public int? TrainingSpecialtySkillsId { get; set; }
    public bool TrainingSpecialQuals { get; set; }
    public bool TrainingTrgFleets { get; set; }

    // Sustainment
    public int? SustainmentStatusId { get; set; }
    public int? SustainmentCombatRationsStatusId { get; set; }
    public int? SustainmentPersonalEquipmentStatusId { get; set; }
    public int? SustainmentPetrolStatusId { get; set; }
    public int? SustainmentAmmunitionStatusId { get; set; }
    public int? SustainmentOtherStatusId { get; set; }
    public int? SustainmentSparesStatusId { get; set; }
    public bool SustainmentRationsReadinessFactor { get; set; }
    public bool SustainmentUniformsReadinessFactor { get; set; }
    public bool SustainmentPPEReadinessFactor { get; set; }
    public bool SustainmentFuelReadinessFactor { get; set; }
    public bool SustainmentAmmunitionReadinessFactor { get; set; }
    public bool SustainmentOtherReadinessFactor { get; set; }
    public bool SustainmentSparePartsReadinessFactor { get; set; }
    public string? SustainmentComments { get; set; }

    // Navigation Properties
    public virtual Capability? Capability { get; set; }
    public virtual PetsoverallStatus? PetsoverallStatus { get; set; }
    public virtual Creval? TrainingCREVALNavigation { get; set; }
    public virtual SpecialtySkill? TrainingSpecialtySkills { get; set; }
}
