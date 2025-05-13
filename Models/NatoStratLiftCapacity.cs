using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class NatoStratLiftCapacity
    {
        public NatoStratLiftCapacity()
        {
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string StratLiftCapacityName { get; set; } = null!;
        public string? StratLiftCapacityNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
