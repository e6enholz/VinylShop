using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("drive_types")]
    public class TypeDrive
    {
        [Key]
        public int id_drive_type { get; set; }

        [Required]
        public string name { get; set; } = null!;

        public string? description { get; set; }
    }
}