// Copyright © 2025 Always Active Technologies PTY Ltd

using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services.Serializers
{
    public interface IDataSerializer
    {
        Task ExportAsync(IEnumerable<Number> data, string outputStream);
        string FileExtension { get; }
    }
}
