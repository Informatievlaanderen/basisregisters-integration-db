namespace Basisregisters.IntegrationDb.SuspiciousCases.Api
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
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
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using NisCodeService.Abstractions;
    using Schema;

    [ApiVersion("2.0")]
    [AdvertiseApiVersions("2.0")]
    [ApiRoute("verdachte-gevallen")]
    [ApiExplorerSettings(GroupName = "SuspiciousCases")]
    public class SuspiciousCasesController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IOvoCodeWhiteList _ovoCodeWhiteList;
        private readonly INisCodeService _nisCodeService;

        public SuspiciousCasesController(
            IMediator mediator,
            IActionContextAccessor actionContextAccessor,
            IOvoCodeWhiteList ovoCodeWhiteList,
            INisCodeService nisCodeService)
        {
            _mediator = mediator;
            _actionContextAccessor = actionContextAccessor;
            _ovoCodeWhiteList = ovoCodeWhiteList;
            _nisCodeService = nisCodeService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.DecentraleBijwerker)]
        public async Task<IActionResult> List(CancellationToken cancellationToken)
        {
            var ovoCode = _actionContextAccessor.ActionContext!.HttpContext.FindOvoCodeClaim();

            if (string.IsNullOrWhiteSpace(ovoCode))
            {
                return Forbid();
            }

            var filtering = Request.ExtractFilteringRequest<SuspiciousCasesListFilter>();

            if (!_ovoCodeWhiteList.IsWhiteListed(ovoCode))
            {
                var nisCode = await _nisCodeService.Get(ovoCode, cancellationToken);

                if (string.IsNullOrWhiteSpace(nisCode))
                {
                    return Forbid();
                }

                filtering.Filter.NisCode = nisCode;
            }

            if (string.IsNullOrWhiteSpace(filtering.Filter.NisCode))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("NisCode", "Niscode ontbreekt.")
                    {
                        ErrorCode = "OntbrekendeNiscode"
                    }
                });
            }

            var response = await _mediator.Send(new SuspiciousCasesListRequest(filtering), cancellationToken);

            return Ok(response);
        }

        [HttpGet("{type}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = PolicyNames.Adres.DecentraleBijwerker)]
        public async Task<IActionResult> Detail(
            [FromRoute] int type,
            CancellationToken cancellationToken)
        {
            var ovoCode = _actionContextAccessor.ActionContext!.HttpContext.FindOvoCodeClaim();

            if (string.IsNullOrWhiteSpace(ovoCode))
            {
                return Forbid();
            }

            var filtering = Request.ExtractFilteringRequest<SuspiciousCasesDetailFilter>();

            if (!_ovoCodeWhiteList.IsWhiteListed(ovoCode))
            {
                var nisCode = await _nisCodeService.Get(ovoCode, cancellationToken);

                if (string.IsNullOrWhiteSpace(nisCode))
                {
                    return Forbid();
                }

                filtering.Filter.NisCode = nisCode;
            }

            if (string.IsNullOrWhiteSpace(filtering.Filter.NisCode))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("NisCode", "Niscode ontbreekt.")
                    {
                        ErrorCode = "OntbrekendeNiscode"
                    }
                });
            }

            if (!Enum.IsDefined(typeof(SuspiciousCasesType), type))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure("Type", "Ongeldig verdacht geval type.")
                    {
                        ErrorCode = "OngeldigType"
                    }
                });
            }

            var pagination = Request.ExtractPaginationRequest();

            var response = await _mediator.Send(
                new SuspiciousCasesDetailRequest(filtering, (SuspiciousCasesType)type, pagination),
                cancellationToken);

            return Ok(response);
        }
    }
}
