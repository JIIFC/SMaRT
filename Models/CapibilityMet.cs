using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class CapibilityMet
    {
        public int Id { get; set; }
        public int CapabilityId { get; set; }
        public int MetId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public virtual Metl Met { get; set; } = null!;
    }
}
