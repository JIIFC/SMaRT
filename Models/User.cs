namespace SMARTV3.Models
{
    public partial class User
    {
        public User()
        {
            ArchiveComments = new HashSet<ArchiveComment>();
            ChangeLogs = new HashSet<ChangeLog>();
            DataCardHistories = new HashSet<DataCardHistory>();
            DataCards = new HashSet<DataCard>();
            DatacardKpis = new HashSet<DatacardKpi>();
            DummyDataCards = new HashSet<DummyDataCard>();
            ForcePackageKpis = new HashSet<ForcePackageKpi>();
            ForcePackageLastEditUserNavigations = new HashSet<ForcePackage>();
            ForcePackagePackageOwners = new HashSet<ForcePackage>();
            FpcompareModels = new HashSet<FpcompareModel>();
            ForcePackages = new HashSet<ForcePackage>();
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Title { get; set; }
        public string Email { get; set; } = null!;
        public bool Enabled { get; set; }
        public int OrganizationId { get; set; }
        public bool POC {  get; set; }
        public virtual Organization Organization { get; set; } = null!;
        public virtual ICollection<ArchiveComment> ArchiveComments { get; set; }
        public virtual ICollection<ChangeLog> ChangeLogs { get; set; }
        public virtual ICollection<DataCardHistory> DataCardHistories { get; set; }
        public virtual ICollection<DataCard> DataCards { get; set; }
        public virtual ICollection<DatacardKpi> DatacardKpis { get; set; }
        public virtual ICollection<DummyDataCard> DummyDataCards { get; set; }
        public virtual ICollection<ForcePackageKpi> ForcePackageKpis { get; set; }
        public virtual ICollection<ForcePackage> ForcePackageLastEditUserNavigations { get; set; }
        public virtual ICollection<ForcePackage> ForcePackagePackageOwners { get; set; }
        public virtual ICollection<FpcompareModel> FpcompareModels { get; set; }

        public virtual ICollection<ForcePackage> ForcePackages { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
