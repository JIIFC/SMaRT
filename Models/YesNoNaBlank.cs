using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class YesNoNaBlank
    {
        public YesNoNaBlank()
        {
            DataCardNato12SdosNavigations = new HashSet<DataCard>();
            DataCardNato18SdosNavigations = new HashSet<DataCard>();
            DataCardNatoCertCompletedNavigations = new HashSet<DataCard>();
            DataCardNatoCertProgramCoordNavigations = new HashSet<DataCard>();
            DataCardNatoEvalCompletedNavigations = new HashSet<DataCard>();
            DataCardNatoNatSupplyPlanNavigations = new HashSet<DataCard>();
            DataCardNatoNatSupportElemNavigations = new HashSet<DataCard>();
        }

        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public string ValueFre { get; set; } = null!;
        public int Order { get; set; }

        public virtual ICollection<DataCard> DataCardNato12SdosNavigations { get; set; }
        public virtual ICollection<DataCard> DataCardNato18SdosNavigations { get; set; }
        public virtual ICollection<DataCard> DataCardNatoCertCompletedNavigations { get; set; }
        public virtual ICollection<DataCard> DataCardNatoCertProgramCoordNavigations { get; set; }
        public virtual ICollection<DataCard> DataCardNatoEvalCompletedNavigations { get; set; }
        public virtual ICollection<DataCard> DataCardNatoNatSupplyPlanNavigations { get; set; }
        public virtual ICollection<DataCard> DataCardNatoNatSupportElemNavigations { get; set; }
    }
}
