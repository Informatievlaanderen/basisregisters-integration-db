namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Infrastructure
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Npgsql;
    using Polly;

    public class MigrationsLogger { }

    public static class MigrationsHelper
    {
        public static void Run(
            string connectionString,
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
                    RunGtmfMeldingenMigrations(connectionString, loggerFactory);
                });
        }

        private static void RunGtmfMeldingenMigrations(string connectionString, ILoggerFactory? loggerFactory)
        {
            var migratorOptions = new DbContextOptionsBuilder<MeldingenContext>()
                .UseNpgsql(
                    connectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.EnableRetryOnFailure();
                        optionsBuilder.MigrationsHistoryTable(MeldingenContext.MigrationsTableName, MeldingenContext.Schema);
                        optionsBuilder.UseNetTopologySuite();
                    });

            if (loggerFactory != null)
            {
                migratorOptions = migratorOptions.UseLoggerFactory(loggerFactory);
            }

            using var migrator = new MeldingenContext(migratorOptions.Options);
            migrator.Database.Migrate();
        }
    }
}
