namespace Basisregisters.IntegrationDb.Api.SuspiciousCase.List
{
    using Abstractions.SuspiciousCase.List;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using MediatR;

    public sealed record SuspiciousCasesListRequest(FilteringHeader<SuspiciousCasesListFilter> FilteringHeader): IRequest<SuspiciousCasesListResponse>;
}
