using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Weighting
    {
        public Weighting()
        {
            DummyForceElements = new HashSet<DummyForceElement>();
            ForceElements = new HashSet<ForceElement>();
        }

        public int Id { get; set; }
        public decimal? WeightValue { get; set; }

        public virtual ICollection<DummyForceElement> DummyForceElements { get; set; }
        public virtual ICollection<ForceElement> ForceElements { get; set; }
    }
}
