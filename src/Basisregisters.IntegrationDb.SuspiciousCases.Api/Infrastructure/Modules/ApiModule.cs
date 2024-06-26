namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Infrastructure.Modules
{
    using System.Collections.Generic;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.Auth;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NisCodeService.HardCoded.Extensions;
    using SuspiciousCases.Infrastructure;

    public class ApiModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<ProblemDetailsHelper>()
                .AsSelf();

            builder
                .RegisterModule(new MediatRModule())
                .RegisterModule(new SuspiciousCasesModule(_configuration, _services, _loggerFactory));

            _services.AddAcmIdmAuthorizationHandlers();

            var ovoCodeWhiteList = _configuration.GetSection("OvoCodeWhiteList").Get<List<string>>();
            var organisationWhiteList = _configuration.GetSection("OrganisationWhiteList").Get<List<string>>();
            _services
                .AddHardCodedNisCodeService()
                .AddOvoCodeWhiteList(ovoCodeWhiteList)
                .AddOrganisationWhiteList(organisationWhiteList)
                ;

            builder.Populate(_services);
        }
    }
}
