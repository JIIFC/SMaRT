using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ChangeLog
    {
        public int Id { get; set; }
        public int? ForceElementId { get; set; }
        public int? SrStatusId { get; set; }
        public int? CommandOverideStatusId { get; set; }
        public string? CommandOverrideAuthority { get; set; }
        public string? CommandOverrideComments { get; set; }
        public int? DeployedStatusId { get; set; }
        public string? NatoGeneralComments { get; set; }
        public string? NatoMajorEquipmentComments { get; set; }
        public string? NatoCavets { get; set; }
        public string? NatoStratLiftCapacityComments { get; set; }
        public string? NatoNationalDeployComments { get; set; }
        public string? NatoNationalAssesmentComments { get; set; }
        public int? PersonnelStatusId { get; set; }
        public string? PersonnelComments { get; set; }
        public int? TrainingStatusId { get; set; }
        public string? TrainingComments { get; set; }
        public int? EquipmentStatusId { get; set; }
        public string? EquipmentComments { get; set; }
        public int? SustainmentStatusId { get; set; }
        public string? SustainmentComments { get; set; }
        public DateTime? ChangedDate { get; set; }
        public int? LastEditUser { get; set; }

        public virtual CommandOverideStatus? CommandOverideStatus { get; set; }
        public virtual DeployedStatus? DeployedStatus { get; set; }
        public virtual PetsoverallStatus? EquipmentStatus { get; set; }
        public virtual ForceElement? ForceElement { get; set; }
        public virtual User? LastEditUserNavigation { get; set; }
        public virtual PetsoverallStatus? PersonnelStatus { get; set; }
        public virtual PetsoverallStatus? SrStatus { get; set; }
        public virtual PetsoverallStatus? SustainmentStatus { get; set; }
        public virtual PetsoverallStatus? TrainingStatus { get; set; }
    }
}
