namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Infrastructure
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Npgsql;
    using Polly;
    using SuspiciousCases.Infrastructure;

    public class MigrationsLogger { }

    public static class MigrationsHelper
    {
        public static void Run(
            string sequenceConnectionString,
            ILoggerFactory? loggerFactory = null)
        {
            var logger = loggerFactory?.CreateLogger<MigrationsLogger>();

            Policy
                .Handle<NpgsqlException>()
                .WaitAndRetry(
                    5,
                    retryAttempt =>
                    {
                        var value = Math.Pow(2, retryAttempt) / 4;
                        var randomValue = new Random().Next((int)value * 3, (int)value * 5);
                        logger?.LogInformation("Retrying after {Seconds} seconds...", randomValue);
                        return TimeSpan.FromSeconds(randomValue);
                    })
                .Execute(() =>
                {
                    logger?.LogInformation("Running EF Migrations.");
                    RunSuspiciousCasesMigrations(sequenceConnectionString, loggerFactory);
                });
        }

        private static void RunSuspiciousCasesMigrations(string connectionString, ILoggerFactory? loggerFactory)
        {
            var migratorOptions = new DbContextOptionsBuilder<SuspiciousCasesContext>()
                .UseNpgsql(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure();
                        sqlServerOptions.MigrationsHistoryTable(SuspiciousCasesContext.MigrationsTableName, Schema.SuspiciousCases);
                        sqlServerOptions.UseNetTopologySuite();
                    });

            if (loggerFactory != null)
            {
                migratorOptions = migratorOptions.UseLoggerFactory(loggerFactory);
            }

            using var migrator = new SuspiciousCasesContext(migratorOptions.Options);
            migrator.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
            migrator.Database.Migrate();
        }
    }
}
