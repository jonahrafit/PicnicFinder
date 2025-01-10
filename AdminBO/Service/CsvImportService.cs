using System.Globalization;
using System.IO;
using AdminBO.Models;
using BCrypt.Net;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdminBO.Services;

public class CsvImportService
{
    private readonly AdminBOContext _dbContext;

    public CsvImportService(AdminBOContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportUsersFromCsvAsync(Stream stream)
    {
        using (var reader = new StreamReader(stream))
        using (
            var csv = new CsvReader(
                reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                }
            )
        )
        {
            var records = csv.GetRecords<User>().ToList();
            foreach (var record in records)
            {
                // Hacher le mot de passe
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(record.Password);

                // Assurez-vous que les champs Role et Status sont bien des chaînes
                var roleString = record.Role.ToString(); // Convertir en chaîne si nécessaire
                var statusString = record.Status.ToString(); // Convertir en chaîne si nécessaire

                // Créer un nouvel utilisateur avec les valeurs extraites du fichier CSV
                var user = new User
                {
                    Email = record.Email,
                    Password = hashedPassword,
                    // Utiliser Enum.TryParse avec des chaînes pour convertir en énumérations
                    Role = Enum.TryParse(roleString, out UserRole role) ? role : UserRole.CLIENT, // Valeur par défaut si l'énumération échoue
                    Phone = record.Phone,
                    Name = record.Name,
                    // Utiliser Enum.TryParse et assigner un statut par défaut si la conversion échoue
                    Status = Enum.TryParse(statusString, out UserStatus status)
                        ? status
                        : UserStatus.PENDING_APPROVAL,
                };

                Console.WriteLine("-----------------------");
                Console.WriteLine(user.ToString());

                // Ajouter l'utilisateur à la base de données
                _dbContext.Users.Add(user);
            }

            // Enregistrer les modifications dans la base de données
            await _dbContext.SaveChangesAsync();
        }
    }
}
