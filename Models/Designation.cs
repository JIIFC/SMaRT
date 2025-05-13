using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Designation
    {
        public Designation()
        {
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DesignationArchiveComments = new HashSet<DesignationArchiveComment>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string DesignationName { get; set; } = null!;
        public string? DesignationNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DesignationArchiveComment> DesignationArchiveComments { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
