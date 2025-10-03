namespace Basisregisters.IntegrationDb.Api.Address
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using CorrectDerivedFromBuildingUnitPositions;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Filters;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("adressen")]
    [ApiExplorerSettings(GroupName = "adressen")]
    public class AddressesController : ApiController
    {
        private readonly IMediator _mediator;

        public AddressesController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Corrigeer adres posities met methode Afgeleid en specificatie Gebouweenheid.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="202"></response>
        /// <returns></returns>
        [HttpPost("corrigeren/afgeleid-van-gebouweenheid-posities")]
        [ProducesResponseType(typeof(CorrigerenAfgeleidVanGebouwEenhedenResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.InterneBijwerker)]
        public async Task<IActionResult> CorrectDerivedFromBuildingUnitPositions(
            [FromBody] CorrigerenAfgeleidVanGebouwEenhedenRequest? request,
            [FromServices] CorrectieAfgeleidVanGebouwEenhedenRequestValidator validator,
            CancellationToken cancellationToken)
        {
            request ??= new CorrigerenAfgeleidVanGebouwEenhedenRequest();
            await validator.ValidateAndThrowAsync(request, cancellationToken);

            var internalRequest = new CorrectDerivedFromBuildingUnitPositionsRequest(request.Adressen);

            var response = await _mediator.Send(internalRequest, cancellationToken);
            return Accepted(response);
        }
    }

    public class CorrectieAfgeleidVanGebouwEenhedenRequestValidator : AbstractValidator<CorrigerenAfgeleidVanGebouwEenhedenRequest>
    {
        public CorrectieAfgeleidVanGebouwEenhedenRequestValidator()
        {
            When(x => x.Adressen is not null, () =>
            {
                RuleFor(x => x.Adressen)
                    .Must(value => value!.Count <= 100)
                    .WithMessage("Maximaal 100 adressen per keer toegestaan.")
                    ;
            });
        }
    }
}
