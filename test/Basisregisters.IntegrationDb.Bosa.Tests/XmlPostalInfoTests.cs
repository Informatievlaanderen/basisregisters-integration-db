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

    public class XmlPostalInfoTests
    {
        private readonly PostalInfo[] _given;
        private const string PostalInfoNamespace = "https://data.vlaanderen.be/id/postinfo";

        public XmlPostalInfoTests()
        {
            _given =
            [
                new PostalInfo(PostalInfoNamespace, "1500", new DateTimeOffset(1940, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "HALLE"),
                new PostalInfo(PostalInfoNamespace, "1600", new DateTimeOffset(2002, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "FAKE"),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2024, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Jeuk"),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2024, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "GINGELOM"),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2024, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Boekhout"),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2024, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Vorsen")
            ];
        }

        [Fact]
        public async Task GivenPostalInfo_ThenSerializesCorrectly()
        {
            var repo = new Mock<IPostalInfoRepository>();
            repo
                .Setup(x => x.GetFlemish())
                .Returns(_given);

            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var service = new PostalInfoService(clock, repo.Object);

            await using var outputStream = new MemoryStream();
            service.CreateXml(outputStream);

            var xml = Encoding.UTF8.GetString(outputStream.ToArray()).Trim();

            // serialize to xml

            var expected =
                """
                <?xml version="1.0" encoding="utf-8"?>
                <tns:postalInfoResponseBySource xmlns:com="http://fsb.belgium.be/data/common" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tns="http://fsb.belgium.be/mappingservices/FullDownload/v1_00">
                  <tns:source>flanders</tns:source>
                  <tns:timestamp>1970-01-01T01:00:00+01:00</tns:timestamp>
                  <tns:postalInfo>
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/postinfo</com:namespace>
                      <com:objectIdentifier>1500</com:objectIdentifier>
                      <com:versionIdentifier>1940-08-13T14:37:33+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>HALLE</com:spelling>
                    </com:name>
                  </tns:postalInfo>
                  <tns:postalInfo>
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/postinfo</com:namespace>
                      <com:objectIdentifier>1600</com:objectIdentifier>
                      <com:versionIdentifier>2002-08-13T14:37:33+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>FAKE</com:spelling>
                    </com:name>
                  </tns:postalInfo>
                  <tns:postalInfo>
                    <com:code>
                      <com:namespace>https://data.vlaanderen.be/id/postinfo</com:namespace>
                      <com:objectIdentifier>3890</com:objectIdentifier>
                      <com:versionIdentifier>2024-08-13T14:37:33+02:00</com:versionIdentifier>
                    </com:code>
                    <com:name>
                      <com:language>nl</com:language>
                      <com:spelling>Boekhout/GINGELOM/Jeuk/Vorsen</com:spelling>
                    </com:name>
                  </tns:postalInfo>
                </tns:postalInfoResponseBySource>
                """;

            xml.Should().Be(expected);
        }
    }
}
