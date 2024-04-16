namespace Basisregisters.IntegrationDb.Bosa
{
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Model.Xml;

    public static class RegistryXmlSerializer
    {
        public static void Serialize<T>(T input, Stream outputStream)
        {
            var streamWriter = XmlWriter.Create(outputStream, new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                IndentChars = "   "
            });

            var serializer = new XmlSerializer(typeof(T));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("com", KnownNamespaces.Com);
            namespaces.Add("tns", KnownNamespaces.Tns);
            namespaces.Add("xsi", KnownNamespaces.Xsi);

            serializer.Serialize(streamWriter, input, namespaces);

            outputStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
