namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = KnownNamespaces.Tns, ElementName = "municipalityResponseBySource")]
    public class XmlMunicipalityRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "municipality")]
        public XmlMunicipality[] Municipalities { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "municipality")]
    public class XmlMunicipality
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public XmlCode Code { get; set; }
        
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "name")]
        public XmlName[] Name { get; set; }
    }
}
