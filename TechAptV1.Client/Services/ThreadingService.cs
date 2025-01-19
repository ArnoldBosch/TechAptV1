// Copyright © 2025 Always Active Technologies PTY Ltd

using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services;

/// <summary>
/// Default constructor providing DI Logger and Data Service
/// </summary>
/// <param name="logger"></param>
/// <param name="dataService"></param>
public sealed class ThreadingService(ILogger<ThreadingService> logger, DataService dataService)
{
    private readonly List<int> _sharedList = new();
    private readonly object _lock = new();

    private int _oddNumbers = 0;
    private int _evenNumbers = 0;
    private int _primeNumbers = 0;
    private int _totalNumbers = 0;

    public int GetOddNumbers() => _oddNumbers;
    public int GetEvenNumbers() => _evenNumbers;
    public int GetPrimeNumbers() => _primeNumbers;
    public int GetTotalNumbers() => _totalNumbers;

    /// <summary>
    /// Start the random number generation process
    /// </summary>
    public async Task Start()
    {
        // Start threads for odd and prime number generation
        Thread oddThread = new(AddOddNumbers);
        Thread primeThread = new(AddPrimeNumbers);

        oddThread.Start();
        primeThread.Start();

        // Wait for the list to reach 2,500,000 entries
        while (true)
        {
            lock (_lock)
            {
                if (_sharedList.Count >= 2_500_000) break;
            }

        }

        // Start the even number thread
        Thread evenThread = new(AddEvenNumbers);
        evenThread.Start();

        // Wait for all threads to finish
        oddThread.Join();
        primeThread.Join();
        evenThread.Join();

        // Sort and count numbers
        SortAndCount();
    }

    /// <summary>
    /// Persist the results to the SQLite database
    /// </summary>
    public async Task Save()
    {
        logger.LogInformation("Save");
        //We can take a shortcut here because all the prime numbers are negated
        await dataService.SaveAsync(_sharedList.Select(n => new Number { Value = n, IsPrime = n < 0 ? 1 : 0 }));
    }
    /// <summary>
    /// Add odd numbers to the list
    /// </summary>
    private void AddOddNumbers()
    {
        Random random = new();
        while (true)
        {
            lock (_lock)
            {
                if (_sharedList.Count >= 10_000_000) break;

                // Check if the Last binary Bit is 0 then make it 1 to ensure Odd Numbers
                // Odd Numbers never end in 0 in binary
                int oddNumber = random.Next(1, int.MaxValue) | 1; 
                _sharedList.Add(oddNumber);
            }
        }
    }
    /// <summary>
    /// Add prime numbers to the list and negate them
    /// </summary>
    private void AddPrimeNumbers()
    {
        int number = 2;
        while (true)
        {
            lock (_lock)
            {
                if (_sharedList.Count >= 10_000_000) break;

                if (IsPrime(number))
                {
                    _sharedList.Add(-number); // Negate the prime number
                }
            }
            number++;
        }
    }
    /// <summary>
    /// Add even numbers to the list
    /// </summary>
    private void AddEvenNumbers()
    {
        Random random = new();
        while (true)
        {
            lock (_lock)
            {
                if (_sharedList.Count >= 10_000_000) break;

                // Check if the Last binary Bit is 1 then make it 0  to ensure Even Numbers
                // Even Numbers never end in 1 in binary
                int evenNumber = random.Next(1, int.MaxValue) & ~1;
                _sharedList.Add(evenNumber);
            }
        }
    }
    /// <summary>
    /// Sort the list and count the numbers
    /// </summary>
    private void SortAndCount()
    {
        lock (_lock)
        {
            _sharedList.Sort();

            //Use Bitwise AND to check if the last bit is 1 or 0, it is faster than modulo and division
            _oddNumbers = _sharedList.Count(n => (n & 1) == 1);
            _evenNumbers = _sharedList.Count(n => (n & 1) == 0);
            _primeNumbers = _sharedList.Count(n => n < 0);
            _totalNumbers = _sharedList.Count;
        }
    }
    /// <summary>
    /// Check if a number is prime
    /// </summary>
    /// <param name="number"></param>
    /// <returns>true if Prime</returns>
    private bool IsPrime(int number)
    {
        if (number <= 1) return false; // Negative numbers, 0, and 1 are not prime
        if (number <= 3) return true; // 2 and 3 are prime
        if ((number & 1) == 0 || number % 3 == 0) return false; // Eliminate even numbers and multiples of 3

        int boundary = (int)Math.Sqrt(number);
        for (int i = 5; i <= boundary; i += 6)
        {
            // Check divisibility by numbers of the form 6k ± 1
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }

        return true;
    }
}
