using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicnicFinder.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [ForeignKey("Space")]
        public int SpaceID { get; set; }

        public Space Space { get; set; } = null!;

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        public User Employee { get; set; } = null!;

        [Range(1, 5)] // Exemple de notation de 1 à 5 étoiles
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

    }
}
