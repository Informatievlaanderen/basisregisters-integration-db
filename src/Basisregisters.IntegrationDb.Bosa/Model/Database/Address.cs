namespace Basisregisters.IntegrationDb.Bosa.Model.Database
{
    using System;

    public enum GeometryMethod
    {
        AppointedByAdministrator = 1,
        DerivedFromObject = 2,
        Interpolated = 3
    }

    public enum GeometrySpecification
    {
        Municipality = 1,
        Street = 2,
        Parcel = 3,
        Lot = 4,
        Stand = 5,
        Berth = 6,
        Building = 7,
        BuildingUnit = 8,
        Entry = 9,
        RoadSegment = 11,
    }

    public enum AddressStatus
    {
        Proposed = 1,
        Current = 2,
        Retired = 3,
        Rejected = 4
    }

    public class Address
    {
        public string Namespace { get; init; }
        public int AddressPersistentLocalId { get; init; }
        public int StreetNamePersistentLocalId { get; init; }
        public string PostalCode { get; init; }

        public DateTimeOffset VersionTimestamp { get; init; }
        public DateTimeOffset CreatedOn { get; init; }

        public double X { get; init; }
        public double Y { get; init; }
        public int SrId { get; init; }

        public GeometryMethod PositionGeometryMethod { get; init; }
        public GeometrySpecification PositionSpecification { get; init; }

        public AddressStatus Status { get; init; }

        public string HouseNumber { get; init; }
        public string? BoxNumber { get; init; }
        public bool? OfficiallyAssigned { get; init; }

        // Needed for Dapper
        protected Address()
        { }

        public Address(
            string @namespace,
            int addressPersistentLocalId,
            int streetNamePersistentLocalId,
            string postalCode,
            DateTimeOffset versionTimestamp,
            DateTimeOffset createdOn,
            double x,
            double y,
            int srId,
            GeometryMethod positionGeometryMethod,
            GeometrySpecification positionSpecification,
            AddressStatus status,
            string houseNumber,
            string? boxNumber,
            bool? officiallyAssigned)
        {
            Namespace = @namespace;
            AddressPersistentLocalId = addressPersistentLocalId;
            StreetNamePersistentLocalId = streetNamePersistentLocalId;
            PostalCode = postalCode;
            VersionTimestamp = versionTimestamp;
            CreatedOn = createdOn;
            X = x;
            Y = y;
            SrId = srId;
            PositionGeometryMethod = positionGeometryMethod;
            PositionSpecification = positionSpecification;
            Status = status;
            HouseNumber = houseNumber;
            BoxNumber = boxNumber;
            OfficiallyAssigned = officiallyAssigned;
        }
    }
}
