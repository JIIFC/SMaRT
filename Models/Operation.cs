using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Operation
    {
        public Operation()
        {
            DataCardOperationsHistories = new HashSet<DataCardOperationsHistory>();
            OperationArchiveComments = new HashSet<OperationArchiveComment>();
            DataCards = new HashSet<DataCard>();
            ForcePackages = new HashSet<ForcePackage>();
        }

        public int Id { get; set; }
        public string OperationName { get; set; } = null!;
        public string? OperationNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DataCardOperationsHistory> DataCardOperationsHistories { get; set; }
        public virtual ICollection<OperationArchiveComment> OperationArchiveComments { get; set; }

        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<ForcePackage> ForcePackages { get; set; }
    }
}
