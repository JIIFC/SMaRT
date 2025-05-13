using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class Organization
    {
        public Organization()
        {
            DummyForceElements = new HashSet<DummyForceElement>();
            ForceElements = new HashSet<ForceElement>();
            OrganizationArchiveComments = new HashSet<OrganizationArchiveComment>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string OrganizationName { get; set; } = null!;
        public string? OrganizationNameFre { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }

        public virtual ICollection<DummyForceElement> DummyForceElements { get; set; }
        public virtual ICollection<ForceElement> ForceElements { get; set; }
        public virtual ICollection<OrganizationArchiveComment> OrganizationArchiveComments { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
