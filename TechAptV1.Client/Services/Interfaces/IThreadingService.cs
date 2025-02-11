// Copyright © 2025 Always Active Technologies PTY Ltd

namespace TechAptV1.Client.Services.Interfaces
{
    /// <summary>
    /// Interface for the threading service that handles number processing.
    /// </summary>
    public interface IThreadingService
    {
        /// <summary>
        /// Start the random number generation process.
        /// </summary>
        Task Start();

        /// <summary>
        /// Persist the results to the database.
        /// </summary>
        Task Save();

        /// <summary>
        /// Gets the count of odd numbers generated.
        /// </summary>
        int GetOddNumbers();

        /// <summary>
        /// Gets the count of even numbers generated.
        /// </summary>
        int GetEvenNumbers();

        /// <summary>
        /// Gets the count of prime numbers generated.
        /// </summary>
        int GetPrimeNumbers();

        /// <summary>
        /// Gets the total count of numbers generated.
        /// </summary>
        int GetTotalNumbers();
    }

}
