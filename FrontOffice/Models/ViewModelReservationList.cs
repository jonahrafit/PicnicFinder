using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontOffice.Models
{
    public class ViewModelReservationList
    {
        public List<ReservationViewModel> Reservations { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
