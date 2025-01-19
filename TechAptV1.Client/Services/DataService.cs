// Copyright © 2025 Always Active Technologies PTY Ltd

using System.Diagnostics;
using SQLite;
using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services;

/// <summary>
/// Data Access Service for interfacing with the SQLite Database
/// </summary>
public sealed class DataService
{
    private readonly ILogger<DataService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly SQLiteAsyncConnection _connection;

    /// <summary>
    /// Default constructor providing DI Logger and Configuration
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public DataService(ILogger<DataService> logger, IConfiguration configuration)
    {
        this._logger = logger;
        this._configuration = configuration;

        // Get the connection string from appsettings.json
        _connectionString = _configuration.GetConnectionString("Default")
                           ?? throw new InvalidOperationException("Connection string 'NumberDb' not found.");

        _connection = new SQLiteAsyncConnection(_connectionString.Split("=")[1]);
        // Optional: Create the table if it does not exist
        InitializeDatabase();
    }

    /// <summary>
    /// Create the Number table if it does not already exist.
    /// </summary>
    private async Task InitializeDatabase()
    {
        try
        {
            // Ensure the table exists using sqlite-net-pcl
            await _connection.CreateTableAsync<Number>();

            _logger.LogInformation("Database initialized and ensured 'Number' table exists.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error initializing database: {ex.Message}");
            throw;
        }
    }


    /// <summary>
    /// Save a list of Number objects to the SQLite Database
    /// </summary>
    /// <param name="dataList"></param>
    /// <returns></returns>
    public async Task SaveAsync(IEnumerable<Number> dataList)
    {
        _logger.LogInformation("Saving data...");

        try
        {
            await _connection.RunInTransactionAsync(connection =>
            {
                connection.InsertAll(dataList);
            });

            _logger.LogInformation($"Saved {dataList.Count()} records to the database.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving data: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Fetch N records from the SQLite Database where N is specified by the count parameter
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<List<Number>> GetAsync(int count)
    {
        _logger.LogInformation("GetAsync");

        var results = await _connection.Table<Number>()
                                       .Take(count) // Get only the specified number of records (LIMIT 20)
                                       .ToListAsync();
        return results;
    }
    public async Task<List<Number>> GetAllWithLinqAsync()
    {
        var results = await _connection.Table<Number>().ToListAsync();
        return results;
    }
}
