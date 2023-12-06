namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.List
{
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using MediatR;

    public sealed record SuspiciousCasesListRequest(FilteringHeader<SuspiciousCasesListFilter> FilteringHeader): IRequest<SuspiciousCasesListResponse>;

    public class SuspiciousCasesListFilter
    {
        public string? NisCode { get; set; }
    }
}
