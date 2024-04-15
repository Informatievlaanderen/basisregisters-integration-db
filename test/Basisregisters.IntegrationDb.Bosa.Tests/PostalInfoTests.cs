namespace Basisregisters.IntegrationDb.Bosa.Tests
{
    using AutoFixture;
    using Model;
    using Xunit;

    public class PostalInfoTests
    {
        [Fact]
        public void GivenPostalInfo_ThenSerializesCorrectly()
        {
            var given = new PostalInfo[];

            //convert to xml entity


            // serialize to xml

            var expected = @"
<?xml version=""1.0"" encoding=""UTF-8""?>
<tns:postalInfoResponseBySource xmlns:tns=""http://fsb.belgium.be/mappingservices/FullDownload/v1_00""
                                xmlns:com=""http://fsb.belgium.be/data/common""
                                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
   <tns:source>flanders</tns:source>
   <tns:timestamp>2024-04-05T03:50:56</tns:timestamp>
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
         <com:versionIdentifier>2002-08-13T16:37:33</com:versionIdentifier>
      </com:code>
      <com:name>
         <com:language>nl</com:language>
         <com:spelling>Boekhout/GINGELOM/Jeuk/Kortijs/Montenaken/Niel-Bij-Sint-Truiden/Vorsen</com:spelling>
      </com:name>
   </tns:postalInfo>
</tns:postalInfoResponseBySource>
";
        }
    }
}
