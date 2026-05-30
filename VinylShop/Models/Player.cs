using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("players")]
    public class Player
    {
        [Key]
        public int id_player { get; set; }

        [Required]
        public string model { get; set; } = null!;

        [Required]
        public decimal price { get; set; }

        [ForeignKey("Brand")]
        public int brandId { get; set; }
        public Brand? Brand { get; set; }

        [ForeignKey("DriveType")]
        public int driveTypeId { get; set; }
        public TypeDrive? DriveType { get; set; }
    }
}