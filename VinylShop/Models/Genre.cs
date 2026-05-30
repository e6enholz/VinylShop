using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("genres")]
    public class Genre
    {
        [Key]
        public int id_genre { get; set; }

        [Required]
        public string name { get; set; } = null!;

        public string? description { get; set; }
    }
}