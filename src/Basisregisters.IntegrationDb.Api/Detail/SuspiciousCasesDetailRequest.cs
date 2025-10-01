namespace Basisregisters.IntegrationDb.Api.Detail
{
    using Abstractions.Detail;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using MediatR;
    using SuspiciousCases;

    public sealed record SuspiciousCasesDetailRequest(
        FilteringHeader<SuspiciousCasesDetailFilter?> FilteringHeader,
        SuspiciousCasesType Type,
        IPaginationRequest Pagination) : IRequest<SuspiciousCasesDetailResponse>;
}
