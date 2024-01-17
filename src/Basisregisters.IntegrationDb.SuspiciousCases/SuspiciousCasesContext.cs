namespace Basisregisters.IntegrationDb.Schema
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Models.Views.SuspiciousCases;

    public class SuspiciousCasesContext : DbContext
    {
        public const string Schema = "integration";
        public const string SchemaSuspiciousCases = "integration_suspicious_cases";
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";

        public DbSet<SuspiciousCaseListItem> SuspiciousCaseListItems { get; set; }



        // public DbSet<BuildingUnitAddressRelations> BuildingUnitAddressRelations { get; set; }
        // public DbSet<ParcelAddressRelations> ParcelAddressRelations { get; set; }
        //
        // public DbSet<CurrentAddressWithoutLinkedParcelOrBuildingUnit> CurrentAddressWithoutLinkedParcelOrBuildingUnits { get; set; }
        // public DbSet<ProposedAddressWithoutLinkedParcelOrBuildingUnit> ProposedAddressWithoutLinkedParcelOrBuildingUnits { get; set; }
        // public DbSet<AddressesLinkedToMultipleBuildingUnits> AddressesLinkedToMultipleBuildingUnits { get; set; }
        // public DbSet<CurrentAddressesOutsideMunicipalityBounds> CurrentAddressesOutsideMunicipalityBounds { get; set; }
        // public DbSet<CurrentStreetNameWithoutLinkedRoadSegments> CurrentStreetNameWithoutLinkedRoadSegments { get; set; }

        public SuspiciousCasesContext()
        { }

        public SuspiciousCasesContext(DbContextOptions<SuspiciousCasesContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuspiciousCasesContext).Assembly);
        }

        public void RefreshView(string viewName)
        {
            Database.ExecuteSqlRaw(@$"REFRESH MATERIALIZED VIEW ""{Schema}"".""{viewName}"";");
        }
    }

    public class ConfigBasedIntegrationContextFactory : IDesignTimeDbContextFactory<SuspiciousCasesContext>
    {
        public SuspiciousCasesContext CreateDbContext(string[] args)
        {
            var migrationConnectionStringName = "IntegrationDbAdmin";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .AddJsonFile($"appsettings.development.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<SuspiciousCasesContext>();

            var connectionString = configuration
                                    .GetConnectionString(migrationConnectionStringName)
                                    ?? throw new InvalidOperationException($"Could not find a connection string with name '{migrationConnectionStringName}'");

            builder
                .UseNpgsql(connectionString, npgSqlOptions =>
                {
                    npgSqlOptions.EnableRetryOnFailure();
                    npgSqlOptions.MigrationsHistoryTable(
                        SuspiciousCasesContext.MigrationsTableName,
                        SuspiciousCasesContext.Schema);
                    npgSqlOptions.UseNetTopologySuite();
                    npgSqlOptions.CommandTimeout(260);
                });

            return new SuspiciousCasesContext(builder.Options);
        }
    }
}
