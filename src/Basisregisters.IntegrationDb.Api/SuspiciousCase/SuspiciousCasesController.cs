namespace Basisregisters.IntegrationDb.Api.SuspiciousCase
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.SuspiciousCase.Detail;
    using Abstractions.SuspiciousCase.List;
    using Asp.Versioning;
    using Basisregisters.IntegrationDb.SuspiciousCases;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Detail;
    using FluentValidation;
    using FluentValidation.Results;
    using List;
    using MediatR;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using NisCodeService.Abstractions;
    using Swashbuckle.AspNetCore.Filters;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("verdachte-gevallen")]
    [ApiExplorerSettings(GroupName = "verdachtegevallen")]
    public class SuspiciousCasesController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IOvoCodeWhiteList _ovoCodeWhiteList;
        private readonly IOrganisationWhiteList _organisationWhiteList;
        private readonly INisCodeService _nisCodeService;

        private readonly DateTime? _nisCodeValidFrom;

        public SuspiciousCasesController(
            IMediator mediator,
            IActionContextAccessor actionContextAccessor,
            IOvoCodeWhiteList ovoCodeWhiteList,
            IOrganisationWhiteList organisationWhiteList,
            INisCodeService nisCodeService,
            IConfiguration configuration)
        {
            _mediator = mediator;
            _actionContextAccessor = actionContextAccessor;
            _ovoCodeWhiteList = ovoCodeWhiteList;
            _organisationWhiteList = organisationWhiteList;
            _nisCodeService = nisCodeService;

            _nisCodeValidFrom = configuration.GetValue<DateTime?>("NisCodeValidFrom");
        }

        /// <summary>
        /// Vraag verdachte gevallen types op en hun aantallen.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Lijst met verdachte gevallen.</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(SuspiciousCasesListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SuspiciousCasesListResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.DecentraleBijwerker)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var filtering = ExtractFiltering<SuspiciousCasesListFilter>(Request);

            filtering.Filter.NisCode = await DetermineNisCode(filtering.Filter.NisCode, cancellationToken);
            if (string.IsNullOrWhiteSpace(filtering.Filter.NisCode))
            {
                return Forbid();
            }

            var response = await _mediator.Send(new SuspiciousCasesListRequest(filtering), cancellationToken);

            return Ok(response);
        }

        /// <summary>
        /// Vraag verdachte gevallen op van een specifiek type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Lijst met de concrete verdachte gevallen van het bevraagde type.</response>
        /// <returns></returns>
        [HttpGet("{type}")]
        [ProducesResponseType(typeof(SuspiciousCasesDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SuspiciousCasesDetailResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.DecentraleBijwerker)]
        public async Task<IActionResult> Detail(
            [FromRoute] int type,
            CancellationToken cancellationToken)
        {
            var filtering = ExtractFiltering<SuspiciousCasesDetailFilter>(Request);

            filtering.Filter.NisCode = await DetermineNisCode(filtering.Filter.NisCode, cancellationToken);
            if (string.IsNullOrWhiteSpace(filtering.Filter.NisCode))
            {
                return Forbid();
            }

            if (!Enum.IsDefined(typeof(SuspiciousCasesType), type))
            {
                throw new ValidationException([
                    new ValidationFailure("Type", "Ongeldig verdacht geval type.")
                    {
                        ErrorCode = "OngeldigType"
                    }
                ]);
            }

            var pagination = Request.ExtractPaginationRequest();

            var response = await _mediator.Send(
                new SuspiciousCasesDetailRequest(filtering!, (SuspiciousCasesType)type, pagination),
                cancellationToken);

            return Ok(response);
        }

        private static FilteringHeader<T> ExtractFiltering<T>(HttpRequest request) where T: new()
        {
            var filtering = request.ExtractFilteringRequest<T>();
            if (filtering.Filter is null)
            {
                filtering = new FilteringHeader<T?>(new T());
            }

            return filtering!;
        }

        private async Task<string?> DetermineNisCode(string? nisCode, CancellationToken cancellationToken)
        {
            var ovoCode = _actionContextAccessor.ActionContext!.HttpContext.FindOvoCodeClaim();

            if (!string.IsNullOrWhiteSpace(ovoCode))
            {
                if (_ovoCodeWhiteList.IsWhiteListed(ovoCode))
                {
                    if (string.IsNullOrWhiteSpace(nisCode))
                    {
                        throw GetMissingNisCodeValidationException();
                    }

                    return nisCode;
                }

                var validFrom = _nisCodeValidFrom ?? DateTime.Now;

                return await _nisCodeService.Get(ovoCode, validFrom, cancellationToken);
            }

            var orgCode = _actionContextAccessor.ActionContext!.HttpContext.FindOrgCodeClaim();
            if (!string.IsNullOrWhiteSpace(orgCode) && _organisationWhiteList.IsWhiteListed(orgCode))
            {
                if (string.IsNullOrWhiteSpace(nisCode))
                {
                    throw GetMissingNisCodeValidationException();
                }

                return nisCode;
            }

            return null;
        }

        private ValidationException GetMissingNisCodeValidationException()
        {
            return new ValidationException([
                new ValidationFailure("NisCode", "Niscode ontbreekt.")
                {
                    ErrorCode = "OntbrekendeNiscode"
                }
            ]);
        }
    }
}
