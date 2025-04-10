namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = KnownNamespaces.Tns, ElementName = "addressResponseBySource")]
    public class XmlAddressRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public required string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "address")]
        public required XmlAddress[] Addresses { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "address")]
    public class XmlAddress
    {
        [XmlAttribute("beginLifeSpanVersion")]
        public required string BeginLifeSpanVersion { get; set; }

        [XmlAttribute("endLifeSpanVersion")]
        public string? EndLifeSpanVersion { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public required XmlCode Code { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "position")]
        public required XmlAddressPosition Position { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "addressStatus")]
        public required XmlAddressStatus Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "boxNumber")]
        public string? BoxNumber { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "houseNumber")]
        public required string HouseNumber { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "officiallyAssigned")]
        public bool OfficiallyAssigned { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "hasStreetName")]
        public required XmlCode HasStreetName { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "hasMunicipality")]
        public required XmlCode HasMunicipality { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "hasPostalInfo")]
        public required XmlCode HasPostalInfo { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "position")]
    public class XmlAddressPosition
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "pointGeometry")]
        public required XmlPointGeometry PointGeometry { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "positionGeometryMethod")]
        public required string PositionGeometryMethod { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "positionSpecification")]
        public required string PositionSpecification { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "pointGeometry")]
    public class XmlPointGeometry
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "point")]
        public required XmlPoint Point { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "point")]
    public class XmlPoint
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "pos")]
        public required XmlPointPos Pos { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "pos")]
    public class XmlPointPos
    {
        [XmlAttribute("axisLabels")]
        public required string AxisLabels { get; set; }

        [XmlAttribute("srsDimension")]
        public int SrsDimension { get; set; }

        [XmlAttribute("srsName")]
        public required string SrsName { get; set; }

        [XmlAttribute("uomLabels")]
        public required string UomLabels { get; set; }

        [XmlText]
        public required string Value { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "addressStatus")]
    public class XmlAddressStatus
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "status")]
        public required string Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validFrom")]
        public required string ValidFrom { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validTo")]
        public string? ValidTo { get; set; }
    }
}
