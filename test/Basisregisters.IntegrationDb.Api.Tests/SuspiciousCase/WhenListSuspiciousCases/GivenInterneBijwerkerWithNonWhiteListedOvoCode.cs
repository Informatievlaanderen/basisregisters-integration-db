namespace Basisregisters.IntegrationDb.Api.Tests.SuspiciousCase.WhenListSuspiciousCases
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Basisregisters.IntegrationDb.Api.SuspiciousCase;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.HardCoded;
    using Xunit;

    public class GivenInterneBijwerkerWithNonWhiteListedOvoCode
    {
        private readonly IActionResult _response;

        public GivenInterneBijwerkerWithNonWhiteListedOvoCode()
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
                            new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, "OVO002950") })
                        }),
                    }
                });

            var suspiciousCasesController = new SuspiciousCasesController(
                new Mock<IMediator>().Object,
                actionContextAccessor.Object,
                new OvoCodeWhiteList(new List<string> { "OVO002949" }),
                new OrganisationWhiteList(new List<string>()),
                new HardCodedNisCodeService(),
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

            _response = suspiciousCasesController.List(CancellationToken.None).Result;
        }

        [Fact]
        public void ThenForbid()
        {
            _response.Should().BeOfType<ForbidResult>();
        }
    }
}
