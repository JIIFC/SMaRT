using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Metl
    {
        public Metl()
        {
            FelmMetls = new HashSet<FelmMetl>();
        }

        public int MetId { get; set; }
        public string MetCode { get; set; } = null!;
        public string MetName { get; set; } = null!;
        public string? MetDesc { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<FelmMetl> FelmMetls { get; set; }
    }
}
