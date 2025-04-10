namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions;
    using Abstractions.Detail;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Utilities;
    using MediatR;
    using Microsoft.Extensions.Options;

    public sealed class SuspiciousCasesDetailRequestHandler : IRequestHandler<SuspiciousCasesDetailRequest, SuspiciousCasesDetailResponse>
    {
        private readonly SuspiciousCasesContext _context;
        private readonly ResponseOptions _responseOptions;

        public SuspiciousCasesDetailRequestHandler(SuspiciousCasesContext context, IOptions<ResponseOptions> responseOptions)
        {
            _context = context;
            _responseOptions = responseOptions.Value;
        }

        public async Task<SuspiciousCasesDetailResponse> Handle(SuspiciousCasesDetailRequest detailRequest, CancellationToken cancellationToken)
        {
            var paginationRequest = (PaginationRequest)detailRequest.Pagination;

            var suspiciousCases = (await _context.GetSuspiciousCase(
                detailRequest.Type,
                detailRequest.FilteringHeader.Filter!.NisCode!,
                paginationRequest.Offset,
                paginationRequest.Limit + 1,
                cancellationToken)).ToList();

            var nextUrl = _responseOptions.SuspiciousCasesTypeNextUrl.Replace("{type}", ((int)detailRequest.Type).ToString());

            return new SuspiciousCasesDetailResponse(
                    suspiciousCases
                        .Select(x =>
                            new SuspiciousCasesDetailResponseItem(x, _responseOptions))
                        .Take(paginationRequest.Limit)
                        .ToList(),
                    suspiciousCases.Count > paginationRequest.Limit
                        ? new Uri(string.Format(
                            nextUrl,
                            paginationRequest.Offset + paginationRequest.Limit,
                            paginationRequest.Limit))
                        : null);
        }
    }
}
