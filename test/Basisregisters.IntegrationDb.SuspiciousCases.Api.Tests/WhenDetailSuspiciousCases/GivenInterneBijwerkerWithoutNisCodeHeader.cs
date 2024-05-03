namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Tests.WhenDetailSuspiciousCases
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using FluentAssertions;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Primitives;
    using Moq;
    using NisCodeService.HardCoded;
    using Xunit;

    public class GivenInterneBijwerkerWithoutNisCodeHeader
    {
        private readonly SuspiciousCasesController _suspiciousCasesController;

        public GivenInterneBijwerkerWithoutNisCodeHeader()
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
                            new ClaimsIdentity(new[] { new Claim(AcmIdmClaimTypes.VoOvoCode, "OVO002949") })
                        }),
                    }
                });

            _suspiciousCasesController = new SuspiciousCasesController(
                Mock.Of<IMediator>(),
                actionContextAccessor.Object,
                new OvoCodeWhiteList(new List<string> { "OVO002949" }),
                new HardCodedNisCodeService())
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
        public void ThenBadRequestResponse()
        {
            var act = () =>
                _suspiciousCasesController.Detail((int)SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed, CancellationToken.None);
            act
                .Should()
                .ThrowAsync<ValidationException>()
                .Result
                .Where(x =>
                    x.Errors.Any(e => e.ErrorCode == "OntbrekendeNiscode" && e.ErrorMessage == "Niscode ontbreekt."));
        }
    }
}
