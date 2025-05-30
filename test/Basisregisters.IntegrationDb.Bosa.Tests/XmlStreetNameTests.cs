namespace Basisregisters.IntegrationDb.Bosa.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Model.Database;
    using Moq;
    using NodaTime;
    using NodaTime.Testing;
    using Repositories;
    using Xunit;

    public class XmlStreetNameTests
    {
        private readonly StreetName[] _given;
        private const string StreetNameNamespace = "https://data.vlaanderen.be/id/straatnaam";
        private const string MunicipalityNamespace = "https://data.vlaanderen.be/id/gemeente";

        public XmlStreetNameTests()
        {
            _given =
            [
                new StreetName(
                    StreetNameNamespace,
                    27114,
                    StreetNameStatus.Retired,
                    new DateTimeOffset(2015, 08, 31, 17, 19, 02, 397, TimeSpan.FromHours(2)),
                    new DateTimeOffset(2002, 08, 31, 17, 19, 02, 397, TimeSpan.FromHours(2)),
                    "NIjverheidslaan",
                    nameFrench:null,
                    nameGerman:null,
                    homonymAdditionDutch:null,
                    homonymAdditionFrench:null,
                    homonymAdditionGerman:null,
                    "23044",
                    MunicipalityNamespace,
                    new DateTimeOffset(2024, 08, 13, 17, 32, 32, TimeSpan.FromHours(2)),
                    MunicipalityStatus.Proposed),
                new StreetName(
                    StreetNameNamespace,
                    30376,
                    StreetNameStatus.Current,
                    new DateTimeOffset(2016, 10, 03, 08, 44, 22, 810, TimeSpan.FromHours(2)),
                    new DateTimeOffset(2016, 10, 03, 08, 46, 40, TimeSpan.FromHours(2)),
                    "Anderlechtstraat",
                    "Rue d'Anderlecht",
                    nameGerman:null,
                    homonymAdditionDutch:null,
                    homonymAdditionFrench:null,
                    homonymAdditionGerman:null,
                    "23098",
                    MunicipalityNamespace,
                    new DateTimeOffset(2002, 08, 13, 17, 32, 32, TimeSpan.FromHours(2)),
                    MunicipalityStatus.Current
                ),
                new StreetName(
                    StreetNameNamespace,
                    228584,
                    StreetNameStatus.Proposed,
                    new DateTimeOffset(2024, 01, 31, 14, 16, 08, TimeSpan.FromHours(1)),
                    new DateTimeOffset(2024, 01, 31, 14, 16, 03, TimeSpan.FromHours(1)),
                    "Hassyweg",
                    nameFrench:null,
                    nameGerman:null,
                    homonymAdditionDutch:"HO",
                    homonymAdditionFrench:null,
                    homonymAdditionGerman:null,
                    "24059",
                    MunicipalityNamespace,
                    new DateTimeOffset(2010, 08, 13, 17, 32, 32, TimeSpan.FromHours(2)),
                    MunicipalityStatus.Proposed),
                new StreetName(
                    StreetNameNamespace,
                    228585,
                    StreetNameStatus.Proposed,
                    new DateTimeOffset(2024, 01, 31, 14, 16, 08, TimeSpan.FromHours(1)),
                    new DateTimeOffset(2024, 01, 31, 14, 16, 03, TimeSpan.FromHours(1)),
                    "Hassyweg",
                    nameFrench:null,
                    nameGerman:null,
                    homonymAdditionDutch:null,
                    homonymAdditionFrench:null,
                    homonymAdditionGerman:null,
                    "24059",
                    MunicipalityNamespace,
                    new DateTimeOffset(2010, 08, 13, 17, 32, 32, TimeSpan.FromHours(2)),
                    MunicipalityStatus.Retired),
            ];
        }

        [Fact]
        public async Task GivenStreetName_ThenSerializesCorrectly()
        {
            var repo = new Mock<IStreetNameRepository>();
            repo
                .Setup(x => x.GetFlemish())
                .Returns(_given);

            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var service = new StreetNameService(clock, repo.Object);

            await using var outputStream = new MemoryStream();
            service.CreateXml(outputStream);

            var xml = Encoding.UTF8.GetString(outputStream.ToArray()).Trim();

            // serialize to xml

            var expected =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <tns:streetNameResponseBySource xmlns:com="http://fsb.belgium.be/data/common" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tns="http://fsb.belgium.be/mappingservices/FullDownload/v1_00">
                  <tns:source>flanders</tns:source>
                  <tns:timestamp>1970-01-01T01:00:00+01:00</tns:timestamp>
                  <tns:streetName beginLifeSpanVersion="2002-08-31T15:19:02Z" endLifeSpanVersion="2015-08-31T15:19:02Z">
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/straatnaam/</com:namespace>
                      <com:objectIdentifier>27114</com:objectIdentifier>
                      <com:versionIdentifier>2015-08-31T17:19:02+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>NIjverheidslaan</com:spelling>
                    </com:name>
                    <com:streetNameStatus>
                      <com:status>retired</com:status>
                      <com:validFrom>2002-08-31T15:19:02Z</com:validFrom>
                      <com:validTo>2015-08-31T15:19:02Z</com:validTo>
                    </com:streetNameStatus>
                    <com:type>streetname</com:type>
                    <com:isAssignedByMunicipality>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente/</com:namespace>
                      <com:objectIdentifier>23044</com:objectIdentifier>
                      <com:versionIdentifier>2024-08-13T17:32:32+02:00</com:versionIdentifier>
                    </com:isAssignedByMunicipality>
                  </tns:streetName>
                  <tns:streetName beginLifeSpanVersion="2016-10-03T06:46:40Z">
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/straatnaam/</com:namespace>
                      <com:objectIdentifier>30376</com:objectIdentifier>
                      <com:versionIdentifier>2016-10-03T08:44:22+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>Anderlechtstraat</com:spelling>
                    </com:name>
                    <com:name>
                      <com:language>fr</com:language>
                      <com:spelling>Rue d'Anderlecht</com:spelling>
                    </com:name>
                    <com:streetNameStatus>
                      <com:status>current</com:status>
                      <com:validFrom>2016-10-03T06:46:40Z</com:validFrom>
                    </com:streetNameStatus>
                    <com:type>streetname</com:type>
                    <com:isAssignedByMunicipality>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente/</com:namespace>
                      <com:objectIdentifier>23098</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T17:32:32+02:00</com:versionIdentifier>
                    </com:isAssignedByMunicipality>
                  </tns:streetName>
                  <tns:streetName beginLifeSpanVersion="2024-01-31T13:16:03Z">
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/straatnaam/</com:namespace>
                      <com:objectIdentifier>228584</com:objectIdentifier>
                      <com:versionIdentifier>2024-01-31T14:16:08+01:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>Hassyweg</com:spelling>
                    </com:name>
                    <com:streetNameStatus>
                      <com:status>proposed</com:status>
                      <com:validFrom>2024-01-31T13:16:03Z</com:validFrom>
                    </com:streetNameStatus>
                    <com:type>streetname</com:type>
                    <com:homonymAddition>HO</com:homonymAddition>
                    <com:isAssignedByMunicipality>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente/</com:namespace>
                      <com:objectIdentifier>24059</com:objectIdentifier>
                      <com:versionIdentifier>2010-08-13T17:32:32+02:00</com:versionIdentifier>
                    </com:isAssignedByMunicipality>
                  </tns:streetName>
                </tns:streetNameResponseBySource>
                """;

            xml.Should().Be(expected);
        }
    }
}
