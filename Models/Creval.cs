using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Creval
    {
        public Creval()
        {
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string? CrevalName { get; set; }
        public string? CrevalNameFre { get; set; }

        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
