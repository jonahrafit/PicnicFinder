using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services;

public class SpaceActivityService
{
    private readonly IConfiguration _configuration;
    private readonly AdminBOContext _context;
    private readonly string _connectionString;
    private readonly SpaceService _spaceService;

    public SpaceActivityService(IConfiguration configuration, AdminBOContext dbContext)
    {
        _configuration = configuration;
        _context = dbContext;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _spaceService = new SpaceService(configuration, dbContext);
    }

    public async Task<List<SpaceActivity>> GetAllSpaceActivitysAsync()
    {
        return await _context.SpaceActivities.ToListAsync();
    }

    public async Task<List<SpaceActivity>> GetAllSpaceActivityLinksBySpaceIdAsync(long spaceId)
    {
        var spaceActivities = new List<SpaceActivity>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand(
                @"
                SELECT sa.Id,sa.Name, ca.Id as CategoryId, ca.Name as CategoryName
                FROM SpaceActivityLinks sal
                INNER JOIN SpaceActivities sa ON sal.SpaceActivityId = sa.Id
                LEFT JOIN CategoryActivities ca ON sa.CategoryId = ca.Id
                WHERE sal.SpaceId = @SpaceId",
                connection
            );

            command.Parameters.AddWithValue("@SpaceId", spaceId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var spaceActivity = new SpaceActivity
                    {
                        Id = reader.GetInt64(0),
                        Name = reader.GetString(1),
                        CategoryId = reader.GetInt64(reader.GetOrdinal("CategoryId")),
                        CategoryActivity = new CategoryActivity
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("CategoryId")),
                            Name = reader.GetString(reader.GetOrdinal("CategoryName")),
                        },
                    };

                    spaceActivities.Add(spaceActivity);
                }
            }
        }

        return spaceActivities;
    }

    public async Task<List<String>> GetAllSpaceActivityNameBySpaceIdAsync(long spaceId)
    {
        var spaceActivities = new List<String>();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(
                @"
                SELECT sa.Name 
                FROM SpaceActivityLinks sal
                INNER JOIN SpaceActivities sa ON sal.SpaceActivityId = sa.Id
                WHERE sal.SpaceId = @SpaceId",
                connection
            );

            command.Parameters.AddWithValue("@SpaceId", spaceId);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    spaceActivities.Add(reader.GetString(0));
                }
            }
        }
        return spaceActivities;
    }

    // Retourner un SpaceActivity par ID
    public async Task<SpaceActivity?> GetSpaceActivityByIdAsync(long id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var query =
                @"
            SELECT sa.Id, sa.Name, sa.CategoryId, ca.Name AS CategoryName
            FROM SpaceActivities sa
            LEFT JOIN CategoryActivities ca ON sa.CategoryId = ca.Id
            WHERE sa.Id = @Id";

            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new SpaceActivity
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        CategoryId = reader.GetInt64(reader.GetOrdinal("CategoryId")),
                        CategoryActivity = new CategoryActivity
                        {
                            Id = reader.GetInt64(reader.GetOrdinal("CategoryId")),
                            Name = reader.GetString(reader.GetOrdinal("CategoryName")),
                        },
                    };
                }
            }
        }

        return null;
    }

    // Créer un nouvel SpaceActivity
    public async Task CreateSpaceActivityAsync(SpaceActivity spaceActivity)
    {
        if (spaceActivity == null)
        {
            throw new ArgumentNullException(nameof(spaceActivity), "SpaceActivity cannot be null.");
        }

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand(
                "INSERT INTO SpaceActivities (Name) VALUES (@Name); SELECT SCOPE_IDENTITY();",
                connection
            );

            command.Parameters.AddWithValue("@Name", spaceActivity.Name);

            try
            {
                spaceActivity.Id = Convert.ToInt64(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating SpaceActivity: {ex.Message}");
                throw;
            }
        }
    }

    // Mettre à jour un SpaceActivity
    public async Task UpdateSpaceActivityAsync(SpaceActivity spaceActivity)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand(
                "UPDATE SpaceActivities SET Name = @Name WHERE Id = @Id",
                connection
            );

            command.Parameters.AddWithValue("@Name", spaceActivity.Name);
            command.Parameters.AddWithValue("@Id", spaceActivity.Id);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Supprimer un SpaceActivity
    public async Task DeleteSpaceActivityAsync(long id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand("DELETE FROM SpaceActivities WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);

            await command.ExecuteNonQueryAsync();
        }
    }

    // Vérifier si un SpaceActivity existe
    public async Task<bool> SpaceActivityExistsAsync(long id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var command = new SqlCommand(
                "SELECT COUNT(1) FROM SpaceActivities WHERE Id = @Id",
                connection
            );
            command.Parameters.AddWithValue("@Id", id);

            var result = (int)await command.ExecuteScalarAsync();
            return result > 0;
        }
    }
}
