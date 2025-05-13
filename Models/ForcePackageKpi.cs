using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ForcePackageKpi
    {
        public int Id { get; set; }
        public int ForcePackageId { get; set; }
        public int UserId { get; set; }

        public virtual ForcePackage ForcePackage { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
