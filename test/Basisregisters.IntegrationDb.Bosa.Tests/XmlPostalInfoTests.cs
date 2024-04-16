namespace Basisregisters.IntegrationDb.Bosa.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Model.Database;
    using Model.Xml;
    using Moq;
    using NodaTime;
    using NodaTime.Testing;
    using Repositories;
    using Xunit;

    public class XmlPostalInfoTests
    {
        private PostalInfo[] _given;
        private const string PostalInfoNamespace = "https://data.vlaanderen.be/id/postinfo";

        public XmlPostalInfoTests()
        {
            _given =
            [
                new PostalInfo(PostalInfoNamespace, "1500", new DateTimeOffset(1900, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "HALLE", "2002-08-13T16:37:33"),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2002, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Boekhout", null),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2002, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "GINGELOM", null),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2002, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Jeuk", null),
                new PostalInfo(PostalInfoNamespace, "3890", new DateTimeOffset(2002, 08, 13, 14, 37, 33, TimeSpan.FromHours(2)), "Vorsen", null)
            ];
        }

        [Fact]
        public void GivenPostalInfo_ThenPostalInfoRootIsExpected()
        {
            var expected = new XmlPostalInfoRoot
            {
                Source = "flanders",
                Timestamp = new DateTime(2024, 04, 05, 03, 50, 56),
                PostalInfos =
                [
                    new()
                    {
                        Code = new XmlCode
                        {
                            Namespace = PostalInfoNamespace,
                            ObjectIdentifier = "1500",
                            VersionIdentifier = "2002-08-13T16:37:33"
                        },
                        Name = new XmlName
                        {
                            Language = "nl",
                            Spelling = "HALLE"
                        }
                    },
                    new()
                    {
                        Code = new XmlCode
                        {
                            Namespace = PostalInfoNamespace,
                            ObjectIdentifier = "3890",
                            VersionIdentifier = "2002-08-13T14:37:33+02:00"
                        },
                        Name = new XmlName
                        {
                            Language = "nl",
                            Spelling = "Boekhout/GINGELOM/Jeuk/Vorsen"
                        }
                    }
                ]
            };

            expected.Source.Should().Be("flanders");
            expected.Timestamp.Should().Be(new DateTime(2024, 04, 05, 03, 50, 56));
        }

        [Fact]
        public async Task GivenPostalInfo_ThenSerializesCorrectly()
        {
            var repo = new Mock<IPostalInfoRepository>();
            repo
                .Setup(x => x.GetAll())
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
                         <com:namespace>https://data.vlaanderen.be/id/postinfo/</com:namespace>
                         <com:objectIdentifier>1500</com:objectIdentifier>
                         <com:versionIdentifier>2002-08-13T16:37:33</com:versionIdentifier>
                      </com:code>
                      <com:name>
                         <com:language>nl</com:language>
                         <com:spelling>HALLE</com:spelling>
                      </com:name>
                   </tns:postalInfo>
                   <tns:postalInfo>
                      <com:code>
                         <com:namespace>https://data.vlaanderen.be/id/postinfo/</com:namespace>
                         <com:objectIdentifier>3890</com:objectIdentifier>
                         <com:versionIdentifier>2002-08-13T14:37:33+02:00</com:versionIdentifier>
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
