namespace Basisregisters.IntegrationDb.Api.Tests.WhenDetailSuspiciousCases
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Detail;
    using FluentAssertions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.Abstractions;
    using SuspiciousCases;
    using Xunit;

    public class GivenValidType
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly IActionResult _response;


        public GivenValidType()
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

            var suspiciousCasesController = new SuspiciousCasesController(
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

            _response = suspiciousCasesController.Detail((int)SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed, CancellationToken.None).Result;
        }

        [Fact]
        public void ThenOkResponse()
        {
            _response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ThenMediatorIsCalled()
        {
            _mediator.Verify(x => x.Send(
                It.Is<SuspiciousCasesDetailRequest>(y =>
                    y.Type == SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed),
                It.IsAny<CancellationToken>()));
        }
    }
}
