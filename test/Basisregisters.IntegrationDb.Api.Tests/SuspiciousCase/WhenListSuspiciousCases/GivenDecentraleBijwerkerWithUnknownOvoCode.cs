namespace Basisregisters.IntegrationDb.Api.Tests.SuspiciousCase.WhenListSuspiciousCases
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Api.SuspiciousCase;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NisCodeService.Abstractions;
    using Xunit;

    public class GivenDecentraleBijwerkerWithUnknownOvoCode
    {
        private readonly IActionResult _response;

        public GivenDecentraleBijwerkerWithUnknownOvoCode()
        {
            Mock<IHttpContextAccessor> httpContextAccessor = new();
            var ovoCode = "OVO003105";
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new[]
                    {
                        new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, ovoCode) })
                    })
                });

            Mock<INisCodeService> nisCodeService = new();
            nisCodeService
                .Setup(x => x.Get(ovoCode, It.IsAny<DateTime>(), CancellationToken.None))
                .ReturnsAsync(string.Empty);

            var suspiciousCasesController = new SuspiciousCasesController(
                new Mock<IMediator>().Object,
                httpContextAccessor.Object,
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

            _response = suspiciousCasesController.List(CancellationToken.None).Result;
        }

        [Fact]
        public void ThenOkResponse()
        {
            _response.Should().BeOfType<ForbidResult>();
        }
    }
}
