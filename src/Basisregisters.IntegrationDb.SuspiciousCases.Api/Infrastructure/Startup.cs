namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Abstractions;
    using Asp.Versioning.ApiExplorer;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Configuration;
    using FluentValidation;
    using IdentityModel.AspNetCore.OAuth2Introspection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using Modules;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private const string DatabaseTag = "db";

        private IContainer _applicationContainer = null!;

        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var oAuth2IntrospectionOptions = _configuration
                .GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>();

            var baseUrl = _configuration.GetValue<string>("BaseUrl")!;
            var baseUrlForExceptions = baseUrl.EndsWith("/")
                ? baseUrl.Substring(0, baseUrl.Length - 1)
                : baseUrl;

            services.AddAcmIdmAuthentication(oAuth2IntrospectionOptions!);
            services
                .ConfigureDefaultForApi<Startup>(new StartupConfigureOptions
                    {
                        Cors =
                        {
                            Origins = _configuration
                                .GetSection("Cors")
                                .GetChildren()
                                .Select(c => c.Value)
                                .ToArray()!
                        },
                        Server =
                        {
                            BaseUrl = baseUrlForExceptions
                        },
                        Swagger =
                        {
                            ApiInfo = (_, description) => new OpenApiInfo
                            {
                                Version = description.ApiVersion.ToString(),
                                Title = "Basisregisters Vlaanderen Verdachte Gevallen API",
                                Description = GetApiLeadingText(description),
                                Contact = new OpenApiContact
                                {
                                    Name = "Digitaal Vlaanderen",
                                    Email = "digitaal.vlaanderen@vlaanderen.be",
                                    Url = new Uri("https://backoffice.basisregisters.vlaanderen")
                                }
                            },
                            XmlCommentPaths = new[] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name! }
                        },
                        MiddlewareHooks =
                        {
                            AfterHealthChecks = health =>
                            {
                                var connectionStrings = _configuration
                                    .GetSection("ConnectionStrings")
                                    .GetChildren();

                                foreach (var connectionString in connectionStrings)
                                    health.AddNpgSql(
                                        connectionString.Value!,
                                        name: $"npgsql-{connectionString.Key.ToLowerInvariant()}",
                                        tags: new[] {DatabaseTag, "sql", "npgsql"});
                            },
                            Authorization = options => { options.AddAddressPolicies([]); }
                        }
                    }
                .EnableJsonErrorActionFilterOption())
                .AddValidatorsFromAssemblyContaining<Startup>()
                .Configure<ResponseOptions>(_configuration.GetSection("ResponseOptions"))
                .AddHostedService<RefreshCountService>()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>(); // Used to retrieve the authenticated user claims.

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new ApiModule(_configuration, services, _loggerFactory));
            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            HealthCheckService healthCheckService)
        {
            app
                .UseDefaultForApi(new StartupUseOptions
                {
                    Common =
                    {
                        ApplicationContainer = _applicationContainer,
                        ServiceProvider = serviceProvider,
                        HostingEnvironment = env,
                        ApplicationLifetime = appLifetime,
                        LoggerFactory = loggerFactory,
                    },
                    Api =
                    {
                        VersionProvider = apiVersionProvider,
                        Info = groupName => $"Basisregisters Vlaanderen - Verdachte Gevallen API {groupName}",
                        CSharpClientOptions =
                        {
                            ClassName = "SuspiciousCases.Api",
                            Namespace = "Be.Vlaanderen.Basisregisters.IntegrationDb"
                        },
                        TypeScriptClientOptions =
                        {
                            ClassName = "SuspiciousCases.Api"
                        }
                    },
                    Server =
                    {
                        PoweredByName = "Vlaamse overheid - Basisregisters Vlaanderen",
                        ServerName = "Digitaal Vlaanderen"
                    },
                    MiddlewareHooks =
                    {
                        EnableAuthorization = true,
                        AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                    }
                });

            appLifetime.ApplicationStarted.Register(() =>
            {
                MigrationsHelper.Run(
                    _configuration.GetConnectionString("Integration")!,
                    _loggerFactory);
            });

            StartupHelpers.CheckDatabases(healthCheckService, DatabaseTag, loggerFactory).GetAwaiter().GetResult();
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            =>
                $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Verdachte Gevallen API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
    }
}
