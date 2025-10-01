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
    using Moq;
    using NisCodeService.Abstractions;
    using SuspiciousCases;
    using Xunit;

    public class GivenDecentraleBijwerkerWithKnownOvoCode
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly IActionResult _response;

        private const string OvoCode = "OVO002037";
        private const string ExpectedNisCode = "31005";

        public GivenDecentraleBijwerkerWithKnownOvoCode()
        {
            Mock<IActionContextAccessor> actionContextAccessor = new();
            actionContextAccessor
                .Setup(x => x.ActionContext)
                .Returns(new ActionContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new[]
                        {
                            new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, OvoCode) })
                        }),
                    }
                });

            Mock<INisCodeService> nisCodeService = new();
            nisCodeService
                .Setup(x => x.Get(OvoCode, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(ExpectedNisCode);

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
                    HttpContext = new DefaultHttpContext()
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
        public void ThenNisCodeFromHeadersIsUsed()
        {
            _mediator.Verify(x => x.Send(
                It.Is<SuspiciousCasesDetailRequest>(y =>
                    y.FilteringHeader.Filter!.NisCode == ExpectedNisCode &&
                    y.Type == SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed),
                It.IsAny<CancellationToken>()));
        }
    }
}
