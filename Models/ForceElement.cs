using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ForceElement
    {
        public ForceElement()
        {
            ChangeLogs = new HashSet<ChangeLog>();
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyForceElements = new HashSet<DummyForceElement>();
            ForceElementArchiveComments = new HashSet<ForceElementArchiveComment>();
        }

        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }
        public string ElementId { get; set; } = null!;
        public string ElementName { get; set; } = null!;
        public string? ElementNameFre { get; set; }
        public string? Shortname { get; set; }
        public string? ShortnameFR { get; set; }
        public int OrganizationId { get; set; }
        public int? WeightingId { get; set; }
        public int Ordered { get; set; }
        public bool Archived { get; set; }
        public bool Aspirational { get; set; }

        public virtual Organization Organization { get; set; } = null!;
        public virtual Weighting? Weighting { get; set; }
        public virtual ICollection<ChangeLog> ChangeLogs { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyForceElement> DummyForceElements { get; set; }
        public virtual ICollection<ForceElementArchiveComment> ForceElementArchiveComments { get; set; }
    }
}
