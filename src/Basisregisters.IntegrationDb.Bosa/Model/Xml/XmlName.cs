namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System.Xml.Serialization;

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "name")]
    public class XmlName
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "language")]
        public required string Language { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "spelling")]
        public required string Spelling { get; set; }
    }
}
