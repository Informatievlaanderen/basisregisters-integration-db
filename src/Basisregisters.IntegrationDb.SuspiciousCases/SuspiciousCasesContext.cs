﻿namespace Basisregisters.IntegrationDb.SuspiciousCases
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

        public DbSet<CurrentAddressWithoutLinkedParcelOrBuildingUnit> CurrentAddressesWithoutLinkedParcelOrBuilding => Set<CurrentAddressWithoutLinkedParcelOrBuildingUnit>();
        public DbSet<ProposedAddressWithoutLinkedParcelOrBuildingUnit> ProposedAddressesWithoutLinkedParcelOrBuilding => Set<ProposedAddressWithoutLinkedParcelOrBuildingUnit>();
        public DbSet<StreetNameLongerThanTwoYearsProposed> StreetNamesLongerThanTwoYearsProposed => Set<StreetNameLongerThanTwoYearsProposed>();
        public DbSet<AddressLongerThanTwoYearsProposed> AddressesLongerThanTwoYearsProposed => Set<AddressLongerThanTwoYearsProposed>();
        public DbSet<CurrentAddressLinkedToProposedStreetName> CurrentAddressesLinkedToProposedStreetName => Set<CurrentAddressLinkedToProposedStreetName>();
        public DbSet<BuildingLongerThanTwoYearsPlanned> BuildingsLongerThanTwoYearsPlanned => Set<BuildingLongerThanTwoYearsPlanned>();
        public DbSet<BuildingUnitsLongerThanTwoYearsPlanned> BuildingUnitsLongerThanTwoYearsPlanned => Set<BuildingUnitsLongerThanTwoYearsPlanned>();
        public DbSet<ActiveAddressOutsideMunicipalityBounds> ActiveAddressesOutsideMunicipalityBounds => Set<ActiveAddressOutsideMunicipalityBounds>();
        public DbSet<CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit> CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit => Set<CurrentAddressWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit>();
        public DbSet<ActiveAddresLinkedToMultipleBuildingUnits> ActiveAddressesLinkedToMultipleBuildingUnits => Set<ActiveAddresLinkedToMultipleBuildingUnits>();
        public DbSet<ActiveBuildingUnitLinkedToMultipleAddresses> ActiveBuildingUnitLinkedToMultipleAddresses => Set<ActiveBuildingUnitLinkedToMultipleAddresses>();
        public DbSet<CurrentAddressLinkedWithBuildingUnitButNotWithParcel> CurrentAddressesLinkedWithBuildingUnitButNotWithParcel => Set<CurrentAddressLinkedWithBuildingUnitButNotWithParcel>();
        public DbSet<RoadSegmentLongerThanTwoYearsWithPermit> RoadSegmentsLongerThanTwoYearsWithPermit => Set<RoadSegmentLongerThanTwoYearsWithPermit>();
        public DbSet<CurrentStreetNameWithoutLinkedRoadSegments> CurrentStreetNamesWithoutLinkedRoadSegments => Set<CurrentStreetNameWithoutLinkedRoadSegments>();
        public DbSet<RoadSegmentWithSingleLinkedStreetName> RoadSegmentsWithSingleLinkedStreetName => Set<RoadSegmentWithSingleLinkedStreetName>();
        public DbSet<RoadSegmentLinkedToRetiredStreetName> RoadSegmentsLinkedToRetiredStreetName => Set<RoadSegmentLinkedToRetiredStreetName>();

        public async Task<IEnumerable<SuspiciousCase>> GetSuspiciousCase(
            SuspiciousCasesType type,
            string nisCode,
            int offset,
            int limit,
            CancellationToken ct)
        {
            switch (type)
            {
                // Address
                case SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits:
                    return await CurrentAddressesWithoutLinkedParcelOrBuilding
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.ProposedAddressWithoutLinkedParcelOrBuildingUnit:
                    return await ProposedAddressesWithoutLinkedParcelOrBuilding
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.ActiveAddressOutsideOfMunicipalityBounds:
                    return await ActiveAddressesOutsideMunicipalityBounds
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.AddressLongerThanTwoYearsProposed:
                    return await AddressesLongerThanTwoYearsProposed
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit:
                    return await CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.ActiveAddressLinkedToMultipleBuildingUnits:
                    return await ActiveAddressesLinkedToMultipleBuildingUnits
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.CurrentAddressLinkedWithBuildingUnitButNotWithParcel:
                    return await CurrentAddressesLinkedWithBuildingUnitButNotWithParcel
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.CurrentAddressLinkedToProposedStreetName:
                    return await CurrentAddressesLinkedToProposedStreetName
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);

                // StreetName
                case SuspiciousCasesType.CurrentStreetNameWithoutLinkedRoadSegment:
                    return await CurrentStreetNamesWithoutLinkedRoadSegments
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed:
                    return await StreetNamesLongerThanTwoYearsProposed
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);

                // RoadSegment
                case SuspiciousCasesType.RoadSegmentLongerThanTwoYearsWithPermit:
                    return await RoadSegmentsLongerThanTwoYearsWithPermit
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.RoadSegmentPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.RoadSegmentWithSingleLinkedStreetName:
                    return await RoadSegmentsWithSingleLinkedStreetName
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.RoadSegmentPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                case SuspiciousCasesType.RoadSegmentLinkedToRetiredStreetName:
                    return await RoadSegmentsLinkedToRetiredStreetName
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.Description) // Contains the StreetNameName
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);

                // Building
                case SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned:
                    return await BuildingsLongerThanTwoYearsPlanned
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.BuildingPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);

                // BuildingUnit
                case SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned:
                    return await BuildingUnitsLongerThanTwoYearsPlanned
                        .Where(x => x.NisCode == nisCode)
                        .OrderBy(x => x.BuildingUnitPersistentLocalId)
                        .Skip(offset)
                        .Take(limit)
                        .ToListAsync(ct);
                // case SuspiciousCasesType.ActiveBuildingUnitWithoutAddress:
                //     return await ActiveBuildingUnitWithoutAddresses
                //         .Where(x => x.NisCode == nisCode)
                //         .OrderBy(x => x.BuildingUnitPersistentLocalId)
                //         .Skip(offset)
                //         .Take(limit)
                //         .ToListAsync(ct);
                // case SuspiciousCasesType.ActiveBuildingUnitsLinkedToMultipleAddresses:
                //     return await ActiveBuildingUnitLinkedToMultipleAddresses
                //         .Where(x => x.NisCode == nisCode)
                //         .OrderBy(x => x.BuildingUnitPersistentLocalId)
                //         .Skip(offset)
                //         .Take(limit)
                //         .ToListAsync(ct);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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
