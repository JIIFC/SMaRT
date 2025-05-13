using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class CapabilityArchiveComment
    {
        public int Id { get; set; }
        public int CapabilityId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Capability Capability { get; set; } = null!;
    }
}
