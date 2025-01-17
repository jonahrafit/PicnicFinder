using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services
{
    public class ReservationService
    {
        private readonly IConfiguration _configuration;
        private readonly AdminBOContext _context;
        private readonly string _connectionString;
        private readonly SpaceService _spaceService;

        public ReservationService(IConfiguration configuration, AdminBOContext dbContext)
        {
            _configuration = configuration;
            _context = dbContext;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _spaceService = new SpaceService(configuration, dbContext);
        }

        // Create a new reservation
        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            _context.Set<Reservation>().Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<List<Reservation>> GetReservationsPagedAsyncByClient(
            int page,
            int pageSize,
            long idUser
        )
        {
            return await _context
                .Reservations.Where(r => r.EmployeeId == idUser)
                .Include(r => r.Space)
                .OrderBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetApprovedReservationsByMonthAndYearAsync(
            int month,
            int year
        )
        {
            return await _context
                .Reservations.Where(r =>
                    r.Status == ReservationStatus.CONFIRMED
                    && // Comparaison avec l'énumérateur
                    r.StartDate.Month == month
                    && r.StartDate.Year == year
                )
                .Include(r => r.Space) // Inclure les données liées
                .OrderBy(r => r.StartDate) // Optionnel, pour trier par date de début
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsAsyncByOwner(long ownerId)
        {
            var spaces = await _spaceService.GetAllSpacesAsyncByOwner(ownerId);

            if (!spaces.Any())
            {
                Console.WriteLine("No spaces found for this owner.");
                return new List<Reservation>();
            }

            var reservationsQuery = _context
                .Reservations.Include(r => r.Employee)
                .Include(r => r.Space)
                .Where(reservation =>
                    spaces.Select(space => space.Id).Contains(reservation.SpaceId)
                );

            return await reservationsQuery.ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsPagedAsyncByOwner(
            int page,
            int pageSize,
            long ownerId
        )
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;

            // Attendre que la liste des réservations soit obtenue
            var reservations = await GetReservationsAsyncByOwner(ownerId);

            // Appliquer la pagination sur la liste obtenue
            var pagedReservations = reservations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine(pagedReservations);
            return pagedReservations;
        }

        public async Task<int> GetTotalReservationsCountAsync()
        {
            return await _context.Reservations.CountAsync();
        }

        public async Task<int> GetTotalReservationsCountAsyncByClient(long employeeId)
        {
            return await _context.Reservations.Where(r => r.EmployeeId == employeeId).CountAsync();
        }

        string BuildReservationQuery(string year)
        {
            // Définir la structure de base du CTE en fonction de la présence de l'année
            string cteBase;
            if (string.IsNullOrEmpty(year))
            {
                // Générer les 12 derniers mois
                cteBase =
                    @"
                    WITH GeneratedMonths AS (
                        SELECT 
                            FORMAT(DATEADD(MONTH, -n, GETDATE()), 'yyyy-MM') AS MonthYear
                        FROM 
                            (VALUES (0), (1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11)) AS x(n)
                    )";
            }
            else if (int.TryParse(year, out int _))
            {
                // Générer les 12 mois d'une année spécifique
                cteBase =
                    @"
                    WITH GeneratedMonths AS (
                        SELECT 
                            FORMAT(DATEFROMPARTS(@Year, n, 1), 'yyyy-MM') AS MonthYear
                        FROM 
                            (VALUES (1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11), (12)) AS x(n)
                    )";
            }
            else
            {
                throw new ArgumentException("Invalid year format.");
            }

            // Requête principale avec jointure
            string mainQuery =
                @"
                SELECT 
                    gm.MonthYear,
                    ISNULL(COUNT(r.Id), 0) AS TotalReservations,
                    ISNULL(SUM(CASE WHEN r.Status = 0 THEN 1 ELSE 0 END), 0) AS PendingReservations,
                    ISNULL(SUM(CASE WHEN r.Status = 1 THEN 1 ELSE 0 END), 0) AS ConfirmedReservations,
                    ISNULL(SUM(CASE WHEN r.Status = 2 THEN 1 ELSE 0 END), 0) AS CancelledReservations
                FROM 
                    GeneratedMonths gm
                LEFT JOIN 
                    Reservations r ON FORMAT(r.StartDate, 'yyyy-MM') = gm.MonthYear
                GROUP BY 
                    gm.MonthYear
                ORDER BY 
                    gm.MonthYear";

            // Retourne la requête complète
            return $"{cteBase} {mainQuery}";
        }

        public List<ReservationStats> GetReservationsByYearOrLast12Months(string? year)
        {
            var stats = new List<ReservationStats>();
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = BuildReservationQuery(year);
                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(year) && int.TryParse(year, out int parsedYear))
                    {
                        command.Parameters.AddWithValue("@Year", parsedYear);
                        // string sqlQuery = query.Replace("@Year", parsedYear.ToString());
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stats.Add(
                                new ReservationStats
                                {
                                    MonthYear = reader["MonthYear"].ToString(),
                                    PendingReservations = Convert.ToInt32(
                                        reader["PendingReservations"]
                                    ),
                                    ConfirmedReservations = Convert.ToInt32(
                                        reader["ConfirmedReservations"]
                                    ),
                                    CancelledReservations = Convert.ToInt32(
                                        reader["CancelledReservations"]
                                    ),
                                }
                            );
                        }
                    }
                }
            }

            return stats;
        }

        // Fonction pour obtenir le nombre total de réservations d'une année donnée
        public async Task<int> GetTotalReservationsByYearAsync(int year)
        {
            int totalReservations = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(
                    @"
                        SELECT COUNT(*) 
                        FROM Reservations 
                        WHERE YEAR(ReservationDate) = @Year
                        AND Status = 1",
                    connection
                );

                command.Parameters.AddWithValue("@Year", year);

                totalReservations = (int)await command.ExecuteScalarAsync();
            }

            return totalReservations;
        }

        public async Task<double> CalculateReservationGrowthAsync(string year)
        {
            int currentYear = int.Parse(year);
            int currentYearReservations = await GetTotalReservationsByYearAsync(currentYear);
            int lastYearReservations = await GetTotalReservationsByYearAsync(currentYear - 1);

            // Calculer la croissance en pourcentage
            double growthPercentage = 0;

            if (lastYearReservations > 0)
            {
                // Calcul de la croissance si l'année précédente a des réservations
                growthPercentage =
                    ((double)currentYearReservations / lastYearReservations - 1) * 100;
            }
            else if (lastYearReservations == 0 && currentYearReservations > 0)
            {
                // Si aucune réservation l'année précédente mais des réservations cette année
                growthPercentage = 100;
            }
            else
            {
                // Cas où aucune réservation n'est présente pour l'année en cours
                growthPercentage = 0; // aucune réservation cette année
            }

            return growthPercentage;
        }

        // Construire la requête sans modifier manuellement la chaîne SQL

        public List<ViewReservation> GetAllReservationsByFilter(
            long ownerId,
            int monthFilter,
            int yearFilter,
            string status
        )
        {
            var reservations = new List<ViewReservation>();

            using (var connection = new SqlConnection(_connectionString))
            {
                // Construction de la requête SQL de base
                string query =
                    "SELECT r.Id, r.EmployeeId, r.EmployeeName,r.OwnerId, r.OwnerName,r.SpaceId, r.SpaceName, r.ReservationDate, r.StartDate, r.EndDate, r.Status, r.OwnerId "
                    + "FROM View_Reservation r "
                    + "WHERE r.OwnerId = @OwnerId AND YEAR(r.StartDate) = @YearFilter";
                if (monthFilter > 0)
                {
                    query += " AND MONTH(r.StartDate) = @MonthFilter";
                }
                if (status != "TOUS")
                {
                    query += " AND r.Status = @Status";
                }

                // Exécution de la requête avec paramètres
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OwnerId", ownerId);
                    command.Parameters.AddWithValue("@YearFilter", yearFilter);

                    if (monthFilter > 0)
                    {
                        command.Parameters.AddWithValue("@MonthFilter", monthFilter);
                    }
                    if (status != "TOUS")
                    {
                        command.Parameters.AddWithValue(
                            "@Status",
                            Enum.Parse<ReservationStatus>(status)
                        );
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ViewReservation temp = new ViewReservation
                            {
                                Id = Convert.ToInt64(reader["Id"]),
                                EmployeeId = Convert.ToInt64(reader["EmployeeId"]),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                OwnerId = Convert.ToInt64(reader["OwnerId"]),
                                OwnerName = reader["OwnerName"].ToString(),
                                SpaceId = Convert.ToInt64(reader["SpaceId"]),
                                SpaceName = reader["SpaceName"].ToString(),
                                ReservationDate = Convert.ToDateTime(reader["ReservationDate"]),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                Status = (ReservationStatus)
                                    Enum.Parse(
                                        typeof(ReservationStatus),
                                        reader["Status"].ToString()
                                    ),
                            };
                            reservations.Add(temp);
                        }
                    }
                }
            }

            return reservations;
        }

        // _______________________
        // ______________________
        // ______________________
        // ______________________
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Set<Reservation>().Include(r => r.Employee).ToListAsync();
        }

        // Retrieve a single reservation by ID
        public async Task<Reservation?> GetReservationByIdAsync(long id)
        {
            return await _context
                .Set<Reservation>()
                .Include(r => r.Employee)
                .Include(r => r.Space)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Update an existing reservation
        public async Task<Reservation?> UpdateReservationAsync(
            long id,
            Reservation updatedReservation
        )
        {
            var existingReservation = await GetReservationByIdAsync(id);
            if (existingReservation == null)
            {
                return null;
            }

            existingReservation.EmployeeId = updatedReservation.EmployeeId;
            existingReservation.SpaceId = updatedReservation.SpaceId;
            existingReservation.ReservationDate = updatedReservation.ReservationDate;
            existingReservation.StartDate = updatedReservation.StartDate;
            existingReservation.EndDate = updatedReservation.EndDate;
            existingReservation.Status = updatedReservation.Status;

            _context.Set<Reservation>().Update(existingReservation);
            await _context.SaveChangesAsync();

            return existingReservation;
        }

        // Delete a reservation
        public async Task<bool> DeleteReservationAsync(long id)
        {
            var reservation = await GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return false;
            }

            _context.Set<Reservation>().Remove(reservation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
