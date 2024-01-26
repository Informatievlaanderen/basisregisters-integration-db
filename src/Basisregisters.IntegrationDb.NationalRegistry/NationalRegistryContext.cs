namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class NationalRegistryContext : DbContext
    {
        public const string Schema = "national_registry";
        public const string MigrationsTableName = "__EFMigrationsHistoryNationalRegistry";

        public NationalRegistryContext()
        { }

        public NationalRegistryContext(DbContextOptions<NationalRegistryContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NationalRegistryContext).Assembly);
        }
    }

    public class ConfigBasedIntegrationContextFactory : IDesignTimeDbContextFactory<NationalRegistryContext>
    {
        public NationalRegistryContext CreateDbContext(string[] args)
        {
            var migrationConnectionStringName = "IntegrationAdmin";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .AddJsonFile($"appsettings.development.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<NationalRegistryContext>();

            var connectionString = configuration
                                       .GetConnectionString(migrationConnectionStringName)
                                   ?? throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

            builder
                .UseNpgsql(connectionString, npgSqlOptions =>
                {
                    npgSqlOptions.EnableRetryOnFailure();
                    npgSqlOptions.MigrationsHistoryTable(
                        NationalRegistryContext.MigrationsTableName,
                        NationalRegistryContext.Schema);
                });

            return new NationalRegistryContext(builder.Options);
        }
    }
}
