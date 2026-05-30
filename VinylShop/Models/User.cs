using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        public int id_user { get; set; }

        [Required]
        public string login { get; set; } = null!;

        [Required]
        public string email { get; set; } = null!;

        [Required]
        public string password { get; set; } = null!;

        [ForeignKey("Status")]
        public int statusId { get; set; }
        public Status? Status { get; set; }

        [ForeignKey("Role")]
        public int roleId { get; set; }
        public Role? Role { get; set; }
    }
}