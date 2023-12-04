namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.List
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using MediatR;
    using Microsoft.Extensions.Options;

    public sealed class SuspiciousCasesListRequestHandler : IRequestHandler<SuspiciousCasesListRequest, SuspiciousCasesListResponse>
    {
        private readonly ResponseOptions _responseOptions;

        public SuspiciousCasesListRequestHandler(IOptions<ResponseOptions> responseOptions)
        {
            _responseOptions = responseOptions.Value;
        }

        public async Task<SuspiciousCasesListResponse> Handle(SuspiciousCasesListRequest listRequest, CancellationToken cancellationToken)
        {
            return new SuspiciousCasesListResponse(new List<SuspiciousCasesListResponseItem>
            {
                new SuspiciousCasesListResponseItem(
                    "Huisnummers \"in gebruik\" zonder koppeling met perceel",
                    Category.Address,
                    Severity.Incorrect,
                    10,
                    new Uri(string.Format(_responseOptions.SuspiciousCasesTypeUrl, "TODO")))
            });
        }
    }
}
