namespace Basisregisters.IntegrationDb.Api.Tests.SuspiciousCase.WhenListSuspiciousCases
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.Api.SuspiciousCase;
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
    using NisCodeService.HardCoded;
    using Xunit;

    public class GivenGlobaleBijwerkerWithoutNisCodeHeader
    {
        private readonly SuspiciousCasesController _suspiciousCasesController;

        public GivenGlobaleBijwerkerWithoutNisCodeHeader()
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

            _suspiciousCasesController = new SuspiciousCasesController(
                Mock.Of<IMediator>(),
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
                        Request = { Headers = { new KeyValuePair<string, StringValues>("X-Filtering", "{ }") } }
                    }
                }
            };
        }

        [Fact]
        public async Task ThenThrowsValidationException()
        {
            var act = () => _suspiciousCasesController.List(CancellationToken.None);
            await act
                .Should()
                .ThrowAsync<ValidationException>()
                .Where(x =>
                    x.Errors.Any(e => e.ErrorCode == "OntbrekendeNiscode" && e.ErrorMessage == "Niscode ontbreekt."));
        }
    }
}
