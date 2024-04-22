namespace Basisregisters.IntegrationDb.Bosa.Model.Xml
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(Namespace = KnownNamespaces.Tns, ElementName = "addressResponseBySource")]
    public class XmlAddressRoot
    {
        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "source")]
        public string Source { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Tns, ElementName = "address")]
        public XmlAddress[] Addresses { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Tns, TypeName = "address")]
    public class XmlAddress
    {
        [XmlAttribute("beginLifeSpanVersion")]
        public string BeginLifeSpanVersion { get; set; }

        [XmlAttribute("endLifeSpanVersion")]
        public string? EndLifeSpanVersion { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "code")]
        public XmlCode Code { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "position")]
        public XmlAddressPosition Position { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "addressStatus")]
        public XmlAddressStatus Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "boxNumber")]
        public string? BoxNumber { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "houseNumber")]
        public string HouseNumber { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "officiallyAssigned")]
        public bool OfficiallyAssigned { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "hasStreetName")]
        public XmlCode HasStreetName { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "hasMunicipality")]
        public XmlCode HasMunicipality { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "hasPostalInfo")]
        public XmlCode HasPostalInfo { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "position")]
    public class XmlAddressPosition
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "pointGeometry")]
        public XmlPointGeometry PointGeometry { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "positionGeometryMethod")]
        public string PositionGeometryMethod { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "positionSpecification")]
        public string PositionSpecification { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "pointGeometry")]
    public class XmlPointGeometry
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "point")]
        public XmlPoint Point { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "point")]
    public class XmlPoint
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "pos")]
        public XmlPointPos Pos { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "pos")]
    public class XmlPointPos
    {
        [XmlAttribute("axisLabels")]
        public string AxisLabels { get; set; }

        [XmlAttribute("srsDimension")]
        public int SrsDimension { get; set; }

        [XmlAttribute("srsName")]
        public string SrsName { get; set; }

        [XmlAttribute("uomLabels")]
        public string UomLabels { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    [XmlType(Namespace = KnownNamespaces.Com, TypeName = "addressStatus")]
    public class XmlAddressStatus
    {
        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validFrom")]
        public string ValidFrom { get; set; }

        [XmlElement(Namespace = KnownNamespaces.Com, ElementName = "validTo")]
        public string? ValidTo { get; set; }
    }
}
