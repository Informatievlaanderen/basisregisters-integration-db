namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System.Xml.Serialization;

    public class XmlCode
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "namespace")]
        public string Namespace { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "objectIdentifier")]
        public string ObjectIdentifier { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "versionIdentifier")]
        public string VersionIdentifier { get; set; }
    }
}
