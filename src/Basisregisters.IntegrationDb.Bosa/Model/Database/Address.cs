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
        Unknown = 0,
        Proposed = 1,
        Current = 2,
        Retired = 3,
        Rejected = 4
    }

    public class Address : IHasVersionTimestamps
    {
        public string Namespace { get; init; }
        public string AddressPersistentLocalId { get; init; }

        public DateTimeOffset VersionTimestamp { get; init; }
        public string? CrabVersionTimestamp { get; init; }
        public string? CrabCreatedOn { get; init; }
        public DateTimeOffset CreatedOn { get; init; }

        public double X { get; init; }
        public double Y { get; init; }
        public int SrId { get; init; }

        public GeometryMethod PositionGeometryMethod { get; init; }
        public GeometrySpecification PositionSpecification { get; init; }

        public AddressStatus AddressStatus { get; init; }

        public string HouseNumber { get; init; }
        public string? BoxNumber { get; init; }
        public bool? OfficiallyAssigned { get; init; }

        // Needed for Dapper
        protected Address()
        { }

        public Address(
            string @namespace,
            string addressPersistentLocalId,
            DateTimeOffset versionTimestamp,
            string? crabVersionTimestamp,
            string? crabCreatedOn,
            DateTimeOffset createdOn,
            double x,
            double y,
            int srId,
            GeometryMethod positionGeometryMethod,
            GeometrySpecification positionSpecification,
            AddressStatus addressStatus,
            string houseNumber,
            string? boxNumber,
            bool? officiallyAssigned)
        {
            Namespace = @namespace;
            AddressPersistentLocalId = addressPersistentLocalId;
            VersionTimestamp = versionTimestamp;
            CrabVersionTimestamp = crabVersionTimestamp;
            CrabCreatedOn = crabCreatedOn;
            CreatedOn = createdOn;
            X = x;
            Y = y;
            SrId = srId;
            PositionGeometryMethod = positionGeometryMethod;
            PositionSpecification = positionSpecification;
            AddressStatus = addressStatus;
            HouseNumber = houseNumber;
            BoxNumber = boxNumber;
            OfficiallyAssigned = officiallyAssigned;
        }
    }
}
