using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Conplan
    {
        public Conplan()
        {
            ConplanArchiveComments = new HashSet<ConplanArchiveComment>();
            DataCardConplanHistories = new HashSet<DataCardConplanHistory>();
            DataCards = new HashSet<DataCard>();
            ForcePackages = new HashSet<ForcePackage>();
        }

        /// <summary>
        /// Primary Key for Conplan Table
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// English Conplan Name
        /// </summary>
        public string ConplanName { get; set; } = null!;
        /// <summary>
        /// French Conplan Name
        /// </summary>
        public string? ConplanNameFre { get; set; }
        /// <summary>
        /// If the Conplan is archived and should no longer show in the UI
        /// </summary>
        public bool Archived { get; set; }
        /// <summary>
        /// Order Conplans in Drop Downs
        /// </summary>
        public int Ordered { get; set; }

        public virtual ICollection<ConplanArchiveComment> ConplanArchiveComments { get; set; }
        public virtual ICollection<DataCardConplanHistory> DataCardConplanHistories { get; set; }

        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<ForcePackage> ForcePackages { get; set; }
    }
}
