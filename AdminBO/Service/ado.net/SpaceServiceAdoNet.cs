using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services;

public class SpaceServiceAdoNet
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public SpaceServiceAdoNet(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public Space GetSpaceById(long spaceId)
    {
        Space space = null;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Spaces WHERE Id = @Id";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.BigInt) { Value = spaceId });

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    space = new Space
                    {
                        Id = reader.GetInt64(reader.GetOrdinal("Id")),
                        OwnerId = reader.GetInt64(reader.GetOrdinal("OwnerId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Latitude = reader.GetDouble(reader.GetOrdinal("Latitude")),
                        Longitude = reader.GetDouble(reader.GetOrdinal("Longitude")),
                        Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("Description")),
                        Status = (SpaceStatus)
                            Enum.Parse(
                                typeof(SpaceStatus),
                                reader.GetString(reader.GetOrdinal("Status"))
                            ),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                        Photos = new List<string>(), // Vous devrez peut-être ajouter une logique pour charger les photos à partir d'une autre table
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Vous pouvez ajouter ici un mécanisme de logging ou un autre traitement des erreurs
            }
        }

        return space;
    }
}
