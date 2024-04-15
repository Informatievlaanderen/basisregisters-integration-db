namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "postalInfoResponseBySource")]
    public class XmlPostalInfoRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "postalInfo")]
        public XmlPostalInfo[] PostalInfos { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "postalInfo")]
    public class XmlPostalInfo
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public XmlCode Code { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "name")]
        public XmlName Name { get; set; }
    }
}
