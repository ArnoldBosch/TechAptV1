// Copyright © 2025 Always Active Technologies PTY Ltd

using System.Xml;
using System.Xml.Serialization;
using TechAptV1.Client.Models;

namespace TechAptV1.Client.Services.Serializers
{
    public class XmlDataSerializer : IDataSerializer
    {
        public async Task ExportAsync(IEnumerable<Number> data, string outputPath)
        {
            // Create an XML serializer for the List<Number>
            var serializer = new XmlSerializer(typeof(List<Number>));

            // Write the data to an XML file
            using var writer = new StreamWriter(outputPath);
            await Task.Run(() => serializer.Serialize(writer, data));
        }

        public string FileExtension => ".xml";
    }
}
