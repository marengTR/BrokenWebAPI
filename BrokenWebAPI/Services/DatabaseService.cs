using System.Data;
using System.Data.SqlClient;
using BrokenWebAPI.Models;

namespace BrokenWebAPI.Services;

public class DatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                           "Server=localhost;Database=vulnerable_db;User Id=sa;Password=VeryStr0ngP@ssw0rd!;";
        _logger = logger;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        string query = "SELECT Id, Username, Password, Email, Role FROM Users WHERE Username = '" + username + "'";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            try
            {
                await connection.OpenAsync();
                using SqlCommand command = new SqlCommand(query, connection);
                using SqlDataReader reader = await command.ExecuteReaderAsync();
                
                if (await reader.ReadAsync())
                {
                    return new User
                    {
                        Id = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Password = reader.GetString(2),
                        Email = reader.GetString(3),
                        Role = reader.GetString(4)
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing SQL query: {Query}", query);
                throw new Exception($"Database error: {ex.Message}, Query: {query}");
            }
        }
        
        return null;
    }

    public async Task<bool> ValidateUser(string username, string password)
    {
        string query = $"SELECT COUNT(*) FROM Users WHERE Username = '{username}' AND Password = '{password}'";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using SqlCommand command = new SqlCommand(query, connection);
            
            try
            {
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login validation error");
                return false;
            }
        }
    }

    public async Task<object> GetUserData(string serializedData)
    {
        try
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using var memStream = new MemoryStream(Convert.FromBase64String(serializedData));
            

            var deserializedObject = formatter.Deserialize(memStream);
            
            return deserializedObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Deserialization error");
            throw;
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        _logger.LogInformation("Database initialized");
    }
}
