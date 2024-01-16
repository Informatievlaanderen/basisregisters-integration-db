namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using MediatR;
    using Microsoft.Extensions.Options;

    public sealed class SuspiciousCasesDetailRequestHandler : IRequestHandler<SuspiciousCasesDetailRequest, SuspiciousCasesDetailResponse>
    {
        private readonly ResponseOptions _responseOptions;

        public SuspiciousCasesDetailRequestHandler(IOptions<ResponseOptions> responseOptions)
        {
            _responseOptions = responseOptions.Value;
        }

        public async Task<SuspiciousCasesDetailResponse> Handle(SuspiciousCasesDetailRequest detailRequest, CancellationToken cancellationToken)
        {
            return new SuspiciousCasesDetailResponse(new List<Uri>
            {
                new Uri(string.Format(_responseOptions.SuspiciousCasesTypeUrl, "TODO"))
            });
        }
    }
}
