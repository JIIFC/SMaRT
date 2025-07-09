using System.ComponentModel.DataAnnotations;

namespace SMARTV3.Models
{
    [MetadataType(typeof(DataCardAnnotation))]
    public partial class DataCard
    {
    }

    internal sealed class DataCardAnnotation
    {
        [Required(ErrorMessage = "Required!")]
        public string Subunit { get; set; }
    }
}