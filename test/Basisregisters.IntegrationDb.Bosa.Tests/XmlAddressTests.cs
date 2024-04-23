namespace Basisregisters.IntegrationDb.Bosa.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging.Abstractions;
    using Model.Database;
    using Moq;
    using NodaTime;
    using NodaTime.Testing;
    using Repositories;
    using Xunit;

    public class XmlAddressTests
    {
        private readonly Address[] _given;
        private const string AddressNamespace = "https://data.vlaanderen.be/id/adres";
        private const string MunicipalityNamespace = "https://data.vlaanderen.be/id/gemeente";
        private const string StreetNameNamespace = "https://data.vlaanderen.be/id/straatnaam";
        private const string PostalInfoNamespace = "https://data.vlaanderen.be/id/postinfo";

        private static readonly DateTimeOffset DateBeforeMigration = new DateTimeOffset(new DateTime(2023, 11, 9));

        public XmlAddressTests()
        {
            _given =
            [
                new Address(
                    AddressNamespace,
                    200001,
                    14602,
                    "2230",
                    DateBeforeMigration,
                    "2015-08-31T17:19:02",
                    DateBeforeMigration,
                    188473.52,
                    193390.22,
                    31370,
                    GeometryMethod.DerivedFromObject,
                    GeometrySpecification.BuildingUnit,
                    AddressStatus.Retired,
                    "59",
                    "0101",
                    true
                ),
                new Address(
                    AddressNamespace,
                    30328681,
                    6345,
                    "2520",
                    new DateTimeOffset(2024, 4, 4, 18, 44, 37, TimeSpan.FromHours(2)),
                    null,
                    new DateTimeOffset(2014, 4, 4, 18, 44, 32, TimeSpan.FromHours(2)),
                    165260.87,
                    210822.78,
                    31370,
                    GeometryMethod.DerivedFromObject,
                    GeometrySpecification.Parcel,
                    AddressStatus.Current,
                    "1",
                    null,
                    true
                ),
                new Address(
                    AddressNamespace,
                    30328682,
                    6345,
                    "2520",
                    new DateTimeOffset(2024, 4, 4, 18, 44, 37, TimeSpan.FromHours(2)),
                    "2016-08-31T17:19:02",
                    new DateTimeOffset(2014, 4, 4, 18, 44, 32, TimeSpan.FromHours(2)),
                    165260.87,
                    210822.78,
                    31370,
                    GeometryMethod.DerivedFromObject,
                    GeometrySpecification.Parcel,
                    AddressStatus.Current,
                    "2",
                    null,
                    true
                )
            ];
        }

        [Fact]
        public async Task GivenAddress_ThenSerializesCorrectly()
        {
            var municipalities = new Municipality[]
            {
                new (MunicipalityNamespace, "13013", DateBeforeMigration, "Gemeente", "2002-08-13T17:32:32"),
                new (MunicipalityNamespace, "11035", DateBeforeMigration, "Gemeente", "2002-08-13T17:32:32")
            };

            var postalInfos = new PostalInfo[]
            {
                new (PostalInfoNamespace, "2230", DateBeforeMigration, "Postcode", "2002-08-13T16:37:33"),
                new (PostalInfoNamespace, "2520", DateBeforeMigration, "Postcode", "2002-08-13T16:37:33")
            };

            var streetNames = new StreetNameIdentifier[]
            {
                new (StreetNameNamespace, 14602, DateBeforeMigration, "2015-01-19T20:09:59.577", "13013"),
                new (StreetNameNamespace, 6345, DateBeforeMigration, "2014-10-22T17:45:58.707", "11035")
            };

            var municipalityRepo = new Mock<IMunicipalityRepository>();
            municipalityRepo
                .Setup(x => x.GetFlemish())
                .Returns(municipalities);

            var postalInfoRepo = new Mock<IPostalInfoRepository>();
            postalInfoRepo
                .Setup(x => x.GetFlemish())
                .Returns(postalInfos);

            var streetNameRepo = new Mock<IStreetNameRepository>();
            streetNameRepo
                .Setup(x => x.GetFlemishIdentifiers())
                .Returns(streetNames);

            var addressRepo = new Mock<IAddressRepository>();
            addressRepo
                .Setup(x => x.GetFlemish())
                .Returns(_given);

            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var service = new AddressService(clock, addressRepo.Object, streetNameRepo.Object, municipalityRepo.Object, postalInfoRepo.Object, new NullLoggerFactory());

            await using var outputStream = new MemoryStream();
            service.CreateXml(outputStream);

            var xml = Encoding.UTF8.GetString(outputStream.ToArray()).Trim();

            // serialize to xml

            var expected =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <tns:addressResponseBySource xmlns:com="http://fsb.belgium.be/data/common" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tns="http://fsb.belgium.be/mappingservices/FullDownload/v1_00">
                  <tns:source>flanders</tns:source>
                  <tns:timestamp>1970-01-01T01:00:00+01:00</tns:timestamp>
                  <tns:address beginLifeSpanVersion="2023-11-09T00:00:00" endLifeSpanVersion="2015-08-31T17:19:02">
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/adres</com:namespace>
                      <com:objectIdentifier>200001</com:objectIdentifier>
                      <com:versionIdentifier>2015-08-31T17:19:02</com:versionIdentifier>
                    </com:code>
                    <com:position>
                      <com:pointGeometry>
                        <com:point>
                          <com:pos axisLabels="x y" srsDimension="2" srsName="http://www.opengis.net/def/crs/EPSG/0/31370" uomLabels="m m">188473.52 193390.22</com:pos>
                        </com:point>
                      </com:pointGeometry>
                      <com:positionGeometryMethod>derivedFromObject</com:positionGeometryMethod>
                      <com:positionSpecification>buildingUnit</com:positionSpecification>
                    </com:position>
                    <com:addressStatus>
                      <com:status>retired</com:status>
                      <com:validFrom>2023-11-09T00:00:00</com:validFrom>
                    </com:addressStatus>
                    <com:boxNumber>0101</com:boxNumber>
                    <com:houseNumber>59</com:houseNumber>
                    <com:officiallyAssigned>true</com:officiallyAssigned>
                    <com:hasStreetName>
                      <com:namespace>https://data.vlaanderen.be/id/straatnaam</com:namespace>
                      <com:objectIdentifier>14602</com:objectIdentifier>
                      <com:versionIdentifier>2015-01-19T20:09:59.577</com:versionIdentifier>
                    </com:hasStreetName>
                    <com:hasMunicipality>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente</com:namespace>
                      <com:objectIdentifier>13013</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T17:32:32</com:versionIdentifier>
                    </com:hasMunicipality>
                    <com:hasPostalInfo>
                      <com:namespace>https://data.vlaanderen.be/id/postinfo</com:namespace>
                      <com:objectIdentifier>2230</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T16:37:33</com:versionIdentifier>
                    </com:hasPostalInfo>
                  </tns:address>
                  <tns:address beginLifeSpanVersion="2014-04-04T18:44:32">
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/adres</com:namespace>
                      <com:objectIdentifier>30328681</com:objectIdentifier>
                      <com:versionIdentifier>2024-04-04T18:44:37+02:00</com:versionIdentifier>
                    </com:code>
                    <com:position>
                      <com:pointGeometry>
                        <com:point>
                          <com:pos axisLabels="x y" srsDimension="2" srsName="http://www.opengis.net/def/crs/EPSG/0/31370" uomLabels="m m">165260.87 210822.78</com:pos>
                        </com:point>
                      </com:pointGeometry>
                      <com:positionGeometryMethod>derivedFromObject</com:positionGeometryMethod>
                      <com:positionSpecification>parcel</com:positionSpecification>
                    </com:position>
                    <com:addressStatus>
                      <com:status>current</com:status>
                      <com:validFrom>2014-04-04T18:44:32</com:validFrom>
                    </com:addressStatus>
                    <com:houseNumber>1</com:houseNumber>
                    <com:officiallyAssigned>true</com:officiallyAssigned>
                    <com:hasStreetName>
                      <com:namespace>https://data.vlaanderen.be/id/straatnaam</com:namespace>
                      <com:objectIdentifier>6345</com:objectIdentifier>
                      <com:versionIdentifier>2014-10-22T17:45:58.707</com:versionIdentifier>
                    </com:hasStreetName>
                    <com:hasMunicipality>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente</com:namespace>
                      <com:objectIdentifier>11035</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T17:32:32</com:versionIdentifier>
                    </com:hasMunicipality>
                    <com:hasPostalInfo>
                      <com:namespace>https://data.vlaanderen.be/id/postinfo</com:namespace>
                      <com:objectIdentifier>2520</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T16:37:33</com:versionIdentifier>
                    </com:hasPostalInfo>
                  </tns:address>
                  <tns:address beginLifeSpanVersion="2014-04-04T18:44:32">
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/adres</com:namespace>
                      <com:objectIdentifier>30328682</com:objectIdentifier>
                      <com:versionIdentifier>2024-04-04T18:44:37+02:00</com:versionIdentifier>
                    </com:code>
                    <com:position>
                      <com:pointGeometry>
                        <com:point>
                          <com:pos axisLabels="x y" srsDimension="2" srsName="http://www.opengis.net/def/crs/EPSG/0/31370" uomLabels="m m">165260.87 210822.78</com:pos>
                        </com:point>
                      </com:pointGeometry>
                      <com:positionGeometryMethod>derivedFromObject</com:positionGeometryMethod>
                      <com:positionSpecification>parcel</com:positionSpecification>
                    </com:position>
                    <com:addressStatus>
                      <com:status>current</com:status>
                      <com:validFrom>2014-04-04T18:44:32</com:validFrom>
                    </com:addressStatus>
                    <com:houseNumber>2</com:houseNumber>
                    <com:officiallyAssigned>true</com:officiallyAssigned>
                    <com:hasStreetName>
                      <com:namespace>https://data.vlaanderen.be/id/straatnaam</com:namespace>
                      <com:objectIdentifier>6345</com:objectIdentifier>
                      <com:versionIdentifier>2014-10-22T17:45:58.707</com:versionIdentifier>
                    </com:hasStreetName>
                    <com:hasMunicipality>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente</com:namespace>
                      <com:objectIdentifier>11035</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T17:32:32</com:versionIdentifier>
                    </com:hasMunicipality>
                    <com:hasPostalInfo>
                      <com:namespace>https://data.vlaanderen.be/id/postinfo</com:namespace>
                      <com:objectIdentifier>2520</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T16:37:33</com:versionIdentifier>
                    </com:hasPostalInfo>
                  </tns:address>
                </tns:addressResponseBySource>
                """;

            xml.Should().Be(expected);
        }
    }
}
