namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System.Xml.Serialization;

    public class XmlName
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "language")]
        public string Language { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "spelling")]
        public string Spelling { get; set; }
    }
}
