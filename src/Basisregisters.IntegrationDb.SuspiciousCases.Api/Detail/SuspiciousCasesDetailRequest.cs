namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using Abstractions.Detail;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using MediatR;

    public sealed record SuspiciousCasesDetailRequest(
        FilteringHeader<SuspiciousCasesDetailFilter?> FilteringHeader,
        SuspiciousCasesType Type,
        IPaginationRequest Pagination) : IRequest<SuspiciousCasesDetailResponse>;
}
