using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class DesignationArchiveComment
    {
        public int Id { get; set; }
        public int DesignationId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Designation Designation { get; set; } = null!;
    }
}
