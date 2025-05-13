using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class OutputTask
    {
        public OutputTask()
        {
            OutputForceElements = new HashSet<OutputForceElement>();
        }

        public int Id { get; set; }
        public string OutputName { get; set; } = null!;
        public string? OutputDesc { get; set; }
        public int? CapabilityId { get; set; }
        public double? Priority { get; set; }
        public DateTime? OutputStart { get; set; }
        public DateTime? OutputEnd { get; set; }
        public int? NtmId { get; set; }

        public virtual Capability? Capability { get; set; }
        public virtual NoticeToMove? Ntm { get; set; }
        public virtual ICollection<OutputForceElement> OutputForceElements { get; set; }
    }
}
