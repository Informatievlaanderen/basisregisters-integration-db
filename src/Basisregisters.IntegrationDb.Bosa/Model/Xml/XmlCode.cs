namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System.Xml.Serialization;

    public class XmlCode
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "namespace")]
        public required string Namespace { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "objectIdentifier")]
        public required string ObjectIdentifier { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "versionIdentifier")]
        public required string VersionIdentifier { get; set; }
    }
}
