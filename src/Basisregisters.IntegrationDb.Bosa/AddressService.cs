namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.SpatialTools.GeometryCoordinates;
    using Microsoft.Extensions.Logging;
    using Model.Database;
    using Model.Xml;
    using NodaTime;
    using Repositories;

    public class AddressService(
        IClock clock,
        IAddressRepository addressRepo,
        IStreetNameRepository streetNameRepo,
        IMunicipalityRepository municipalityRepo,
        IPostalInfoRepository postalInfoRepo,
        ILoggerFactory loggerFactory) : BaseRegistryService, IRegistryService
    {
        private static string GetFileName() => $"FlandersAddress{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var logger = loggerFactory.CreateLogger<AddressService>();
            logger.LogInformation("Retrieving address data...");

            var postalCodes = postalInfoRepo.GetFlemish()
                .GroupBy(x => x.PostalCode)
                .ToDictionary(x => x.Key, x => x.First());
            var municipalities = municipalityRepo.GetFlemish().ToDictionary(
                x => x.NisCode, x => x);
            var streetNames = streetNameRepo.GetFlemishIdentifiers().ToDictionary(
                x => x.StreetNamePersistentLocalId, x => x);
            var addresses = addressRepo.GetFlemish();

            logger.LogInformation("Processing address data...");
            var xmlAddresses = new ConcurrentDictionary<int, XmlAddress>();
            Parallel.ForEach(addresses, address =>
            {
                if (!streetNames.TryGetValue(address.StreetNamePersistentLocalId, out var streetName))
                {
                    logger.LogInformation($"No street name found for {address.StreetNamePersistentLocalId}");
                    return;
                }

                if (streetName.MunicipalityStatus == MunicipalityStatus.Retired)
                {
                    return;
                }

                if (!postalCodes.TryGetValue(address.PostalCode, out var postalInfo))
                {
                    logger.LogInformation($"No postal code found for {address.PostalCode}");
                    return;
                }

                var municipality = municipalities[streetName.NisCode];

                var beginLifeSpanVersion = GetZuluVersionAsString(address.CreatedOn);
                var endLifeSpanVersion = GetEndLifeSpanVersion(address);

                var xmlAddress = new XmlAddress
                {
                    BeginLifeSpanVersion = beginLifeSpanVersion,
                    EndLifeSpanVersion = endLifeSpanVersion,
                    Code = new XmlCode
                    {
                        Namespace = FormatNamespace(address.Namespace),
                        ObjectIdentifier = address.AddressPersistentLocalId.ToString(),
                        VersionIdentifier = GetVersionAsString(address.VersionTimestamp)
                    },
                    Position = new XmlAddressPosition
                    {
                        PointGeometry = new XmlPointGeometry
                        {
                            Point = new XmlPoint
                            {
                                Pos = new XmlPointPos
                                {
                                    AxisLabels = "x y",
                                    SrsDimension = 2,
                                    SrsName = $"http://www.opengis.net/def/crs/EPSG/0/{address.SrId}",
                                    UomLabels = "m m",
                                    Value =
                                        $"{address.X.ToPointGeometryCoordinateValueFormat()} {address.Y.ToPointGeometryCoordinateValueFormat()}"
                                }
                            }
                        },
                        PositionGeometryMethod = GetPositionGeometryMethod(address),
                        PositionSpecification = GetPositionSpecification(address)
                    },
                    BoxNumber = address.BoxNumber,
                    HouseNumber = address.HouseNumber,
                    OfficiallyAssigned = address.OfficiallyAssigned ?? false,
                    Status = new XmlAddressStatus
                    {
                        Status = GetStatus(address),
                        ValidFrom = beginLifeSpanVersion
                    },
                    HasStreetName = new XmlCode
                    {
                        Namespace = FormatNamespace(streetName.Namespace),
                        ObjectIdentifier = streetName.StreetNamePersistentLocalId.ToString(),
                        VersionIdentifier = GetVersionAsString(streetName.VersionTimestamp)
                    },
                    HasMunicipality = new XmlCode
                    {
                        Namespace = FormatNamespace(municipality.Namespace),
                        ObjectIdentifier = municipality.NisCode,
                        VersionIdentifier = GetVersionAsString(municipality.VersionTimestamp)
                    },
                    HasPostalInfo = new XmlCode
                    {
                        Namespace = FormatNamespace(postalInfo.Namespace),
                        ObjectIdentifier = postalInfo.PostalCode,
                        VersionIdentifier = GetVersionAsString(postalInfo.VersionTimestamp)
                    }
                };

                var isAdded = xmlAddresses.TryAdd(address.AddressPersistentLocalId, xmlAddress);

                if (!isAdded)
                    logger.LogError($"Address {address.AddressPersistentLocalId} already exists");
            });

            logger.LogInformation("Serializing address data...");
            var serializable = new XmlAddressRoot
            {
                Source = "flanders",
                Timestamp = clock.GetCurrentInstant().ToBelgianDateTimeOffset(),
                Addresses = xmlAddresses
                    .OrderBy(x => x.Key)
                    .Select(x => x.Value)
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }

        private static string? GetEndLifeSpanVersion(Address address)
        {
            return address.Status is AddressStatus.Rejected or AddressStatus.Retired
                ? GetZuluVersionAsString(address.VersionTimestamp)
                : null;
        }

        private static string GetPositionSpecification(Address address)
        {
            return address.PositionSpecification switch
            {
                GeometrySpecification.Municipality => "municipality",
                GeometrySpecification.Street => "street",
                GeometrySpecification.Parcel => "parcel",
                GeometrySpecification.Lot => "plot",
                GeometrySpecification.Stand => "stand",
                GeometrySpecification.Berth => "mooringPlace",
                GeometrySpecification.Building => "building",
                GeometrySpecification.BuildingUnit => "buildingUnit",
                GeometrySpecification.Entry => "entrance",
                GeometrySpecification.RoadSegment => "street",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetPositionGeometryMethod(Address address)
        {
            return address.PositionGeometryMethod switch
            {
                GeometryMethod.AppointedByAdministrator => "assignedByAdministrator",
                GeometryMethod.DerivedFromObject => "derivedFromObject",
                GeometryMethod.Interpolated => "derivedFromObject",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetStatus(Address address)
        {
            return address.Status switch
            {
                AddressStatus.Proposed => "proposed",
                AddressStatus.Current => "current",
                AddressStatus.Retired => "retired",
                AddressStatus.Rejected => "retired",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
