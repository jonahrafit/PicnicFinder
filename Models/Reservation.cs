using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicnicFinder.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [ForeignKey("Space")]
        public int SpaceID { get; set; }

        public Space Space { get; set; } = null!;

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }

        public User Employee { get; set; } = null!; 

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = string.Empty; // "Confirmée", "Rejetée", etc.
    }
}
