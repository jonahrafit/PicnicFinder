// Modèle pour les statistiques des réservations
public class ReservationStats
{
    public string MonthYear { get; set; }
    public int TotalReservations { get; set; }
    public int PendingReservations { get; set; }
    public int ConfirmedReservations { get; set; }
    public int CancelledReservations { get; set; }
}
