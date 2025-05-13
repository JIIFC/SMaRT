using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class NoticeToMove
    {
        public NoticeToMove()
        {
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string NoticeToMoveName { get; set; } = null!;
        public string? NoticeToMoveNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
        public virtual ICollection<OutputTask> OutputTasks { get; set; }
    }
}
