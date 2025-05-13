namespace SMARTV3.Models
{
    public class DatacardReadinessTableModel
    {
        public string? AverageDays;
        public int TotalCount;
        public double TotalDays;
        public decimal TotalWeight;

        // Readiness counts
        public int ReadyCount;

        public int ReadyLimsCount;
        public int CombatIneffectiveCount;
        public int NotReadyCount;

        // Deployed counts
        public int DeployedCount;

        public int PartiallyDeployedCount;
        public int ForwardToNatoCount;
        public int HighAlertCount;
        public int ForceGeneratingCount;
        public int InDevelopmentCount;
        public int NotDeployedCount;

        // Readiness percentages
        public string? ReadyPercentage;

        public string? ReadyLimsPercentatge;
        public string? CombatIneffectivePercentage;
        public string? NotReadyPercentage;

        // Deployed percentages
        public string? DeployedPercentage;

        public string? PartiallyDeployedPercentage;
        public string? ForwardToNatoPercentage;
        public string? HighAlertPercentage;
        public string? ForceGeneratingPercentage;
        public string? InDevelopmentPercentage;
        public string? NotDeployedPercentage;

        // Readiness weighted percentages
        public string? ReadyWeightPercentage;

        public string? ReadyLimsWeightPercentage;
        public string? CombatIneffectiveWeightPercentage;
        public string? NotReadyWeightPercentage;

        // Readiness weighted sums
        public decimal ReadyWeightSum;

        public decimal ReadyLimsWeightSum;
        public decimal CombatIneffectiveWeightSum;
        public decimal NotReadyWeightSum;

        // Deployed weighted percentages
        public string? DeployedWeightPercentage;

        public string? PartiallyDeployedWeightPercentage;
        public string? ForwardToNatoWeightPercentage;
        public string? HighAlertWeightPercentage;
        public string? ForceGeneratingWeightPercentage;
        public string? InDevelopmentWeightPercentage;
        public string? NotDeployedWeightPercentage;

        // Deployed weighted sums
        public decimal DeployedWeightSum;

        public decimal PartiallyDeployedWeightSum;
        public decimal ForwardToNatoWeightSum;
        public decimal HighAlertWeightSum;
        public decimal ForceGeneratingWeightSum;
        public decimal InDevelopmentWeightSum;
        public decimal NotDeployedWeightSum;

        // Ready with limits PETS percentages
        public string? YellowStatusPersonnelPercentage;

        public string? YellowStatusEquipmentPercentage;
        public string? YellowStatusTrainingPercentage;
        public string? YellowStatusSustainmentPercentage;

        // Combat Ineffective PETS percentages
        public string? OrangeStatusPersonnelPercentage;

        public string? OrangeStatusEquipmentPercentage;
        public string? OrangeStatusTrainingPercentage;
        public string? OrangeStatusSustainmentPercentage;

        // Not Ready PETS percentages
        public string? RedStatusPersonnelPercentage;

        public string? RedStatusEquipmentPercentage;
        public string? RedStatusTrainingPercentage;
        public string? RedStatusSustainmentPercentage;
    }
}