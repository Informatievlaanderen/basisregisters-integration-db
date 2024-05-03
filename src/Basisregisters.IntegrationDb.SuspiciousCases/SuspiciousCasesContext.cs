namespace Basisregisters.IntegrationDb.SuspiciousCases
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Views;

    public class SuspiciousCasesContext : DbContext
    {
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";

        public DbSet<SuspiciousCaseCount> SuspiciousCaseCounts => Set<SuspiciousCaseCount>();

        public DbSet<CurrentAddressWithoutLinkedParcelOrBuildingUnit> CurrentAddressWithoutLinkedParcelOrBuilding => Set<CurrentAddressWithoutLinkedParcelOrBuildingUnit>();
        public DbSet<ProposedAddressWithoutLinkedParcelOrBuildingUnit> ProposedAddressWithoutLinkedParcelOrBuilding => Set<ProposedAddressWithoutLinkedParcelOrBuildingUnit>();
        public DbSet<StreetNamesLongerThanTwoYearsProposed> StreetNamesLongerThanTwoYearsProposed => Set<StreetNamesLongerThanTwoYearsProposed>();
        public DbSet<AddressLongerThanTwoYearsProposed> AddressesLongerThanTwoYearsProposed => Set<AddressLongerThanTwoYearsProposed>();
        public DbSet<BuildingsLongerThanTwoYearsPlanned> BuildingsLongerThanTwoYearsPlanned => Set<BuildingsLongerThanTwoYearsPlanned>();
        public DbSet<BuildingUnitsLongerThanTwoYearsPlanned> BuildingUnitsLongerThanTwoYearsPlanned => Set<BuildingUnitsLongerThanTwoYearsPlanned>();
        public DbSet<ActiveAddressOutsideMunicipalityBounds> CurrentAddressesOutsideMunicipalityBounds => Set<ActiveAddressOutsideMunicipalityBounds>();
        public DbSet<CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit> CurrentAddressesWithSpecificationDerivedFromObjectWithoutBuildingUnits => Set<CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit>();
        public DbSet<ActiveBuildingUnitWithoutAddress> ActiveBuildingUnitWithoutAddresses => Set<ActiveBuildingUnitWithoutAddress>();
        public DbSet<ActiveAddresLinkedToMultipleBuildingUnits> AddressesLinkedToMultipleBuildingUnits => Set<ActiveAddresLinkedToMultipleBuildingUnits>();
        public DbSet<ActiveBuildingUnitLinkedToMultipleAddresses> ActiveBuildingUnitLinkedToMultipleAddresses => Set<ActiveBuildingUnitLinkedToMultipleAddresses>();
        public DbSet<CurrentAddressLinkedWithBuildingUnitButNotWithParcel> AddressesLinkedWithBuildingUnitButNotWithParcel => Set<CurrentAddressLinkedWithBuildingUnitButNotWithParcel>();

        public async Task<IEnumerable<SuspiciousCase>> GetSuspiciousCase(
            SuspiciousCasesType type,
            string nisCode,
            int offset,
            int limit,
            CancellationToken ct)
        {
            switch (type)
            {
                case SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits:
                    return await CurrentAddressWithoutLinkedParcelOrBuilding
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.ProposedAddressWithoutLinkedParcelsOrBuildingUnits:
                    return await ProposedAddressWithoutLinkedParcelOrBuilding
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.CurrentAddressesOutsideOfMunicipalityBounds:
                    return await CurrentAddressesOutsideMunicipalityBounds
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.CurrentStreetNamesWithoutRoadSegment:
                    break;
                case SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed:
                    return await StreetNamesLongerThanTwoYearsProposed
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.StreetNamePersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.AddressesLongerThanTwoYearsProposed:
                    return await AddressesLongerThanTwoYearsProposed
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.RoadSegmentsLongerThanTwoYearsProposed:
                    break;
                case SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned:
                    return await BuildingsLongerThanTwoYearsPlanned
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.BuildingPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned:
                    return await BuildingUnitsLongerThanTwoYearsPlanned
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.BuildingUnitPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.StreetNameWithOnlyOneRoadSegmentToOnlyOneSide:
                    break;
                case SuspiciousCasesType.CurrentAddressesWithSpecificationDerivedFromObjectWithoutBuildingUnit:
                    return await CurrentAddressesWithSpecificationDerivedFromObjectWithoutBuildingUnits
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.ActiveBuildingUnitsWithoutAddress:
                    return await ActiveBuildingUnitWithoutAddresses
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.BuildingUnitPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                // case SuspiciousCasesType.ActiveBuildingUnitsLinkedToMultipleAddresses:
                //     return await ActiveBuildingUnitLinkedToMultipleAddresses
                //         .Where(x => x.NisCode == nisCode)
                //         .OrderBy(x => x.BuildingUnitPersistentLocalId)
                //         .Skip(offset)
                //         .Take(limit)
                //         .ToListAsync(ct);
                case SuspiciousCasesType.AddressesLinkedToMultipleBuildingUnits:
                    return await AddressesLinkedToMultipleBuildingUnits
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.AddressLinkedWithBuildingUnitButNotWithParcel:
                    return await AddressesLinkedWithBuildingUnitButNotWithParcel
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.AddressPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        public SuspiciousCasesContext()
        { }

        public SuspiciousCasesContext(DbContextOptions<SuspiciousCasesContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SuspiciousCasesContext).Assembly);
        }

        // public void RefreshView(string viewName)
        // {
        //     Database.ExecuteSqlRaw(@$"REFRESH MATERIALIZED VIEW ""{Schema}"".""{viewName}"";");
        // }
    }

    public class ConfigBasedIntegrationContextFactory : IDesignTimeDbContextFactory<SuspiciousCasesContext>
    {
        public SuspiciousCasesContext CreateDbContext(string[] args)
        {
            var migrationConnectionStringName = "IntegrationAdmin";

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
                        Schema.SuspiciousCases);
                    npgSqlOptions.UseNetTopologySuite();
                    npgSqlOptions.CommandTimeout(260);
                });

            return new SuspiciousCasesContext(builder.Options);
        }
    }
}
