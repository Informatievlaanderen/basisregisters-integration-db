namespace Basisregisters.IntegrationDb.Bosa
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Model.Xml;

    public static class RegistryZipArchiveFactory
    {
        public static async Task<ZipArchive> Create<T>(
            Stream outputStream,
            T serializable,
            string xmlFileName)
        {
            var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);

            await using var xmlStream = zipArchive.CreateEntry(xmlFileName).Open();
            var streamWriter = XmlWriter.Create(xmlStream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "   "
            });

            var serializer = new XmlSerializer(typeof(XmlPostalInfoRoot));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("tns", KnownNamespaces.Tns);
            namespaces.Add("com", KnownNamespaces.Com);

            serializer.Serialize(streamWriter, serializable, namespaces);

            return zipArchive;
        }
    }
}
