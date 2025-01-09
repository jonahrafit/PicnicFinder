using AdminBO.Models;

public class ViewReservation
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public long OwnerId { get; set; }
    public string OwnerName { get; set; }
    public long SpaceId { get; set; }
    public string SpaceName { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; }
}
