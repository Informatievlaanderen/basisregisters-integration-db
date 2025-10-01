﻿namespace Basisregisters.IntegrationDb.Api.List
{
    using Abstractions.List;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using MediatR;

    public sealed record SuspiciousCasesListRequest(FilteringHeader<SuspiciousCasesListFilter> FilteringHeader): IRequest<SuspiciousCasesListResponse>;
}
