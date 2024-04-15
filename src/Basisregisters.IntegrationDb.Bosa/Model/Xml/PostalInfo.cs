namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public static class KnownNamespaces
    {
        public const string Com = "http://fsb.belgium.be/data/common";
        public const string Tns = "http://fsb.belgium.be/mappingservices/FullDownload/v1_00";
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "postalInfoResponseBySource")]
    public class PostalInfoRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "postalInfo")]
        public ICollection<PostalInfo> PostalInfos { get; set; }
    }

    public class PostalInfo
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public Code Code { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "name")]
        public Name Name { get; set; }
    }

    public class Code
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "namespace")]
        public string Namespace { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "objectIdentifier")]
        public string ObjectIdentifier { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "versionIdentifier")]
        public string VersionIdentifier { get; set; }
    }

    public class Name
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "language")]
        public string Language { get; set; }
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "spelling")]
        public string Spelling { get; set; }
    }
}
