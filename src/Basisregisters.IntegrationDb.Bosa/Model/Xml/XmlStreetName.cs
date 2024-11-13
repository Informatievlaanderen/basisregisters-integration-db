namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = KnownNamespaces.Tns, ElementName = "streetNameResponseBySource")]
    public class XmlStreetNameRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "streetName")]
        public XmlStreetName[] StreetNames { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "streetName")]
    public class XmlStreetName
    {
        [XmlAttribute("beginLifeSpanVersion")]
        public string BeginLifeSpanVersion { get; set; }

        [XmlAttribute("endLifeSpanVersion")]
        public string? EndLifeSpanVersion { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public XmlCode Code { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "name")]
        public XmlName[] Names { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "streetNameStatus")]
        public XmlStreetNameStatus Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "type")]
        public string Type { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "homonymAddition")]
        public string? HomonymAddition { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "isAssignedByMunicipality")]
        public XmlCode IsAssignedByMunicipality { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "streetNameStatus")]
    public class XmlStreetNameStatus
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validFrom")]
        public string ValidFrom { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validTo")]
        public string? ValidTo { get; set; }
    }
}
