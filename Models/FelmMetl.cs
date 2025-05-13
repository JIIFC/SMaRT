using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class FelmMetl
    {
        public int Id { get; set; }
        public int FelmId { get; set; }
        public int MetId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public virtual Metl Met { get; set; } = null!;
    }
}
