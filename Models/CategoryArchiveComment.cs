using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class CategoryArchiveComment
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
    }
}
