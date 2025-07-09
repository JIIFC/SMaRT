using System;
using System.Collections.Generic;

namespace SMARTV3.Models;

public partial class DataCardV2
{
    public DataCardV2()
    {
        DatacardKpis = new HashSet<DatacardKpi>();
        Conplans = new HashSet<Conplan>();
        Operations = new HashSet<Operation>();
    }
    
    public int Id { get; set; }
    public int? ForceElementId { get; set; }
    public int? DesignationId { get; set; }
    public int? ServiceId { get; set; }
    public int? CapabilityId { get; set; }
    public int? CategoryId { get; set; }
    public string? ForceElementDesc { get; set; }
    public string? Division { get; set; }
    public string? Brigade { get; set; }
    public string? Unit { get; set; }
    public string? Subunit { get; set; }
    public int? EchelonId { get; set; }
    public int? SrStatusId { get; set; }
    public int? CommandOverrideStatusId { get; set; }
    public string? CommandOverrideAuthority { get; set; }
    public string? CommandOverrideComments { get; set; }
    public DateTime? ReadinessFromDate { get; set; }
    public DateTime? ReadinessToDate { get; set; }
    public DateTime Validitydate { get; set; }
    public int? DeployedStatusId { get; set; }
    public int? NoticeToMoveId { get; set; }
    public string? Ntmdetails { get; set; }
    public bool NatoActive { get; set; }
    public string? NatoLocation { get; set; }
    public string? NatoCoordinates { get; set; }
    public string? NatoMajorEquipmentComments { get; set; }
    public int? NatoAssetsDeclared { get; set; }
    public int? NatoStratLiftCapacityId { get; set; }
    public bool NatoStratLiftCapacity { get; set; }
    public string? NatoStratLiftCapacityComments { get; set; }
    public int? NatoNationalDeployId { get; set; }
    public bool NatoNationalDeploy { get; set; }
    public string? NatoNationalDeployComments { get; set; }
    public bool NatoFph { get; set; }
    public int? NatoFphyesNoBlank { get; set; }
    public string? NatoNationalAssesmentComments { get; set; }
    public bool NatoAfstraining { get; set; }
    public int? NatoAfstrainingPercentage { get; set; }
    public string? NatoNationalTrainingRemarks { get; set; }
    public int? NatoCertProgramCoord { get; set; }
    public int? NatoEvalCompleted { get; set; }
    public DateTime? NatoPlannedEvalDate { get; set; }
    public int? NatoCertCompleted { get; set; }
    public string? NatoRequirementName { get; set; }
    public int? NatoNoticeToEffect { get; set; }
    public int? Nato12Sdos { get; set; }
    public int? Nato18Sdos { get; set; }
    public int? NatoCurrentSdos { get; set; }
    public int? NatoNatSupplyPlan { get; set; }
    public int? NatoNatSupportElem { get; set; }
    public string? NatoNationalName { get; set; }
    public string? NatoCavets { get; set; }
    public string? NatoGeneralComments { get; set; }
    public bool FwdNato { get; set; }
    public bool NFMActive { get; set; }
    public bool? DataCardComplete { get; set; }
    public bool Concurrency { get; set; }
    public string? ConcurrencyComments { get; set; }
    public bool Rds { get; set; }
    public int? LastEditUser { get; set; }
    public DateTime? LastEditDate { get; set; }

    // Navigation Properties
    public virtual ForceElement? ForceElement { get; set; }
    public virtual Designation? Designation { get; set; }
    public virtual Service? Service { get; set; }
    public virtual Capability? Capability { get; set; }
    public virtual Category? Category { get; set; }
    public virtual Echelon? Echelon { get; set; }
    public virtual PetsoverallStatus? SrStatus { get; set; }
    public virtual CommandOverideStatus? CommandOverrideStatus { get; set; }
    public virtual DeployedStatus? DeployedStatus { get; set; }
    public virtual NoticeToMove? NoticeToMove { get; set; }
    public virtual NatoStratLiftCapacity? NatoStratLiftCapacityNavigation { get; set; }
    public virtual NatoNationalDeploy? NatoNationalDeployNavigation { get; set; }
    public virtual YesNoBlank? NatoFphyesNoBlankNavigation { get; set; }
    public virtual AfsTrainingPercentage? NatoAfstrainingPercentageNavigation { get; set; }
    public virtual YesNoNaBlank? NatoCertProgramCoordNavigation { get; set; }
    public virtual YesNoNaBlank? NatoEvalCompletedNavigation { get; set; }
    public virtual YesNoNaBlank? NatoCertCompletedNavigation { get; set; }
    public virtual YesNoNaBlank? NatoNatSupplyPlanNavigation { get; set; }
    public virtual YesNoNaBlank? NatoNatSupportElemNavigation { get; set; }
    public virtual YesNoNaBlank? Nato12SdosNavigation { get; set; }
    public virtual YesNoNaBlank? Nato18SdosNavigation { get; set; }
    public virtual User? LastEditUserNavigation { get; set; }

    // Collections
    public virtual ICollection<DatacardKpi> DatacardKpis { get; set; } = new HashSet<DatacardKpi>();
    public virtual ICollection<Conplan> Conplans { get; set; } = new HashSet<Conplan>();
    public virtual ICollection<Operation> Operations { get; set; } = new HashSet<Operation>();
}
