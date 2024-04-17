namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.IO;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Extensions;
    using Model.Database;
    using Model.Xml;
    using NodaTime;
    using Repositories;

    public class MunicipalityService(
        IClock clock,
        IMunicipalityRepository repo) : BaseRegistryService<Municipality>, IRegistryService
    {
        private static string GetFileName() => $"FlandersMunicipality{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var items = repo.GetAll();

            var serializable = new XmlMunicipalityRoot
            {
                Source = "flanders",
                Timestamp = clock.GetCurrentInstant().ToBelgianDateTimeOffset(),
                Municipalities = items
                    .Select(x => new XmlMunicipality
                    {
                        Code = new XmlCode
                        {
                            Namespace = x.Namespace,
                            ObjectIdentifier = x.NisCode,
                            VersionIdentifier = ShouldUseNewVersion(x)
                                ? x.VersionTimestamp.ToBelgianString()
                                : x.CrabVersionTimestamp!
                        },
                        Name = new XmlName
                        {
                            Language = "nl",
                            Spelling = x.DutchName
                        }
                    })
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }
    }
}
