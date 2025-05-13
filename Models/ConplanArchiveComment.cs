using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ConplanArchiveComment
    {
        public int Id { get; set; }
        public int ConplanId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Conplan Conplan { get; set; } = null!;
    }
}
