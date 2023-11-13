﻿namespace Basisregisters.Integration.Db.Schema
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class IntegrationContext : DbContext
    {
        public const string Schema = "Integration";
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";

        public DbSet<Municipalities> Municipalities { get; set; }

        public IntegrationContext() { }

        public IntegrationContext(DbContextOptions<IntegrationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationContext).Assembly);
        }

    }

    public class ConfigBasedIntegrationContextFactory : IDesignTimeDbContextFactory<IntegrationContext>
    {
        public IntegrationContext CreateDbContext(string[] args)
        {
            var migrationConnectionStringName = "IntegrationDbAdmin";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .AddJsonFile($"appsettings.development.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<IntegrationContext>();

            var connectionString = configuration
                                    .GetConnectionString(migrationConnectionStringName)
                                    ?? throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

            builder
                .UseNpgsql(connectionString, npgSqlOptions =>
                {
                    npgSqlOptions.EnableRetryOnFailure();
                    npgSqlOptions.MigrationsHistoryTable(
                        IntegrationContext.MigrationsTableName,
                        IntegrationContext.Schema);
                    npgSqlOptions.UseNetTopologySuite();
                });

            return new IntegrationContext(builder.Options);
        }
    }
}
