namespace SMARTV3.Models
{
    public class FPcompareViewModel
    {
        public int Id { get; set; }
        public string SerializedForcePackageIds { get; set; } = null!;
        public string MainForcePackage { get; set; } = null!;
        public List<string>? OtherForcePackageNames { get; set; }
    }
}