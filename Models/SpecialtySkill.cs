using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class SpecialtySkill
    {
        public SpecialtySkill()
        {
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string SpecialtySkillName { get; set; } = null!;
        public string SpecialtySkillNameFre { get; set; } = null!;

        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
