namespace Basisregisters.IntegrationDb.SuspiciousCases.Api
{
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Detail;
    using List;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("verdachte-gevallen")]
    [ApiExplorerSettings(GroupName = "SuspiciousCases")]
    public class SuspiciousCasesController : ApiController
    {
        private readonly IMediator _mediator;

        public SuspiciousCasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.DecentraleBijwerker)]
        public async Task<IActionResult> GetSuspiciousCases(CancellationToken cancellationToken)
        {
            var filtering = Request.ExtractFilteringRequest<SuspiciousCasesListFilter>();
            var response = await _mediator.Send(new SuspiciousCasesListRequest(filtering), cancellationToken);

            return Ok(response);
        }

        [HttpGet("{type}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.DecentraleBijwerker)]
        public async Task<IActionResult> GetSuspiciousCases(
            [FromRoute] string type,
            CancellationToken cancellationToken)
        {
            var filtering = Request.ExtractFilteringRequest<SuspiciousCasesDetailFilter>();
            var response = await _mediator.Send(new SuspiciousCasesDetailRequest(filtering, type), cancellationToken);

            return Ok(response);
        }
    }
}
