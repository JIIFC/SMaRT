using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryArchiveComments = new HashSet<CategoryArchiveComment>();
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DummyDataCards = new HashSet<DummyDataCard>();
        }

        /// <summary>
        /// Primary Key for Categories
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// English Category Name
        /// </summary>
        public string CategoryName { get; set; } = null!;
        /// <summary>
        /// French Category Name
        /// </summary>
        public string? CategoryNameFre { get; set; }
        /// <summary>
        /// If the Categories is archived and should no longer show in the UI
        /// </summary>
        public bool Archived { get; set; }
        /// <summary>
        /// Order in Drop Downs
        /// </summary>
        public int Ordered { get; set; }

        public virtual ICollection<CategoryArchiveComment> CategoryArchiveComments { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
    }
}
