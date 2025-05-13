namespace SMARTV3.Models
{
    public class FPSharedViewModel
    {
        public FPSharedViewModel(ForcePackage forcePackage)
        {
            Id = forcePackage.Id;
            PackageOwnerId = forcePackage.PackageOwnerId;
            ForcePackageName = forcePackage.ForcePackageName;
            ForcePackageDescription = forcePackage.ForcePackageDescription;
            ForcePackagePurpose = forcePackage.ForcePackagePurpose;
            DateLastFetchedLiveData = forcePackage.DateLastFetchedLiveData;
            DateLastFetchedLiveData = forcePackage.DateLastFetchedLiveData;
            LastEditUser = forcePackage.LastEditUser;
            LastEditDate = forcePackage.LastEditDate;
            ForcePackagePurposeNavigation = forcePackage.ForcePackagePurposeNavigation;
            LastEditUserNavigation = forcePackage.LastEditUserNavigation;
            PackageOwner = forcePackage.PackageOwner;
            SharedDummyForceElements = new HashSet<KeyValuePair<string, DummyForceElement>>();
            OtherDummyForceElements = forcePackage.DummyForceElements;
            ForcePackageKpis = forcePackage.ForcePackageKpis;
            Conplans = forcePackage.Conplans;
            Operations = forcePackage.Operations;
            Users = forcePackage.Users;
        }

        public int Id { get; set; }
        public int PackageOwnerId { get; set; }
        public string ForcePackageName { get; set; } = null!;
        public string? ForcePackageDescription { get; set; }
        public int ForcePackagePurpose { get; set; }
        public DateTime DateLastFetchedLiveData { get; set; }
        public int? LastEditUser { get; set; }
        public DateTime? LastEditDate { get; set; }
        public DatacardReadinessTableModel? DRTModel { get; set; }

        public virtual ForcePackagePurpose ForcePackagePurposeNavigation { get; set; } = null!;
        public virtual User? LastEditUserNavigation { get; set; }
        public virtual User PackageOwner { get; set; } = null!;
        public virtual ICollection<KeyValuePair<string, DummyForceElement>> SharedDummyForceElements { get; set; }
        public virtual ICollection<DummyForceElement> OtherDummyForceElements { get; set; }
        public virtual ICollection<ForcePackageKpi> ForcePackageKpis { get; set; }

        public virtual ICollection<Conplan> Conplans { get; set; }
        public virtual ICollection<Operation> Operations { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}