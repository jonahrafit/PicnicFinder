using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBO.Models.formBody
{
    public class ReservationStatusUpdateRequest
    {
        public long ReservationId { get; set; }
        public long EmployeeId { get; set; }
        public string Status { get; set; } = string.Empty; // Acceptation en tant que chaîne
    }
}
