namespace SMARTV3.Models
{
    public partial class Caveat
    {
        public int Id { get; set; }
        public string Caveats { get; set; } = null!;
        public bool Archived { get; set; }
        public int Ordered { get; set; }
    }
}