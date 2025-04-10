namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions
{
    public class ResponseOptions
    {
        public required string AddressDetailUrl { get; set; }
        public required string BuildingDetailUrl { get; set; }
        public required string BuildingUnitDetailUrl { get; set; }
        public required string StreetNameDetailUrl { get; set; }
        public required string ParcelDetailUrl { get; set; }
        public required string RoadSegmentDetailUrl { get; set; }
        public required string SuspiciousCasesTypeUrl { get; set; }
        public required string SuspiciousCasesTypeNextUrl { get; set; }
    }
}
