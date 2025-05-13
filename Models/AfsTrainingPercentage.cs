using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class AfsTrainingPercentage
    {
        public AfsTrainingPercentage()
        {
            AfsTrainingPercentageArchiveComments = new HashSet<AfsTrainingPercentageArchiveComment>();
            DataCards = new HashSet<DataCard>();
        }

        public int Id { get; set; }
        public string StatusDisplayColour { get; set; } = null!;
        public string StatusDisplayValue { get; set; } = null!;
        public int Ordering { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<AfsTrainingPercentageArchiveComment> AfsTrainingPercentageArchiveComments { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
    }
}
