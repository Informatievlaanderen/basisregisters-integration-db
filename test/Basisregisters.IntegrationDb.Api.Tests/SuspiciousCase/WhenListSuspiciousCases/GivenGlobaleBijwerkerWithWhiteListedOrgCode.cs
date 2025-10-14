namespace Basisregisters.IntegrationDb.Api.Tests.SuspiciousCase.WhenListSuspiciousCases
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Basisregisters.IntegrationDb.Api.SuspiciousCase;
    using Basisregisters.IntegrationDb.Api.SuspiciousCase.List;
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

    public class GivenGlobaleBijwerkerWithWhiteListedOrgCode
    {
        private readonly Mock<IMediator> _mediator = new();

        private readonly IActionResult _response;

        public GivenGlobaleBijwerkerWithWhiteListedOrgCode()
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
                _mediator.Object,
                actionContextAccessor.Object,
                new OvoCodeWhiteList(new List<string>()),
                new OrganisationWhiteList(new List<string> { "0643634986" }),
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
        public void ThenOkResponse()
        {
            _response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ThenNisCodeFromHeadersIsUsed()
        {
            _mediator.Verify(x => x.Send(
                It.Is<SuspiciousCasesListRequest>(y => y.FilteringHeader.Filter.NisCode == "11001"),
                It.IsAny<CancellationToken>()));
        }
    }
}
