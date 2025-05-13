using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class AfsTrainingPercentageArchiveComment
    {
        public int Id { get; set; }
        public int AfsTrainingPercentageId { get; set; }
        public int ArchiveCommentId { get; set; }

        public virtual AfsTrainingPercentage AfsTrainingPercentage { get; set; } = null!;
        public virtual ArchiveComment ArchiveComment { get; set; } = null!;
    }
}
