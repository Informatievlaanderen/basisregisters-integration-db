namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases;

using System;
using System.IO;
using IntegrationDb.SuspiciousCases.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class SuspiciousCaseReportingContext : DbContext
{
    public const string MigrationsTableName = "__EFMigrationsHistoryReporting";

    public DbSet<SuspiciousCase> SuspiciousCases => Set<SuspiciousCase>();
    public DbSet<SuspiciousCaseReport> SuspiciousCaseReports => Set<SuspiciousCaseReport>();

    public SuspiciousCaseReportingContext()
    { }

    public SuspiciousCaseReportingContext(DbContextOptions<SuspiciousCaseReportingContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuspiciousCaseReportingContext).Assembly);
    }
}

public class ConfigBasedIntegrationContextFactory : IDesignTimeDbContextFactory<SuspiciousCaseReportingContext>
{
    public SuspiciousCaseReportingContext CreateDbContext(string[] args)
    {
        var migrationConnectionStringName = "IntegrationAdmin";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
            .AddJsonFile($"appsettings.development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<SuspiciousCaseReportingContext>();

        var connectionString = configuration
                                   .GetConnectionString(migrationConnectionStringName)
                               ?? throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

        builder
            .UseNpgsql(connectionString, npgSqlOptions =>
            {
                npgSqlOptions.EnableRetryOnFailure();
                npgSqlOptions.MigrationsHistoryTable(
                    SuspiciousCaseReportingContext.MigrationsTableName,
                    Schema.SuspiciousCases);
            });

        return new SuspiciousCaseReportingContext(builder.Options);
    }
}
