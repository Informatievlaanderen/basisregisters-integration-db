namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Infrastructure;
    using MediatR;
    using Microsoft.Extensions.Options;
    using Schema;

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
                detailRequest.FilteringHeader.Filter.NisCode!,
                paginationRequest.Offset,
                paginationRequest.Limit + 1,
                cancellationToken)).ToList();

            return new SuspiciousCasesDetailResponse(
                    suspiciousCases
                        .Select(x =>
                            new SuspiciousCasesDetailResponseItem(x, _responseOptions))
                        .Take(paginationRequest.Limit)
                        .ToList(),
                    suspiciousCases.Count > paginationRequest.Limit
                        ? new Uri(string.Format(
                            _responseOptions.SuspiciousCasesTypeNextUrl,
                            (int)detailRequest.Type,
                            paginationRequest.Offset + paginationRequest.Limit,
                            paginationRequest.Limit))
                        : null);
        }
    }
}
