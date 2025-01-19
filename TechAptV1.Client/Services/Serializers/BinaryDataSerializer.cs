// Copyright © 2025 Always Active Technologies PTY Ltd

using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services.Serializers
{
    public class BinaryDataSerializer : IDataSerializer
    {
        public async Task ExportAsync(IEnumerable<Number> data, string outputPath)
        {
            // Write binary data to the file
            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(fileStream);

            await Task.Run(() =>
            {
                foreach (var number in data)
                {
                    writer.Write(number.Value);
                    writer.Write(number.IsPrime);
                }
            });
        }

        public string FileExtension => ".bin";
    }
}
