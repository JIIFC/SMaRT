using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class NatoNationalDeploy
    {
        public NatoNationalDeploy()
        {
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        public int Id { get; set; }
        public string NationalDeployName { get; set; } = null!;
        public string? NationalDeployNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
