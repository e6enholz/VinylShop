using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("order_vinyls")]
    public class OrderVinyl
    {
        [Key]
        public int id_order_vinyl { get; set; }

        [ForeignKey("Delivery")]
        public int deliveryId { get; set; }
        public Delivery? Delivery { get; set; }

        [ForeignKey("Vinyl")]
        public int vinylId { get; set; }
        public Vinyl? Vinyl { get; set; }
    }
}