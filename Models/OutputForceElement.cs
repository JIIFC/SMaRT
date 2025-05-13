using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class OutputForceElement
    {
        public int Id { get; set; }
        public int OutputTaskId { get; set; }
        public int FelmId { get; set; }
        public DateTime AssignmentStart { get; set; }
        public DateTime AssignmentEnd { get; set; }

        public virtual OutputTask OutputTask { get; set; } = null!;
    }
}
