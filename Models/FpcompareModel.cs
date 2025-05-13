using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class FpcompareModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SerializedForcePackageIds { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
