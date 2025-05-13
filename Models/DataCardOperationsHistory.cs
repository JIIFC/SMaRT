using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class DataCardOperationsHistory
    {
        public int DataCardId { get; set; }
        public int OperationId { get; set; }
        public int HistoryYear { get; set; }
        public int HistoryMonth { get; set; }

        public virtual Operation Operation { get; set; } = null!;
    }
}
