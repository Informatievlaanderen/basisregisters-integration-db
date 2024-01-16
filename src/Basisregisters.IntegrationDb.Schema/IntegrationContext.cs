namespace Basisregisters.IntegrationDb.Schema
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Models.Views;
    using Models.Views.SuspiciousCases;

    public class IntegrationContext : DbContext
    {
        public const string Schema = "Integration";
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";

        public DbSet<SuspiciousCaseListItem> SuspiciousCaseListItems { get; set; }

        public DbSet<BuildingUnitAddressRelations> BuildingUnitAddressRelations { get; set; }
        public DbSet<ParcelAddressRelations> ParcelAddressRelations { get; set; }

        public DbSet<CurrentAddressWithoutLinkedParcelOrBuildingUnit> CurrentAddressWithoutLinkedParcelOrBuildingUnits { get; set; }
        public DbSet<ProposedAddressWithoutLinkedParcelOrBuildingUnit> ProposedAddressWithoutLinkedParcelOrBuildingUnits { get; set; }
        public DbSet<AddressesLinkedToMultipleBuildingUnits> AddressesLinkedToMultipleBuildingUnits { get; set; }
        public DbSet<CurrentAddressesOutsideMunicipalityBounds> CurrentAddressesOutsideMunicipalityBounds { get; set; }
        public DbSet<CurrentStreetNameWithoutLinkedRoadSegments> CurrentStreetNameWithoutLinkedRoadSegments { get; set; }

        public IntegrationContext()
        { }

        public IntegrationContext(DbContextOptions<IntegrationContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationContext).Assembly);
        }

        public void RefreshView(string viewName)
        {
            Database.ExecuteSqlRaw(@$"REFRESH MATERIALIZED VIEW ""{Schema}"".""{viewName}"";");
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
                    npgSqlOptions.CommandTimeout(260);
                });

            return new IntegrationContext(builder.Options);
        }
    }
}
