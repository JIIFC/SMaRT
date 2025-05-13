using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class OperationArchiveComment
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Operation Operation { get; set; } = null!;
    }
}
