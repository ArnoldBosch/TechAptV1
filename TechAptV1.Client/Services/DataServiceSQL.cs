using System.Diagnostics;
using SQLite;
using TechAptV1.Client.Models;
using System.Data;
using TechAptV1.Client.Services.Interfaces;

namespace TechAptV1.Client.Services;

/// <summary>
/// Data Access Service for interfacing with the SQLite Database using raw SQL queries
/// </summary>
public sealed class DataServiceSQL: IBatchDataService
{
    private readonly ILogger<DataService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly SQLiteAsyncConnection _connection;

    public DataServiceSQL(ILogger<DataService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        // Get the connection string from appsettings.json
        _connectionString = _configuration.GetConnectionString("Default")
                           ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        _connection = new SQLiteAsyncConnection(_connectionString);
        // Optional: Create the table if it does not exist
        _ = InitializeDatabase();
    }

    private async Task InitializeDatabase()
    {
        try
        {
            // Ensure the table exists using raw SQL
            string sql = @"CREATE TABLE IF NOT EXISTS Number (Id INTEGER PRIMARY KEY AUTOINCREMENT, Value TEXT);";
            await _connection.ExecuteAsync(sql);

            _logger.LogInformation("Database initialized and ensured 'Number' table exists.");

            // Optimize database for bulk inserts
            await _connection.ExecuteAsync("PRAGMA journal_mode = WAL;");
            await _connection.ExecuteAsync("PRAGMA synchronous = OFF;");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error initializing database: {ex.Message}");
            throw;
        }
    }

    public async Task SaveAsync(IEnumerable<Number> dataList)
    {
        _logger.LogInformation("Saving data...");

        var sql = "INSERT INTO Number (Value) VALUES (@Value);";
        try
        {
            await _connection.RunInTransactionAsync(conn =>
            {
                foreach (var item in dataList)
                {
                    conn.Execute(sql, new { Value = item.Value });
                }
            });

            _logger.LogInformation($"Saved {dataList.Count()} records to the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving data: {ex.Message}");
            throw;
        }
    }
    public async Task SaveBatchAsync(IEnumerable<Number> dataList)
    {
        _logger.LogInformation("Saving data...");
        try
        {
            int batchSize = 10000; // Adjust this number based on performance tests
            List<Number> batchList = new List<Number>(batchSize);

            foreach (var item in dataList)
            {
                batchList.Add(item);
                if (batchList.Count >= batchSize)
                {
                    await InsertBatchAsync(batchList);
                    batchList.Clear();
                }
            }
            if (batchList.Count > 0)
            {
                await InsertBatchAsync(batchList); // Insert any remaining items
            }

            _logger.LogInformation("All data saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving data: {ex.Message}");
            throw;
        }
    }
    private async Task InsertBatchAsync(List<Number> batchList)
    {
        var sql = "INSERT INTO Number (Value) VALUES (@Value);";
        await _connection.RunInTransactionAsync(conn =>
        {
            foreach (var item in batchList)
            {
                conn.Execute(sql, new { Value = item.Value });
            }
        });
    }

    public async Task<List<Number>> GetAsync(int count)
    {
        _logger.LogInformation("Fetching data...");

        var sql = $"SELECT * FROM Number LIMIT {count};";
        var results = await _connection.QueryAsync<Number>(sql);
        return results;
    }

    public async Task<List<Number>> GetAllWithLinqAsync()
    {
        var sql = "SELECT * FROM Number;";
        var results = await _connection.QueryAsync<Number>(sql);
        return results;
    }
}
