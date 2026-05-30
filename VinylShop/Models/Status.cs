using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("statuses")]
    public class Status
    {
        [Key]
        public int id_status { get; set; }

        [Required]
        public string name { get; set; } = null!;

        [Required]
        public decimal min_spend { get; set; }

        [Required]
        public int discount_percentage { get; set; }
    }
}