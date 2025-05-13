using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class DatacardKpi
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DatacardId { get; set; }
        public int? OverallStatusBelowId { get; set; }
        public int? OverallStatusAboveId { get; set; }
        public bool AlertAnyChanges { get; set; }
        public bool AlertOnSubmit { get; set; }
        public bool AlertWhenIncomplete { get; set; }

        public virtual DataCard Datacard { get; set; } = null!;
        public virtual PetsoverallStatus? OverallStatusAbove { get; set; }
        public virtual PetsoverallStatus? OverallStatusBelow { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
