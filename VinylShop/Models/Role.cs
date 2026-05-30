using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        public int id_role { get; set; }

        [Required]
        public string name { get; set; } = null!;
    }
}