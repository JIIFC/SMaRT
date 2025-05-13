using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Echelon
    {
        public Echelon()
        {
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
            EchelonArchiveComments = new HashSet<EchelonArchiveComment>();
        }

        public int Id { get; set; }
        public string EchelonName { get; set; } = null!;
        public string? EchelonNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
        public virtual ICollection<EchelonArchiveComment> EchelonArchiveComments { get; set; }
    }
}
