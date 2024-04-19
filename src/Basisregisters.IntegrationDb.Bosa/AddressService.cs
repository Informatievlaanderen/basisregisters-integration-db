namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.GrAr.Common;
    using Model.Database;
    using Model.Xml;
    using NodaTime;
    using Repositories;

    public class AddressService(
        IClock clock,
        IAddressRepository addressRepo,
        IStreetNameRepository streetNameRepo,
        IMunicipalityRepository municipalityRepo,
        IPostalInfoRepository postalInfoRepo) : BaseRegistryService<Address>, IRegistryService
    {
        private static string GetFileName() => $"FlandersAddress{DateTimeOffset.Now:yyyyMMdd}L72";

        public string GetXmlFileName() => $"{GetFileName()}.xml";
        public string GetZipFileName() => $"{GetFileName()}.zip";

        public void CreateXml(Stream outputStream)
        {
            var addresses = addressRepo.GetFlemish();
            var postalCodes = postalInfoRepo.GetFlemish();
            var municipalities = municipalityRepo.GetFlemish();
            var streetNames = streetNameRepo.GetFlemish();

            var serializable = new XmlAddressRoot
            {
                Source = "flanders",
                Timestamp = clock.GetCurrentInstant().ToBelgianDateTimeOffset(),
                Addresses = addresses
                    .Select(address =>
                    {
                        var beginLifeSpanVersion = GetBeginLifeSpanVersion(address);
                        var endLifeSpanVersion = GetEndLifeSpanVersion(address);

                        return new XmlAddress
                        {
                            BeginLifeSpanVersion = beginLifeSpanVersion,
                            EndLifeSpanVersion = GetEndLifeSpanVersion(address),
                            Code = new XmlCode
                            {
                                Namespace = address.Namespace,
                                ObjectIdentifier = address.AddressPersistentLocalId.ToString(),
                                VersionIdentifier = GetVersionAsString(address)
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
                                            Value = $"{address.X.ToString("0.0", CultureInfo.InvariantCulture)} {address.Y.ToString("0.0", CultureInfo.InvariantCulture)}"
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
                                ValidFrom = beginLifeSpanVersion,
                                ValidTo = endLifeSpanVersion
                            },
                            // IsAssignedByMunicipality = new XmlCode
                            // {
                            //     Namespace = address.MunicipalityNamespace,
                            //     ObjectIdentifier = address.NisCode,
                            //     VersionIdentifier = GetVersionAsString(
                            //         address.MunicipalityCrabVersionTimestamp,
                            //         address.MunicipalityVersionTimestamp)
                            // }
                        };
                    })
                    .ToArray()
            };

            RegistryXmlSerializer.Serialize(serializable, outputStream);
        }

        private static string? GetBeginLifeSpanVersion(Address address)
            => address.CrabCreatedOn ?? GetVersionAsString(address.CreatedOn);

        private static string? GetEndLifeSpanVersion(Address address)
        {
            return address.Status is AddressStatus.Rejected or AddressStatus.Retired
                ? GetVersionAsString(address)
                : null;
        }

        private string GetPositionSpecification(Address address)
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

        private string GetPositionGeometryMethod(Address address)
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
