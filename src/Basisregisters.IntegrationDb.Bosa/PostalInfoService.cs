namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.IO;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Model.Xml;
    using NodaTime;
    using Repositories;

    public class PostalInfoService(
        IClock clock,
        IPostalInfoRepository repo) : BaseRegistryService, IRegistryService
    {
        private static string GetFileName() => $"FlandersPostalInfo{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var items = repo.GetFlemish();

            var serializable = new XmlPostalInfoRoot
            {
                Source = "flanders",
                Timestamp = clock.GetCurrentInstant().ToBelgianDateTimeOffset(),
                PostalInfos = items
                    .GroupBy(x => x.PostalCode)
                    .Select(x =>
                    {
                        var postalInfo = x.First();
                        var names = string.Join('/', x.Select(y => y.DutchName).OrderBy(y => y));

                        return new XmlPostalInfo
                        {
                            Code = new XmlCode
                            {
                                Namespace = FormatNamespace(postalInfo.Namespace),
                                ObjectIdentifier = postalInfo.PostalCode,
                                VersionIdentifier = GetVersionAsString(postalInfo.VersionTimestamp),
                            },
                            Name = new XmlName
                            {
                                Language = "nl",
                                Spelling = names
                            }
                        };
                    })
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }
    }
}
