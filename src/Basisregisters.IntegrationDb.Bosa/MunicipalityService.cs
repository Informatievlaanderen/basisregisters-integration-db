namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Model.Database;
    using Model.Xml;
    using NodaTime;
    using Repositories;

    public class MunicipalityService(
        IClock clock,
        IMunicipalityRepository repo) : BaseRegistryService, IRegistryService
    {
        private static string GetFileName() => $"FlandersMunicipality{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var items = repo.GetFlemish().Where(x => x.Status != MunicipalityStatus.Retired);

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
                            VersionIdentifier = GetVersionAsString(x.VersionTimestamp)
                        },
                        Name = GetNames(x).Select(name => new XmlName
                        {
                            Language = name.Key,
                            Spelling = name.Value
                        }).ToArray()
                    })
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }

        private static Dictionary<string, string> GetNames(Municipality municipality)
        {
            var namesByLanguage = new Dictionary<string, string> { { "nl", municipality.DutchName } };

            if(!string.IsNullOrWhiteSpace(municipality.FrenchName))
                namesByLanguage.Add("fr", municipality.FrenchName);

            if(!string.IsNullOrWhiteSpace(municipality.GermanName))
                namesByLanguage.Add("de", municipality.GermanName);

            if(!string.IsNullOrWhiteSpace(municipality.EnglishName))
                namesByLanguage.Add("en", municipality.EnglishName);

            return namesByLanguage;
        }
    }
}
