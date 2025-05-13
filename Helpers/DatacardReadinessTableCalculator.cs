using System.Globalization;

using SMARTV3.Models;

namespace SMARTV3.Helpers
{
    public class DatacardReadinessTableCalculator
    {
        private int totalCount = 0;
        private int readyCount = 0;
        private int readyLimsCount = 0;
        private int combatIneffectiveCount = 0;
        private int notReadyCount = 0;
        
        private int deployedCount = 0;
        private int partiallyDeployedCount = 0;
        private int forwardToNatoCount = 0;
        private int highAlertCount = 0;
        private int forceGeneratingCount = 0;
        private int inDevelopmentCount = 0;
        private int notDeployedCount = 0;

        private decimal readyLimsPersWeight = 0;
        private decimal readyLimsEquipWeight = 0;
        private decimal readyLimsTrainWeight = 0;
        private decimal readyLimsSustWeight = 0;

        private decimal combatIneffPersWeight = 0;
        private decimal combatIneffEquipWeight = 0;
        private decimal combatIneffTrainWeight = 0;
        private decimal combatIneffSustWeight = 0;

        private decimal notReadyPersWeight = 0;
        private decimal notReadyEquipWeight = 0;
        private decimal notReadyTrainWeight = 0;
        private decimal notReadySustWeight = 0;

        private double totalDays = 0;
        // Calculate weighting

        private decimal readyWeight = 0;
        private decimal readyLimsWeight = 0;
        private decimal combatIneffectiveWeight = 0;
        private decimal notReadyWeight = 0;

        private decimal deployedWeight = 0;
        private decimal partiallyDeployedWeight = 0;
        private decimal forwardToNatoWeight = 0;
        private decimal highAlertWeight = 0;
        private decimal forceGeneratingWeight = 0;
        private decimal inDevelopmentWeight = 0;
        private decimal notDeployedWeight = 0;
        private decimal totalWeight = 0;

        public DatacardReadinessTableModel? CalculateDisplay(List<DataCard> dataCards)
        {
            if (dataCards.Count == 0) return null;
            foreach (var datacard in dataCards)
            {
                decimal weight = datacard.ForceElement?.Weighting?.WeightValue ?? 0;
                if (datacard.DeployedStatus != null)
                {
                    switch (datacard.DeployedStatus.Id)
                    {
                        // 1 Deployed
                        case 1:
                            deployedCount++;
                            deployedWeight += weight;
                            break;
                        // 2 Partially Deployed
                        case 2:
                            partiallyDeployedCount++;
                            partiallyDeployedWeight += weight;
                            break;
                        // 3 High Alert
                        case 3:
                            highAlertCount++;
                            highAlertWeight += weight;
                            break;
                        // 4 In Development
                        case 4:
                            inDevelopmentCount++;
                            inDevelopmentWeight += weight;
                            break;
                        // 5 Force Generating
                        case 5:
                            forceGeneratingCount++;
                            forceGeneratingWeight += weight;
                            break;
                        // 6 Forward To Nato
                        case 6:
                            forwardToNatoCount++;
                            forwardToNatoWeight += weight;
                            break;
                        // 7 Not Employed
                        case 7:
                            notDeployedCount++;
                            notDeployedWeight += weight;
                            break;
                    }
                }
                else
                {
                    notDeployedCount++;
                    notDeployedWeight += weight;
                }
                if (datacard.CommandOverideStatus != null)
                {
                    switch (datacard.CommandOverideStatus.Id)
                    {
                        // 1 Ready
                        case 1:
                            readyCount++;
                            readyWeight += weight;
                            break;
                        // 2 Ready with Lims
                        case 2:
                            readyLimsCount++;
                            readyLimsWeight += weight;
                            switch (datacard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsPersWeight += weight;
                                    break;
                            }
                            switch (datacard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsEquipWeight += weight;
                                    break;
                            }
                            switch (datacard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsTrainWeight += weight;
                                    break;
                            }
                            switch (datacard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsSustWeight += weight;
                                    break;
                            }
                            break;

                        case 3:
                            combatIneffectiveCount++;
                            combatIneffectiveWeight += weight;
                            switch (datacard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffPersWeight += weight;
                                    break;
                            }
                            switch (datacard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffEquipWeight += weight;
                                    break;
                            }
                            switch (datacard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffTrainWeight += weight;
                                    break;
                            }
                            switch (datacard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffSustWeight += weight;
                                    break;
                            }
                            break;
                        // 4 Not ready
                        case 4:
                            notReadyCount++;
                            notReadyWeight += weight;
                            switch (datacard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyPersWeight += weight;
                                    break;
                            }
                            switch (datacard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyEquipWeight += weight;
                                    break;
                            }
                            switch (datacard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyTrainWeight += weight;
                                    break;
                            }
                            switch (datacard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadySustWeight += weight;
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (datacard.SrStatus?.Id)
                    {
                        // 1 Ready
                        case 1:
                            readyCount++;
                            readyWeight += weight;
                            break;
                        // 2 Ready with Lims
                        case 2:
                            readyLimsCount++;
                            readyLimsWeight += weight;
                            switch (datacard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsPersWeight += weight;
                                    break;
                            }
                            switch (datacard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsEquipWeight += weight;
                                    break;
                            }
                            switch (datacard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsTrainWeight += weight;
                                    break;
                            }
                            switch (datacard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsSustWeight += weight;
                                    break;
                            }
                            break;
                        // 3 Combat Ineffective
                        case 3:
                            combatIneffectiveCount++;
                            combatIneffectiveWeight += weight;
                            switch (datacard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffPersWeight += weight;
                                    break;
                            }
                            switch (datacard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffEquipWeight += weight;
                                    break;
                            }
                            switch (datacard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffTrainWeight += weight;
                                    break;
                            }
                            switch (datacard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffSustWeight += weight;
                                    break;
                            }
                            break;
                        // 4 Not ready
                        case 4:
                            notReadyCount++;
                            notReadyWeight += weight;
                            switch (datacard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyPersWeight += weight;
                                    break;
                            }
                            switch (datacard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyEquipWeight += weight;
                                    break;
                            }
                            switch (datacard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyTrainWeight += weight;
                                    break;
                            }
                            switch (datacard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadySustWeight += weight;
                                    break;
                            }
                            break;
                    }
                }
                totalCount++;
                totalWeight += weight;

                // Calculate latency
                double days = (DateTime.Now - datacard.Validitydate).TotalDays;
                totalDays += days;
            }
            return CreateDataCardDisplayModel();
        }

        public DatacardReadinessTableModel? CalculateDisplay(List<DummyDataCard> dummyDataCards)
        {
            if (dummyDataCards.Count == 0) return null;
            foreach (var dummyDataCard in dummyDataCards)
            {
                decimal weight = dummyDataCard.DummyForceElement.Weighting?.WeightValue ?? 0;
                if (dummyDataCard.DeployedStatus != null)
                {
                    switch (dummyDataCard.DeployedStatus.Id)
                    {
                        // 1 Deployed
                        case 1:
                            deployedCount++;
                            deployedWeight += weight;
                            break;
                        // 2 Partially Deployed
                        case 2:
                            partiallyDeployedCount++;
                            partiallyDeployedWeight += weight;
                            break;
                        // 3 High Alert
                        case 3:
                            highAlertCount++;
                            highAlertWeight += weight;
                            break;
                        // 4 In Development
                        case 4:
                            inDevelopmentCount++;
                            inDevelopmentWeight += weight;
                            break;
                        // 5 Force Generating
                        case 5:
                            forceGeneratingCount++;
                            forceGeneratingWeight += weight;
                            break;
                        // 6 Forward To Nato
                        case 6:
                            forwardToNatoCount++;
                            forwardToNatoWeight += weight;
                            break;
                        // 7 Not Employed
                        case 7:
                            notDeployedCount++;
                            notDeployedWeight += weight;
                            break;
                    }
                }
                else
                {
                    notDeployedCount++;
                    notDeployedWeight += weight;
                }
                if (dummyDataCard.CommandOverideStatus != null)
                {
                    switch (dummyDataCard.CommandOverideStatus.Id)
                    {
                        // 1 Ready
                        case 1:
                            readyCount++;
                            readyWeight += weight;
                            break;
                        // 2 Ready with Lims
                        case 2:
                            readyLimsCount++;
                            readyLimsWeight += weight;
                            switch (dummyDataCard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsEquipWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsTrainWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsSustWeight += weight;
                                    break;
                            }
                            break;

                        case 3:
                            combatIneffectiveCount++;
                            combatIneffectiveWeight += weight;
                            switch (dummyDataCard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffEquipWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffEquipWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffSustWeight += weight;
                                    break;
                            }
                            break;
                        // 4 Not ready
                        case 4:
                            notReadyCount++;
                            notReadyWeight += weight;
                            switch (dummyDataCard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyEquipWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyTrainWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadySustWeight += weight;
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    switch (dummyDataCard.SrStatus?.Id)
                    {
                        // 1 Ready
                        case 1:
                            readyCount++;
                            readyWeight += weight;
                            break;
                        // 2 Ready with Lims
                        case 2:
                            readyLimsCount++;
                            readyLimsWeight += weight;
                            switch (dummyDataCard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsEquipWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsTrainWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    readyLimsSustWeight += weight;
                                    break;
                            }
                            break;
                        // 3 Combat Ineffective
                        case 3:
                            combatIneffectiveCount++;
                            combatIneffectiveWeight += weight;
                            switch (dummyDataCard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffTrainWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    combatIneffSustWeight += weight;
                                    break;
                            }
                            break;
                        // 4 Not ready
                        case 4:
                            notReadyCount++;
                            notReadyWeight += weight;
                            switch (dummyDataCard.PersonnelStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyPersWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.EquipmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyEquipWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.TrainingStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadyTrainWeight += weight;
                                    break;
                            }
                            switch (dummyDataCard.SustainmentStatusId)
                            {
                                case 3:
                                case 4:
                                    notReadySustWeight += weight;
                                    break;
                            }
                            break;
                    }
                }
                totalCount++;
                totalWeight += weight;
            }
            return CreateDataCardDisplayModel();
        }

        private DatacardReadinessTableModel CreateDataCardDisplayModel()
        {
            return new DatacardReadinessTableModel()
            {
                AverageDays = (totalDays / totalCount).ToString("##0.##", CultureInfo.InvariantCulture),
                TotalCount = totalCount,
                TotalDays = totalDays,
                TotalWeight = totalWeight,

                // Readiness counts
                ReadyCount = readyCount,
                ReadyLimsCount = readyLimsCount,
                CombatIneffectiveCount = combatIneffectiveCount,
                NotReadyCount = notReadyCount,

                // Deployed counts
                DeployedCount = deployedCount,
                PartiallyDeployedCount = partiallyDeployedCount,
                ForwardToNatoCount = forwardToNatoCount,
                HighAlertCount = highAlertCount,
                ForceGeneratingCount = forceGeneratingCount,
                InDevelopmentCount = inDevelopmentCount,
                NotDeployedCount = notDeployedCount,

                // Readiness percentages
                ReadyPercentage = CalculatePercentage(readyCount, totalCount),
                ReadyLimsPercentatge = CalculatePercentage(readyLimsCount, totalCount),
                CombatIneffectivePercentage = CalculatePercentage(combatIneffectiveCount, totalCount),
                NotReadyPercentage = CalculatePercentage(notReadyCount, totalCount),

                // Deployed percentages
                DeployedPercentage = CalculatePercentage(deployedCount, totalCount),
                PartiallyDeployedPercentage = CalculatePercentage(partiallyDeployedCount, totalCount),
                ForwardToNatoPercentage = CalculatePercentage(forwardToNatoCount, totalCount),
                HighAlertPercentage = CalculatePercentage(highAlertCount, totalCount),
                ForceGeneratingPercentage = CalculatePercentage(forceGeneratingCount, totalCount),
                InDevelopmentPercentage = CalculatePercentage(inDevelopmentCount, totalCount),
                NotDeployedPercentage = CalculatePercentage(notDeployedCount, totalCount),

                // Readiness weighted percentages
                ReadyWeightPercentage = CalculatePercentage(readyWeight, totalWeight),
                ReadyLimsWeightPercentage = CalculatePercentage(readyLimsWeight, totalWeight),
                CombatIneffectiveWeightPercentage = CalculatePercentage(combatIneffectiveWeight, totalWeight),
                NotReadyWeightPercentage = CalculatePercentage(notReadyWeight, totalWeight),

                // Readiness weighted sums
                ReadyWeightSum = readyWeight,
                ReadyLimsWeightSum = readyLimsWeight,
                CombatIneffectiveWeightSum = combatIneffectiveWeight,
                NotReadyWeightSum = notReadyWeight,

                // Deployed weighted percentages
                DeployedWeightPercentage = CalculatePercentage(deployedWeight, totalWeight),
                PartiallyDeployedWeightPercentage = CalculatePercentage(partiallyDeployedWeight, totalWeight),
                ForwardToNatoWeightPercentage = CalculatePercentage(forwardToNatoWeight, totalWeight),
                HighAlertWeightPercentage = CalculatePercentage(highAlertWeight, totalWeight),
                ForceGeneratingWeightPercentage = CalculatePercentage(forceGeneratingWeight, totalWeight),
                InDevelopmentWeightPercentage = CalculatePercentage(inDevelopmentWeight, totalWeight),
                NotDeployedWeightPercentage = CalculatePercentage(notDeployedWeight, totalWeight),

                // Deployed weighted sums
                DeployedWeightSum = deployedWeight,
                PartiallyDeployedWeightSum = partiallyDeployedWeight,
                ForwardToNatoWeightSum = forwardToNatoWeight,
                HighAlertWeightSum = highAlertWeight,
                ForceGeneratingWeightSum = forceGeneratingWeight,
                InDevelopmentWeightSum = inDevelopmentWeight,
                NotDeployedWeightSum = notDeployedWeight,

                // Ready with limits PETS percentages
                YellowStatusPersonnelPercentage = CalculatePercentage(readyLimsPersWeight, readyLimsWeight),
                YellowStatusEquipmentPercentage = CalculatePercentage(readyLimsEquipWeight, readyLimsWeight),
                YellowStatusTrainingPercentage = CalculatePercentage(readyLimsTrainWeight, readyLimsWeight),
                YellowStatusSustainmentPercentage = CalculatePercentage(readyLimsSustWeight, readyLimsWeight),

                // Combat Ineffective PETS percentages
                OrangeStatusPersonnelPercentage = CalculatePercentage(combatIneffPersWeight, combatIneffectiveWeight),
                OrangeStatusEquipmentPercentage = CalculatePercentage(combatIneffEquipWeight, combatIneffectiveWeight),
                OrangeStatusTrainingPercentage = CalculatePercentage(combatIneffTrainWeight, combatIneffectiveWeight),
                OrangeStatusSustainmentPercentage = CalculatePercentage(combatIneffSustWeight, combatIneffectiveWeight),

                // Not Ready PETS percentages
                RedStatusPersonnelPercentage = CalculatePercentage(notReadyPersWeight, notReadyWeight),
                RedStatusEquipmentPercentage = CalculatePercentage(notReadyEquipWeight, notReadyWeight),
                RedStatusTrainingPercentage = CalculatePercentage(notReadyTrainWeight, notReadyWeight),
                RedStatusSustainmentPercentage = CalculatePercentage(notReadySustWeight, notReadyWeight)
            };
        }

        private static string CalculatePercentage(int left, int right)
        {
            if (right == 0) return "0";
            else return (left / (double)right * 100).ToString("##0.##", CultureInfo.InvariantCulture);
        }

        private static string CalculatePercentage(decimal left, decimal right)
        {
            if (right == 0) return "0";
            else return (left / right * 100).ToString("##0.##", CultureInfo.InvariantCulture);
        }
    }
}