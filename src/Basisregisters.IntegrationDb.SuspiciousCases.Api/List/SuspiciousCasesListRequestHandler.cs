namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.List
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Converters;
    using Infrastructure;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Schema;

    public sealed class SuspiciousCasesListRequestHandler : IRequestHandler<SuspiciousCasesListRequest, SuspiciousCasesListResponse>
    {
        private readonly SuspiciousCasesContext _context;
        private readonly ResponseOptions _responseOptions;

        public SuspiciousCasesListRequestHandler(SuspiciousCasesContext context, IOptions<ResponseOptions> responseOptions)
        {
            _context = context;
            _responseOptions = responseOptions.Value;
        }

        public async Task<SuspiciousCasesListResponse> Handle(SuspiciousCasesListRequest listRequest, CancellationToken cancellationToken)
        {
            var suspiciousCases = await _context.SuspiciousCaseListItems
                .Where(x => x.NisCode.ToString() == listRequest.FilteringHeader.Filter.NisCode)
                .ToListAsync(cancellationToken: cancellationToken);

            return new SuspiciousCasesListResponse(
                suspiciousCases.Select(x =>
                {
                    var suspiciousCase = SuspiciousCase.AllCases[x.Type];

                    return new SuspiciousCasesListResponseItem(
                        suspiciousCase.Description,
                        suspiciousCase.Category.Map(),
                        suspiciousCase.Severity.Map(),
                        x.Count,
                        new Uri(string.Format(_responseOptions.SuspiciousCasesTypeUrl, (int)x.Type)));
                }).ToList());
        }
    }
}
