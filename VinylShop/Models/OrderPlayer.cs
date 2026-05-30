using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("order_players")]
    public class OrderPlayer
    {
        [Key]
        public int id_order_player { get; set; }

        [ForeignKey("Delivery")]
        public int deliveryId { get; set; }
        public Delivery? Delivery { get; set; }

        [ForeignKey("Player")]
        public int playerId { get; set; }
        public Player? Player { get; set; }
    }
}