namespace Basisregisters.IntegrationDb.DataIntegrity;

using System;
using System.IO;
using Feeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public sealed class DataIntegrityContext : DbContext
{
    public const string MigrationsTableName = "__EFMigrationsHistoryIntegrity";
    public const string Schema = "integration_data_integrity";

    public DbSet<MunicipalityFeedIntegrity> MunicipalityFeedIntegrity => Set<MunicipalityFeedIntegrity>();
    public DbSet<PostalFeedIntegrity> PostalFeedIntegrity => Set<PostalFeedIntegrity>();
    public DbSet<PostalNameFeedIntegrity> PostalNameFeedIntegrity => Set<PostalNameFeedIntegrity>();
    public DbSet<StreetNameFeedIntegrity> StreetNameFeedIntegrity => Set<StreetNameFeedIntegrity>();

    public DataIntegrityContext()
    { }

    public DataIntegrityContext(DbContextOptions<DataIntegrityContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataIntegrityContext).Assembly);
    }
}

public class ConfigBasedDataIntegrationContextFactory : IDesignTimeDbContextFactory<DataIntegrityContext>
{
    public DataIntegrityContext CreateDbContext(string[] args)
    {
        var migrationConnectionStringName = "Integration";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
            .AddJsonFile($"appsettings.development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<DataIntegrityContext>();

        var connectionString = configuration
                                   .GetConnectionString(migrationConnectionStringName)
                               ?? throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

        builder
            .UseNpgsql(connectionString, npgSqlOptions =>
            {
                npgSqlOptions.EnableRetryOnFailure();
                npgSqlOptions.MigrationsHistoryTable(
                    DataIntegrityContext.MigrationsTableName,
                    DataIntegrityContext.Schema);
                npgSqlOptions.UseNetTopologySuite();
                npgSqlOptions.CommandTimeout(260);
            });

        return new DataIntegrityContext(builder.Options);
    }
}
