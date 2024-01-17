namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using MediatR;
    using Schema;

    public sealed record SuspiciousCasesDetailRequest(
        FilteringHeader<SuspiciousCasesDetailFilter> FilteringHeader,
        SuspiciousCasesType Type,
        IPaginationRequest Pagination) : IRequest<SuspiciousCasesDetailResponse>;

    public class SuspiciousCasesDetailFilter
    {
        public string? NisCode { get; set; }
    }
}
