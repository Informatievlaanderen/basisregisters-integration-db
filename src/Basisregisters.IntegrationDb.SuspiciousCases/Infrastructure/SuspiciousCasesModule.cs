namespace Basisregisters.IntegrationDb.SuspiciousCases.Infrastructure
{
    using System;
    using Autofac;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class SuspiciousCasesModule : Module
    {
        public SuspiciousCasesModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<SuspiciousCasesModule>();
            var connectionString = configuration.GetConnectionString("Integration");

            var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);
            if (hasConnectionString)
                RunOnNpgSqlServer(services, connectionString);
            else
                RunInMemoryDb(services, loggerFactory, logger);

            logger.LogInformation(
                "Added {Context} to services:" +
                Environment.NewLine +
                "\tSchema: {Schema}" +
                Environment.NewLine +
                "\tTableName: {TableName}",
                nameof(SuspiciousCasesContext), Schema.SuspiciousCases, SuspiciousCasesContext.MigrationsTableName);
        }

        private static void RunOnNpgSqlServer(
            IServiceCollection services,
            string connectionString)
        {
            services
                .AddNpgsql<SuspiciousCasesContext>(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.EnableRetryOnFailure();
                    sqlServerOptions.MigrationsHistoryTable(SuspiciousCasesContext.MigrationsTableName, Schema.SuspiciousCases);
                    sqlServerOptions.UseNetTopologySuite();
                });
        }

        private static void RunInMemoryDb(
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            ILogger logger)
        {
            services
                .AddDbContext<SuspiciousCasesContext>(options => options
                    .UseLoggerFactory(loggerFactory)
                    .UseInMemoryDatabase(Guid.NewGuid().ToString(), _ => { }));

            logger.LogWarning("Running InMemory for {Context}!", nameof(SuspiciousCasesContext));
        }
    }
}
