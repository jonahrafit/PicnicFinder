using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicnicFinder.Models
{
    public class Space
    {
        [Key]
        public int SpaceID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string? Location { get; set; }

        [Range(1, 1000)] // Exemples de capacité raisonnable
        public int Capacity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [ForeignKey("Owner")]
        public int OwnerID { get; set; }

        public User Owner { get; set; } = null!;
    }
}
