namespace Basisregisters.IntegrationDb.Api.Address
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;
    using Asp.Versioning;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.Oslo.Extensions;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit.Validators;
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

            var addressIds = request.Adressen!
                .Select(adres => adres.AsIdentifier().Map(int.Parse).Value)
                .ToList();
            var internalRequest = new CorrectDerivedFromBuildingUnitPositionsRequest(addressIds);

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
                    .WithErrorCode("AdressenLijstTeGroot");

                RuleForEach(x => x.Adressen)
                    .Must(value =>
                        OsloPuriValidator.TryParseIdentifier(value, out var addressId)
                        && int.TryParse(addressId, out _))
                    .WithMessage((_, id) => $"Onbestaand adres '{id}'.")
                    .WithErrorCode("AdresIdIsOnbestaand");
            });
        }
    }
}
