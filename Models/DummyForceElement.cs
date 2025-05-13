using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class DummyForceElement
    {
        public DummyForceElement()
        {
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public bool IsTiedToRealFelm { get; set; }
        public bool IsActiveInForcePackage { get; set; }
        public int? ForceElementId { get; set; }
        public int ForcePackageId { get; set; }
        public string ElementId { get; set; } = null!;
        public string? ElementName { get; set; }
        public string? ElementNameFre { get; set; }
        public int OrganizationId { get; set; }
        public int? WeightingId { get; set; }
        public int Ordered { get; set; }
        public bool Archived { get; set; }

        public virtual ForceElement? ForceElement { get; set; }
        public virtual ForcePackage ForcePackage { get; set; } = null!;
        public virtual Organization Organization { get; set; } = null!;
        public virtual Weighting? Weighting { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
