namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Tests.WhenListSuspiciousCases
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using List;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Moq;
    using NisCodeService.Abstractions;
    using Xunit;

    public class GivenDecentraleBijwerkerWithKnownOvoCode
    {
        private readonly Mock<IMediator> _mediator = new();
        private readonly IActionResult _response;

        private const string OvoCode = "OVO003105";
        private const string ExpectedNisCode = "11202";

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
                .Setup(x => x.Get(OvoCode, CancellationToken.None))
                .ReturnsAsync(ExpectedNisCode);

            var suspiciousCasesController = new SuspiciousCasesController(
                _mediator.Object,
                actionContextAccessor.Object,
                new OvoCodeWhiteList(new List<string>()),
                new OrganisationWhiteList(new List<string>()),
                nisCodeService.Object)
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
            _response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void ThenNisCodeFromHeadersIsUsed()
        {
            _mediator.Verify(x => x.Send(
                It.Is<SuspiciousCasesListRequest>(y => y.FilteringHeader.Filter.NisCode == ExpectedNisCode),
                It.IsAny<CancellationToken>()));
        }
    }
}
