using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("deliveries")]
    public class Delivery
    {
        public Delivery()
        {
            orderDate = DateTime.UtcNow; // Прямо как в дипломе — ставим текущую дату по дефолту
        }

        [Key]
        public int id_delivery { get; set; }

        [Required]
        public string delivery_address { get; set; } = null!;

        [Required]
        public string status_text { get; set; } = null!;

        [Required]
        public DateTime orderDate { get; set; }

        [ForeignKey("User")]
        public int userId { get; set; }
        public User? User { get; set; }
    }
}