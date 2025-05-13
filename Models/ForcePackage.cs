using System;
using System.Collections.Generic;

namespace SMARTV3.Models
{
    public partial class ForcePackage
    {
        public ForcePackage()
        {
            DummyForceElements = new HashSet<DummyForceElement>();
            ForcePackageKpis = new HashSet<ForcePackageKpi>();
            Conplans = new HashSet<Conplan>();
            Operations = new HashSet<Operation>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public int PackageOwnerId { get; set; }
        public string ForcePackageName { get; set; } = null!;
        public string? ForcePackageDescription { get; set; }
        public int ForcePackagePurpose { get; set; }
        public DateTime DateLastFetchedLiveData { get; set; }
        public int? LastEditUser { get; set; }
        public DateTime? LastEditDate { get; set; }

        public virtual ForcePackagePurpose ForcePackagePurposeNavigation { get; set; } = null!;
        public virtual User? LastEditUserNavigation { get; set; }
        public virtual User PackageOwner { get; set; } = null!;
        public virtual ICollection<DummyForceElement> DummyForceElements { get; set; }
        public virtual ICollection<ForcePackageKpi> ForcePackageKpis { get; set; }

        public virtual ICollection<Conplan> Conplans { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
