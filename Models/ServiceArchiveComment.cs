using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ServiceArchiveComment
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
