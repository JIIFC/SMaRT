using SMARTV3.Models;

namespace SMARTV3.Helpers
{
    public class ChangeLogHelper
    {
        private SMARTV3DbContext _context;

        public ChangeLogHelper(SMARTV3DbContext context)
        {
            _context = context;
        }

        public void AddChangeLogItem(DataCard dc)
        {
            ChangeLog changeLog = new()
            {
                ForceElementId = dc.ForceElementId,
                SrStatusId = dc.SrStatusId,
                CommandOverideStatusId = dc.CommandOverideStatusId,
                CommandOverrideComments = dc.CommandOverrideComments,
                NatoGeneralComments = dc.NatoGeneralComments,
                NatoMajorEquipmentComments = dc.NatoMajorEquipmentComments,
                NatoCavets = dc.NatoCavets,
                NatoStratLiftCapacityComments = dc.NatoStratLiftCapacityComments,
                NatoNationalDeployComments = dc.NatoNationalDeployComments,
                NatoNationalAssesmentComments = dc.NatoNationalAssesmentComments,
                PersonnelStatusId = dc.PersonnelStatusId,
                PersonnelComments = dc.PersonnelComments,
                TrainingStatusId = dc.TrainingStatusId,
                TrainingComments = dc.TrainingComments,
                EquipmentStatusId = dc.EquipmentStatusId,
                EquipmentComments = dc.EquipmentComments,
                SustainmentStatusId = dc.SustainmentStatusId,
                SustainmentComments = dc.SustainmentComments,
                ChangedDate = DateTime.Now,
                LastEditUser = null
            };

            _context.ChangeLogs.Add(changeLog);
            _context.SaveChanges();
        }
    }
}