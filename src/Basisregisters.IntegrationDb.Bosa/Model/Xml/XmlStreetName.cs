namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = KnownNamespaces.Tns, ElementName = "streetNameResponseBySource")]
    public class XmlStreetNameRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public required string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public required DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "streetName")]
        public required XmlStreetName[] StreetNames { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "streetName")]
    public class XmlStreetName
    {
        [XmlAttribute("beginLifeSpanVersion")]
        public required string BeginLifeSpanVersion { get; set; }

        [XmlAttribute("endLifeSpanVersion")]
        public string? EndLifeSpanVersion { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public required XmlCode Code { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "name")]
        public required XmlName[] Names { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "streetNameStatus")]
        public required XmlStreetNameStatus Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "type")]
        public required string Type { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "homonymAddition")]
        public string? HomonymAddition { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "isAssignedByMunicipality")]
        public required XmlCode IsAssignedByMunicipality { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "streetNameStatus")]
    public class XmlStreetNameStatus
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "status")]
        public required string Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validFrom")]
        public required string ValidFrom { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validTo")]
        public string? ValidTo { get; set; }
    }
}
