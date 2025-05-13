using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class YesNoBlank
    {
        public YesNoBlank()
        {
            DataCards = new HashSet<DataCard>();
        }

        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public string ValueFre { get; set; } = null!;
        public int Order { get; set; }

        public virtual ICollection<DataCard> DataCards { get; set; }
    }
}
