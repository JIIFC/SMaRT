using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class PetspercentStatus
    {
        public PetspercentStatus()
        {
            DataCardHistoryEquipmentCombatVehicleStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryEquipmentCommunicationsEquipmentStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryEquipmentSpecialEquipmentStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryEquipmentSupportVehicleStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryEquipmentWeaponsServiceRateStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentAmmunitionStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentCombatRationsStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentOtherStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentPersonalEquipmentStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentPetrolStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentSparesStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryTrainingCollectiveTrainingStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryTrainingIndividualTrainingStatuses = new HashSet<DataCardHistory>();
        }

        public int Id { get; set; }
        public string? StatusDisplayColour { get; set; }
        public string? StatusDisplayValue { get; set; }
        public int? StatusValue { get; set; }
        public int? Ordering { get; set; }
        public bool? Archived { get; set; }

        public virtual ICollection<DataCardHistory> DataCardHistoryEquipmentCombatVehicleStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryEquipmentCommunicationsEquipmentStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryEquipmentSpecialEquipmentStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryEquipmentSupportVehicleStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryEquipmentWeaponsServiceRateStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentAmmunitionStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentCombatRationsStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentOtherStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentPersonalEquipmentStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentPetrolStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentSparesStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryTrainingCollectiveTrainingStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryTrainingIndividualTrainingStatuses { get; set; }
    }
}
