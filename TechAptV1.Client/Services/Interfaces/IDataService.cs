// Copyright © 2025 Always Active Technologies PTY Ltd

using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services.Interfaces
{
    /// <summary>
    /// Interface for data access services handling Number entities.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Saves a collection of Number objects to the database.
        /// </summary>
        /// <param name="dataList">The list of Number objects to save.</param>
        Task SaveAsync(IEnumerable<Number> dataList);

        /// <summary>
        /// Fetches a specified number of Number records from the database.
        /// </summary>
        /// <param name="count">The number of records to fetch.</param>
        /// <returns>A list of Number objects.</returns>
        Task<List<Number>> GetAsync(int count);

        /// <summary>
        /// Fetches all Number records from the database.
        /// </summary>
        /// <returns>A list of all Number objects.</returns>
        Task<List<Number>> GetAllWithLinqAsync();
    }
}
