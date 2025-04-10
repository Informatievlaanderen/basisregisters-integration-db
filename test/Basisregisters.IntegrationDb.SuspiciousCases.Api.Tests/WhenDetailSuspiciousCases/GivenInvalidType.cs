namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Tests.WhenDetailSuspiciousCases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.Abstractions;
    using Xunit;

    public class GivenInvalidType
    {
        private readonly Mock<IMediator> _mediator = new();

        private readonly SuspiciousCasesController _suspiciousCasesController;

        public GivenInvalidType()
        {
            const string ovoCode = "OVO002037";
            const string expectedNisCode = "31005";

            Mock<IActionContextAccessor> actionContextAccessor = new();
            actionContextAccessor
                .Setup(x => x.ActionContext)
                .Returns(new ActionContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new[]
                        {
                            new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, ovoCode) })
                        }),
                    }
                });

            Mock<INisCodeService> nisCodeService = new();
            nisCodeService
                .Setup(x => x.Get(ovoCode, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(expectedNisCode);

            _suspiciousCasesController = new SuspiciousCasesController(
                _mediator.Object,
                actionContextAccessor.Object,
                new OvoCodeWhiteList(new List<string>()),
                new OrganisationWhiteList(new List<string>()),
                nisCodeService.Object,
                new ConfigurationBuilder().Build())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Headers = { new KeyValuePair<string, StringValues>("X-Filtering", "{ \"nisCode\": \"11001\"}") } }
                    }
                }
            };
        }

        [Fact]
        public async Task ThenBadRequestResponse()
        {
            var act = () => _suspiciousCasesController.Detail(int.MaxValue, CancellationToken.None);

            await act
                .Should()
                .ThrowAsync<ValidationException>()
                .Where(x =>
                    x.Errors.Any(e =>
                        e.ErrorCode == "OngeldigType"
                        && e.ErrorMessage == "Ongeldig verdacht geval type."));
        }
    }
}
