namespace Basisregisters.IntegrationDb.Api.SuspiciousCase.Detail
{
    using Abstractions.SuspiciousCase.Detail;
    using Basisregisters.IntegrationDb.SuspiciousCases;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using MediatR;

    public sealed record SuspiciousCasesDetailRequest(
        FilteringHeader<SuspiciousCasesDetailFilter?> FilteringHeader,
        SuspiciousCasesType Type,
        IPaginationRequest Pagination) : IRequest<SuspiciousCasesDetailResponse>;
}
