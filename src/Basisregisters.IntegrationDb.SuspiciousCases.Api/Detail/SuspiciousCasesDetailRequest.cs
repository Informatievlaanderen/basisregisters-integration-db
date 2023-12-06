namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using MediatR;

    public sealed record SuspiciousCasesDetailRequest(
        FilteringHeader<SuspiciousCasesDetailFilter> FilteringHeader,
        string Type)
        : IRequest<SuspiciousCasesDetailResponse>;

    public class SuspiciousCasesDetailFilter
    {
        public string? NisCode { get; set; }
    }
}
