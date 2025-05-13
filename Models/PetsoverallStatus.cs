using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class PetsoverallStatus
    {
        public PetsoverallStatus()
        {
            ChangeLogEquipmentStatuses = new HashSet<ChangeLog>();
            ChangeLogPersonnelStatuses = new HashSet<ChangeLog>();
            ChangeLogSrStatuses = new HashSet<ChangeLog>();
            ChangeLogSustainmentStatuses = new HashSet<ChangeLog>();
            ChangeLogTrainingStatuses = new HashSet<ChangeLog>();
            DataCardEquipmentCombatVehicleStatuses = new HashSet<DataCard>();
            DataCardEquipmentCommunicationsEquipmentStatuses = new HashSet<DataCard>();
            DataCardEquipmentSpecialEquipmentStatuses = new HashSet<DataCard>();
            DataCardEquipmentStatuses = new HashSet<DataCard>();
            DataCardEquipmentSupportVehicleStatuses = new HashSet<DataCard>();
            DataCardEquipmentWeaponsServiceRateStatuses = new HashSet<DataCard>();
            DataCardHistoryEquipmentStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryPersonnelStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySrStatuses = new HashSet<DataCardHistory>();
            DataCardHistorySustainmentStatuses = new HashSet<DataCardHistory>();
            DataCardHistoryTrainingStatuses = new HashSet<DataCardHistory>();
            DataCardPersonnelStatuses = new HashSet<DataCard>();
            DataCardSrStatuses = new HashSet<DataCard>();
            DataCardSustainmentAmmunitionStatuses = new HashSet<DataCard>();
            DataCardSustainmentCombatRationsStatuses = new HashSet<DataCard>();
            DataCardSustainmentOtherStatuses = new HashSet<DataCard>();
            DataCardSustainmentPersonalEquipmentStatuses = new HashSet<DataCard>();
            DataCardSustainmentPetrolStatuses = new HashSet<DataCard>();
            DataCardSustainmentSparesStatuses = new HashSet<DataCard>();
            DataCardSustainmentStatuses = new HashSet<DataCard>();
            DataCardTrainingCollectiveTrainingStatuses = new HashSet<DataCard>();
            DataCardTrainingIndividualTrainingStatuses = new HashSet<DataCard>();
            DataCardTrainingStatuses = new HashSet<DataCard>();
            DatacardKpiOverallStatusAboves = new HashSet<DatacardKpi>();
            DatacardKpiOverallStatusBelows = new HashSet<DatacardKpi>();
            DummyDataCardEquipmentCombatVehicleStatuses = new HashSet<DummyDataCard>();
            DummyDataCardEquipmentCommunicationsEquipmentStatuses = new HashSet<DummyDataCard>();
            DummyDataCardEquipmentSpecialEquipmentStatuses = new HashSet<DummyDataCard>();
            DummyDataCardEquipmentStatuses = new HashSet<DummyDataCard>();
            DummyDataCardEquipmentSupportVehicleStatuses = new HashSet<DummyDataCard>();
            DummyDataCardEquipmentWeaponsServiceRateStatuses = new HashSet<DummyDataCard>();
            DummyDataCardPersonnelStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSrStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentAmmunitionStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentCombatRationsStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentOtherStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentPersonalEquipmentStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentPetrolStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentSparesStatuses = new HashSet<DummyDataCard>();
            DummyDataCardSustainmentStatuses = new HashSet<DummyDataCard>();
            DummyDataCardTrainingCollectiveTrainingStatuses = new HashSet<DummyDataCard>();
            DummyDataCardTrainingIndividualTrainingStatuses = new HashSet<DummyDataCard>();
            DummyDataCardTrainingStatuses = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string? StatusDisplayColour { get; set; }
        public string? StatusDisplayValue { get; set; }
        public int? StatusValue { get; set; }
        public int? Ordering { get; set; }
        public bool? Archived { get; set; }

        public virtual ICollection<ChangeLog> ChangeLogEquipmentStatuses { get; set; }
        public virtual ICollection<ChangeLog> ChangeLogPersonnelStatuses { get; set; }
        public virtual ICollection<ChangeLog> ChangeLogSrStatuses { get; set; }
        public virtual ICollection<ChangeLog> ChangeLogSustainmentStatuses { get; set; }
        public virtual ICollection<ChangeLog> ChangeLogTrainingStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardEquipmentCombatVehicleStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardEquipmentCommunicationsEquipmentStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardEquipmentSpecialEquipmentStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardEquipmentStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardEquipmentSupportVehicleStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardEquipmentWeaponsServiceRateStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryEquipmentStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryPersonnelStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySrStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistorySustainmentStatuses { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistoryTrainingStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardPersonnelStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSrStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentAmmunitionStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentCombatRationsStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentOtherStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentPersonalEquipmentStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentPetrolStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentSparesStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardSustainmentStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardTrainingCollectiveTrainingStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardTrainingIndividualTrainingStatuses { get; set; }
        public virtual ICollection<DataCard> DataCardTrainingStatuses { get; set; }
        public virtual ICollection<DatacardKpi> DatacardKpiOverallStatusAboves { get; set; }
        public virtual ICollection<DatacardKpi> DatacardKpiOverallStatusBelows { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardEquipmentCombatVehicleStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardEquipmentCommunicationsEquipmentStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardEquipmentSpecialEquipmentStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardEquipmentStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardEquipmentSupportVehicleStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardEquipmentWeaponsServiceRateStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardPersonnelStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSrStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentAmmunitionStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentCombatRationsStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentOtherStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentPersonalEquipmentStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentPetrolStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentSparesStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardSustainmentStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardTrainingCollectiveTrainingStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardTrainingIndividualTrainingStatuses { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCardTrainingStatuses { get; set; }
    }
}
