using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class DataCardConplanHistory
    {
        public int DataCardId { get; set; }
        public int ConplanId { get; set; }
        public int HistoryYear { get; set; }
        public int HistoryMonth { get; set; }

        public virtual Conplan Conplan { get; set; } = null!;
    }
}
