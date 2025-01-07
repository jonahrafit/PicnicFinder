using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrontOffice.Models;

namespace FrontOffice.Models
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public object Employee { get; set; } // Ajustez le type selon vos besoins
        public int SpaceId { get; set; }
        public SpaceViewModel Space { get; set; } // Ajustez le type selon vos besoins
        public DateTime ReservationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }

        public override string ToString()
        {
            return $"SpaceId: {SpaceId}, "
                + $"StartDate: {StartDate}, "
                + $"EndDate: {EndDate}, "
                + $"Status: {Status}, ";
        }
    }
}
