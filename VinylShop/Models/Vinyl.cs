using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("vinyls")]
    public class Vinyl
    {
        [Key]
        public int id_vinyl { get; set; }

        [Required]
        public string album { get; set; } = null!;

        [Required]
        public string artist { get; set; } = null!;

        [Required]
        public decimal price { get; set; }

        [ForeignKey("Genre")]
        public int genreId { get; set; }
        public Genre? Genre { get; set; }

        [ForeignKey("Condition")]
        public int conditionId { get; set; }
        public Condition? Condition { get; set; }
    }
}