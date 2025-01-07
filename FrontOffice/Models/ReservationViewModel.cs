using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontOffice.Models
{
    public class ReservationViewModel
    {
        public string SpaceId { get; set; }
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
