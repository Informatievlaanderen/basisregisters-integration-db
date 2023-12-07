namespace Basisregisters.IntegrationDb.Schema
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Models.Views;

    public class IntegrationContext : DbContext
    {
        public const string Schema = "Integration";
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";
        public const string GeomFromGmlComputedQuery = "ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ";
        public const string GeomFromEwkbComputedQuery = $"ST_AsHEXEWKB(ST_GeomFromText(\"GeometryAsWkt\"))";

        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<StreetName> StreetNames { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<BuildingUnit> BuildingUnits { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<PostInfo> PostInfo { get; set; }
        public DbSet<RoadSegment> RoadSegments { get; set; }
        public DbSet<ParcelAddressRelations> ParcelAddressRelations { get; set; }
        public DbSet<BuildingUnitAddressRelations> BuildingUnitAddressRelations { get; set; }
        public DbSet<ActiveAddressWithoutLinkedParcelOrBuildingUnits> ActiveAddressWithoutParcelOrBuildingUnitRelations { get; set; }

        public IntegrationContext() { }

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
