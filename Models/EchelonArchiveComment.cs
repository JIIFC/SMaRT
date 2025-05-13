using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class EchelonArchiveComment
    {
        public int Id { get; set; }
        public int EchelonId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
        public virtual Echelon Echelon { get; set; } = null!;
    }
}
