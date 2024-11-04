namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Tests.WhenDetailSuspiciousCases
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NisCodeService.Abstractions;
    using Xunit;

    public class GivenDecentraleBijwerkerWithUnknownOvoCode
    {
        private readonly IActionResult _response;

        public GivenDecentraleBijwerkerWithUnknownOvoCode()
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
                            new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, "OVO003105") })
                        }),
                    }
                });

            Mock<INisCodeService> nisCodeService = new();
            nisCodeService
                .Setup(x => x.Get("OVO003105", It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(string.Empty);

            var suspiciousCasesController = new SuspiciousCasesController(
                new Mock<IMediator>().Object,
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
            _response.Should().BeOfType<ForbidResult>();
        }
    }
}
