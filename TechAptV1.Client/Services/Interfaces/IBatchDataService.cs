// Copyright © 2025 Always Active Technologies PTY Ltd

using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services.Interfaces
{
    public interface IBatchDataService : IDataService
    {
        Task SaveBatchAsync(IEnumerable<Number> dataList);
    }

}
