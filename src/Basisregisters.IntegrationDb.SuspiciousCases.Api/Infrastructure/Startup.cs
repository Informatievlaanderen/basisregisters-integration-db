namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Auth.AcmIdm;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Microsoft;
    using Configuration;
    using IdentityModel.AspNetCore.OAuth2Introspection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.OpenApi.Models;
    using Modules;
    using Schema;

    /// <summary>Represents the startup process for the application.</summary>
    public class Startup
    {
        private const string DatabaseTag = "db";

        private IContainer _applicationContainer;

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

            var baseUrl = _configuration.GetValue<string>("BaseUrl");
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
                                .ToArray()
                        },
                        Server =
                        {
                            BaseUrl = baseUrlForExceptions
                        },
                        Swagger =
                        {
                            ApiInfo = (provider, description) => new OpenApiInfo
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
                            XmlCommentPaths = new[] { typeof(Startup).GetTypeInfo().Assembly.GetName().Name }
                        },
                        MiddlewareHooks =
                        {
                            FluentValidation = fv => fv
                                .RegisterValidatorsFromAssemblyContaining<Startup>(),

                            AfterHealthChecks = health =>
                            {
                                var connectionStrings = _configuration
                                    .GetSection("ConnectionStrings")
                                    .GetChildren();

                                foreach (var connectionString in connectionStrings)
                                    health.AddSqlServer(
                                        connectionString.Value,
                                        name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
                                        tags: new[] { DatabaseTag, "sql", "sqlserver" });
                            },
                            Authorization = options => { options.AddAcmIdmAuthorization(); }
                        }
                    }

                .EnableJsonErrorActionFilterOption())
                .Configure<ResponseOptions>(_configuration.GetSection("ResponseOptions"))
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>(); // Used to retrieve the authenticated user claims.

            var connectionString = _configuration.GetConnectionString("IntegrationDb")
                                   ?? throw new InvalidOperationException(
                                       $"Could not find a connection string with name 'IntegrationDb'");

            services
                .AddDbContext<IntegrationContext>((_, options) =>
                {
                    options.UseLoggerFactory(new NullLoggerFactory());
                    options.UseNpgsql(connectionString, sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions.MigrationsHistoryTable(IntegrationContext.MigrationsTableName, IntegrationContext.Schema);
                        sqlServerOptions.UseNetTopologySuite();
                    });
                });

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
            ApiDataDogToggle datadogToggle,
            ApiDebugDataDogToggle debugDataDogToggle,
            HealthCheckService healthCheckService)
        {
            app
                //TODO: Replace by Datadog Vendor Libraries
                .UseDataDog<Startup>(new DataDogOptions
                {
                    Common =
                    {
                        ServiceProvider = serviceProvider,
                        LoggerFactory = loggerFactory
                    },
                    Toggles =
                    {
                        Enable = datadogToggle,
                        Debug = debugDataDogToggle
                    },
                    Tracing =
                    {
                        ServiceName = _configuration["DataDog:ServiceName"],
                    }
                })
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
                        AfterMiddleware = x => x.UseMiddleware<AddNoCacheHeadersMiddleware>(),
                    }
                });

            // MigrationsHelper.Run(
            //     _configuration.GetConnectionString("SuspiciousCasesAdmin"),
            //     serviceProvider.GetService<ILoggerFactory>());

            //StartupHelpers.CheckDatabases(healthCheckService, DatabaseTag, loggerFactory).GetAwaiter().GetResult();
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            =>
                $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Verdachte Gevallen API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
    }
}
