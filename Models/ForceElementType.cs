namespace SMARTV3.Models
{
    public partial class ForceElementType
    {
        public int Id { get; set; }
        public string ForceElementTypeName { get; set; } = null!;
        public string? ForceElementTypeDescription { get; set; }
        public bool Archived { get; set; }
        public int Ordered { get; set; }
    }
}