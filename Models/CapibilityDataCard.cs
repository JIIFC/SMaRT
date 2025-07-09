using System;
using System.Collections.Generic;

namespace SMARTV3.Models;

public partial class CapibilityDataCard
{
    public int CapabilityId { get; set; }
    public int DataCardId { get; set; }
    public bool PrimaryCode { get; set; }
    public DateTime? validFrom { get; set; }
    public DateTime? validTo { get; set; }

    public virtual DataCardV2 DataCard { get; set; } = null!;
}
