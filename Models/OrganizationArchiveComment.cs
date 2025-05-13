using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class OrganizationArchiveComment
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Organization Organization { get; set; } = null!;
    }
}
