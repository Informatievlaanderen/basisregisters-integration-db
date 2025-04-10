namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = KnownNamespaces.Tns, ElementName = "postalInfoResponseBySource")]
    public class XmlPostalInfoRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public required string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "postalInfo")]
        public required XmlPostalInfo[] PostalInfos { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "postalInfo")]
    public class XmlPostalInfo
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public required XmlCode Code { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "name")]
        public required XmlName Name { get; set; }
    }
}
