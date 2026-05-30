using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("brands")]
    public class Brand
    {
        [Key]
        public int id_brand { get; set; }

        [Required]
        public string name { get; set; } = null!;

        public string? country { get; set; }

        public string? description { get; set; }
    }
}