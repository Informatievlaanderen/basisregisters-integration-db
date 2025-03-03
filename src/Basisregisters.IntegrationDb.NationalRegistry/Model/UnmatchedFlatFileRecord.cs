namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using NetTopologySuite.Geometries;
    using Repositories;

    public sealed class UnmatchedFlatFileRecord
    {
        public required FlatFileRecord Record { get; init; }
        public required Point Position { get; init; }
        public StreetName? StreetName { get; init; }
        public Address? HouseNumberAddress { get; init; }
    }
}
