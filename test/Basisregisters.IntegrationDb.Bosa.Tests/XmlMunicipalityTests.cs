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

    public class XmlMunicipalityTests
    {
        private readonly Municipality[] _given;
        private const string MunicipalityNamespace = "https://data.vlaanderen.be/id/gemeente";

        public XmlMunicipalityTests()
        {
            _given =
            [
                new Municipality(MunicipalityNamespace, "23027", new DateTimeOffset(1900, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "HALLE", "2002-08-13T16:37:33"),
                new Municipality(MunicipalityNamespace, "93027", new DateTimeOffset(2002, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "FAKE", null),
                new Municipality(MunicipalityNamespace, "23028", new DateTimeOffset(2024, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Boekhout", "2002-08-13T16:37:33")
            ];
        }

        [Fact]
        public async Task GivenMunicipality_ThenSerializesCorrectly()
        {
            var repo = new Mock<IMunicipalityRepository>();
            repo
                .Setup(x => x.GetFlemish())
                .Returns(_given);

            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var service = new MunicipalityService(clock, repo.Object);

            await using var outputStream = new MemoryStream();
            service.CreateXml(outputStream);

            var xml = Encoding.UTF8.GetString(outputStream.ToArray()).Trim();

            // serialize to xml

            var expected =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <tns:municipalityResponseBySource xmlns:com="http://fsb.belgium.be/data/common" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tns="http://fsb.belgium.be/mappingservices/FullDownload/v1_00">
                  <tns:source>flanders</tns:source>
                  <tns:timestamp>1970-01-01T01:00:00+01:00</tns:timestamp>
                  <tns:municipality>
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente</com:namespace>
                      <com:objectIdentifier>23027</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T16:37:33</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>HALLE</com:spelling>
                    </com:name>
                  </tns:municipality>
                  <tns:municipality>
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente</com:namespace>
                      <com:objectIdentifier>93027</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T14:37:33+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>FAKE</com:spelling>
                    </com:name>
                  </tns:municipality>
                  <tns:municipality>
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/gemeente</com:namespace>
                      <com:objectIdentifier>23028</com:objectIdentifier>
                      <com:versionIdentifier>2024-08-13T14:37:33+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>Boekhout</com:spelling>
                    </com:name>
                  </tns:municipality>
                </tns:municipalityResponseBySource>
                """;

            xml.Should().Be(expected);
        }
    }
}
