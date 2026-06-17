namespace Basisregisters.IntegrationDb.Api.Tests.SuspiciousCase.WhenDetailSuspiciousCases
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Api.SuspiciousCase;
    using Api.SuspiciousCase.Detail;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.HardCoded;
    using SuspiciousCases;
    using Xunit;

    public class GivenInterneBijwerkerWithWhiteListedOvoCode
    {
        private readonly Mock<IMediator> _mediator = new();

        private readonly IActionResult _response;

        public GivenInterneBijwerkerWithWhiteListedOvoCode()
        {
            Mock<IHttpContextAccessor> httpContextAccessor = new();
            var ovoCode = "OVO002949";
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new[]
                    {
                        new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, ovoCode) })
                    })
                });

            var suspiciousCasesController = new SuspiciousCasesController(
                _mediator.Object,
                httpContextAccessor.Object,
                new OvoCodeWhiteList(new List<string> { ovoCode }),
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
        public void ThenOkResponse()
        {
            _response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ThenNisCodeFromHeadersIsUsed()
        {
            _mediator.Verify(x => x.Send(
                It.Is<SuspiciousCasesDetailRequest>(y =>
                    y.FilteringHeader.Filter!.NisCode == "11001"
                    && y.Type == SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed),
                It.IsAny<CancellationToken>()));
        }
    }
}
