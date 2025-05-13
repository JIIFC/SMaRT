using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ForcePackagePurpose
    {
        public ForcePackagePurpose()
        {
            ForcePackages = new HashSet<ForcePackage>();
        }

        public int Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameFr { get; set; }
        public int Order { get; set; }
        public bool Archived { get; set; }

        public virtual ICollection<ForcePackage> ForcePackages { get; set; }
    }
}
