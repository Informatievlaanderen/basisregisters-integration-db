namespace Basisregisters.IntegrationDb.Api.Tests.SuspiciousCase.WhenDetailSuspiciousCases
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.SuspiciousCase;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.HardCoded;
    using SuspiciousCases;
    using Xunit;

    public class GivenInterneBijwerkerWithoutNisCodeHeader
    {
        private readonly SuspiciousCasesController _suspiciousCasesController;

        public GivenInterneBijwerkerWithoutNisCodeHeader()
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

            _suspiciousCasesController = new SuspiciousCasesController(
                Mock.Of<IMediator>(),
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
                        Request = { Headers = { new KeyValuePair<string, StringValues>("X-Filtering", "{ }") } }
                    }
                }
            };
        }

        [Fact]
        public async Task ThenBadRequestResponse()
        {
            var act = () =>
                _suspiciousCasesController.Detail((int)SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed, CancellationToken.None);
            await act
                .Should()
                .ThrowAsync<ValidationException>()
                .Where(x =>
                    x.Errors.Any(e => e.ErrorCode == "OntbrekendeNiscode" && e.ErrorMessage == "Niscode ontbreekt."));
        }
    }
}
