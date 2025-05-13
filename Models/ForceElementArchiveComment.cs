using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ForceElementArchiveComment
    {
        public int Id { get; set; }
        public int ForceElementId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual ForceElement ForceElement { get; set; } = null!;
    }
}
