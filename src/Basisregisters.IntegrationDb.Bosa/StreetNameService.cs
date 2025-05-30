namespace Basisregisters.IntegrationDb.Bosa
{
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Model.Database;
    using Model.Xml;
    using NodaTime;
    using Repositories;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StreetNameService(
        IClock clock,
        IStreetNameRepository repo) : BaseRegistryService, IRegistryService
    {
        private static string GetFileName() => $"FlandersStreetName{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var items = repo.GetFlemish().Where(x => x.MunicipalityStatus != MunicipalityStatus.Retired);

            var serializable = new XmlStreetNameRoot
            {
                Source = "flanders",
                Timestamp = clock.GetCurrentInstant().ToBelgianDateTimeOffset(),
                StreetNames = items
                    .Select(streetName =>
                    {
                        var beginLifeSpanVersion = GetBeginLifeSpanVersion(streetName);
                        var endLifeSpanVersion = GetEndLifeSpanVersion(streetName);

                        return new XmlStreetName
                        {
                            BeginLifeSpanVersion = beginLifeSpanVersion,
                            EndLifeSpanVersion = endLifeSpanVersion,
                            Code = new XmlCode
                            {
                                Namespace = FormatNamespace(streetName.Namespace),
                                ObjectIdentifier = streetName.StreetNamePersistentLocalId.ToString(),
                                VersionIdentifier = GetVersionAsString(streetName.VersionTimestamp)
                            },
                            Names = GetNames(streetName).ToArray(),
                            Status = new XmlStreetNameStatus
                            {
                                Status = GetStatus(streetName),
                                ValidFrom = beginLifeSpanVersion,
                                ValidTo = endLifeSpanVersion
                            },
                            Type = "streetname",
                            HomonymAddition = GetHomonymAddition(streetName),
                            IsAssignedByMunicipality = new XmlCode
                            {
                                Namespace = FormatNamespace(streetName.MunicipalityNamespace),
                                ObjectIdentifier = streetName.NisCode,
                                VersionIdentifier = GetVersionAsString(streetName.MunicipalityVersionTimestamp)
                            }
                        };
                    })
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }

        private static string? GetHomonymAddition(StreetName streetName)
        {
            if (!string.IsNullOrWhiteSpace(streetName.HomonymAdditionDutch))
                return streetName.HomonymAdditionDutch;

            if (!string.IsNullOrWhiteSpace(streetName.HomonymAdditionFrench))
                return streetName.HomonymAdditionFrench;

            if (!string.IsNullOrWhiteSpace(streetName.HomonymAdditionGerman))
                return streetName.HomonymAdditionGerman;

            return null;
        }

        private static string GetBeginLifeSpanVersion(StreetName streetName)
            => GetZuluVersionAsString(streetName.CreatedOn);

        private static string? GetEndLifeSpanVersion(StreetName streetName)
        {
            return streetName.Status is StreetNameStatus.Rejected or StreetNameStatus.Retired
                ? GetZuluVersionAsString(streetName.VersionTimestamp)
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

        private static string GetStatus(StreetName streetName)
        {
            return streetName.Status switch
            {
                StreetNameStatus.Proposed => "proposed",
                StreetNameStatus.Current => "current",
                StreetNameStatus.Retired => "retired",
                StreetNameStatus.Rejected => "retired",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
