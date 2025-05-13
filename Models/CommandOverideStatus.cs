using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class CommandOverideStatus
    {
        public CommandOverideStatus()
        {
            ChangeLogs = new HashSet<ChangeLog>();
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string? StausDisplayColour { get; set; }
        public string? StatusDisplayvalue { get; set; }
        public string? StatusDisplayvalueFre { get; set; }
        public int? StatusValue { get; set; }
        public int? Ordering { get; set; }
        public bool? Archived { get; set; }

        public virtual ICollection<ChangeLog> ChangeLogs { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
