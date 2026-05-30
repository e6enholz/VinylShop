using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("conditions")]
    public class Condition
    {
        [Key]
        public int id_condition { get; set; }

        [Required]
        public string name { get; set; } = null!;

        public string? description { get; set; }
    }
}