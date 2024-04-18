namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Extensions;
    using Model.Database;
    using Model.Xml;
    using NodaTime;
    using Repositories;

    public class StreetNameService(
        IClock clock,
        IStreetNameRepository repo) : BaseRegistryService<StreetName>, IRegistryService
    {
        private static string GetFileName() => $"FlandersStreetName{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var items = repo.GetFlemish();

            var serializable = new XmlStreetNameRoot
            {
                Source = "flanders",
                Timestamp = clock.GetCurrentInstant().ToBelgianDateTimeOffset(),
                StreetNames = items
                    .Select(streetName =>
                    {
                        return new XmlStreetName
                        {
                            BeginLifeSpanVersion = streetName.CreatedOn,
                            EndLifeSpanVersion = GetEndLifeSpanVersion(streetName),
                            Code = new XmlCode
                            {
                                Namespace = streetName.Namespace,
                                ObjectIdentifier = streetName.StreetNamePersistentLocalId,
                                VersionIdentifier = ShouldUseNewVersion(streetName)
                                    ? streetName.VersionTimestamp.ToBelgianString()
                                    : streetName.CrabVersionTimestamp!
                            },
                            Names = GetNames(streetName).ToArray(),
                            Status = new XmlStreetNameStatus
                            {
                                Status = 
                            }
                        };
                    })
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }

        private static string? GetEndLifeSpanVersion(StreetName streetName)
        {
            return streetName.Status is StreetNameStatus.Rejected or StreetNameStatus.Retired
                ? ShouldUseNewVersion(streetName)
                    ? streetName.VersionTimestamp.ToBelgianString()
                    : streetName.CrabVersionTimestamp!
                : null;
        }

        private static IEnumerable<XmlName> GetNames(StreetName streetName)
        {
            if (!string.IsNullOrEmpty(streetName.NameDutch))
            {
                yield return new XmlName
                {
                    Language = "nl",
                    Spelling = streetName.NameDutch
                };
            }

            if (!string.IsNullOrEmpty(streetName.NameFrench))
            {
                yield return new XmlName
                {
                    Language = "fr",
                    Spelling = streetName.NameFrench
                };
            }

            if (!string.IsNullOrEmpty(streetName.NameGerman))
            {
                yield return new XmlName
                {
                    Language = "de",
                    Spelling = streetName.NameGerman
                };
            }
        }
    }
}
