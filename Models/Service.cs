using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Service
    {
        public Service()
        {
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
            ServiceArchiveComments = new HashSet<ServiceArchiveComment>();
        }

        public int Id { get; set; }
        public string ServiceName { get; set; } = null!;
        public string? ServiceNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
        public virtual ICollection<ServiceArchiveComment> ServiceArchiveComments { get; set; }
    }
}
