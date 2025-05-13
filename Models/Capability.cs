using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Capability
    {
        public Capability()
        {
            CapabilityArchiveComments = new HashSet<CapabilityArchiveComment>();
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        /// <summary>
        /// Primary Key for Capabilities
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// English Capability Name
        /// </summary>
        public string CapabilityName { get; set; } = null!;
        /// <summary>
        /// French Capability Name
        /// </summary>
        public string? CapabilityNameFre { get; set; }
        /// <summary>
        /// If the Capability is archived and should no longer show in the UI
        /// </summary>
        public bool Archived { get; set; }
        /// <summary>
        /// Order Capibilities in Drop Downs
        /// </summary>
        public int Ordered { get; set; }

        public bool NatoCapability { get; set; }

        public string? CapabilityDesc { get; set; }

        public virtual ICollection<CapabilityArchiveComment> CapabilityArchiveComments { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
        public virtual ICollection<OutputTask> OutputTasks { get; set; }
    }
}
