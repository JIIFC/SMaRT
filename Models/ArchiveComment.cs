using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ArchiveComment
    {
        public ArchiveComment()
        {
            AfsTrainingPercentageArchiveComments = new HashSet<AfsTrainingPercentageArchiveComment>();
            CapabilityArchiveComments = new HashSet<CapabilityArchiveComment>();
            CategoryArchiveComments = new HashSet<CategoryArchiveComment>();
            ConplanArchiveComments = new HashSet<ConplanArchiveComment>();
            DesignationArchiveComments = new HashSet<DesignationArchiveComment>();
            EchelonArchiveComments = new HashSet<EchelonArchiveComment>();
            ForceElementArchiveComments = new HashSet<ForceElementArchiveComment>();
            OperationArchiveComments = new HashSet<OperationArchiveComment>();
            OrganizationArchiveComments = new HashSet<OrganizationArchiveComment>();
            ServiceArchiveComments = new HashSet<ServiceArchiveComment>();
        }

        public int Id { get; set; }
        public string Comments { get; set; } = null!;
        public bool Archived { get; set; }
        public DateTime ChangeDate { get; set; }
        public int ChangeUser { get; set; }

        public virtual User ChangeUserNavigation { get; set; } = null!;
        public virtual ICollection<AfsTrainingPercentageArchiveComment> AfsTrainingPercentageArchiveComments { get; set; }
        public virtual ICollection<CapabilityArchiveComment> CapabilityArchiveComments { get; set; }
        public virtual ICollection<CategoryArchiveComment> CategoryArchiveComments { get; set; }
        public virtual ICollection<ConplanArchiveComment> ConplanArchiveComments { get; set; }
        public virtual ICollection<DesignationArchiveComment> DesignationArchiveComments { get; set; }
        public virtual ICollection<EchelonArchiveComment> EchelonArchiveComments { get; set; }
        public virtual ICollection<ForceElementArchiveComment> ForceElementArchiveComments { get; set; }
        public virtual ICollection<OperationArchiveComment> OperationArchiveComments { get; set; }
        public virtual ICollection<OrganizationArchiveComment> OrganizationArchiveComments { get; set; }
        public virtual ICollection<ServiceArchiveComment> ServiceArchiveComments { get; set; }
    }
}
