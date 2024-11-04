namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Tests.WhenDetailSuspiciousCases
{
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
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.HardCoded;
    using Xunit;

    public class GivenGlobaleBijwerkerWithNonWhiteListedOrgCode
    {
        private readonly IActionResult _response;

        public GivenGlobaleBijwerkerWithNonWhiteListedOrgCode()
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
                            new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOrgCode, "0643634986") })
                        }),
                    }
                });

            var suspiciousCasesController = new SuspiciousCasesController(
                new Mock<IMediator>().Object,
                actionContextAccessor.Object,
                new OvoCodeWhiteList(new List<string>()),
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

            _response = suspiciousCasesController.Detail((int)SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed, CancellationToken.None).Result;
        }

        [Fact]
        public void ThenForbid()
        {
            _response.Should().BeOfType<ForbidResult>();
        }
    }
}
