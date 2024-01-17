﻿// <auto-generated />
using System;
using Basisregisters.IntegrationDb.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    [DbContext(typeof(SuspiciousCasesContext))]
    partial class SuspiciousCasesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.BuildingUnitAddressRelations", b =>
                {
                    b.Property<string>("AddressPersistentLocalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("BuildingUnitPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("BuildingUnitAddressRelations", "integration");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                \"BuildingUnitPersistentLocalId\",\r\n                                \"AddressPersistentLocalId\",\r\n                                 \"Timestamp\"\r\n                            FROM  \"integration\".\"VIEW_BuildingUnitAddressRelations\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.ParcelAddressRelations", b =>
                {
                    b.Property<int?>("AddressPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("CaPaKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasIndex("AddressPersistentLocalId");

                    b.HasIndex("CaPaKey");

                    b.ToView("ParcelAddressRelations", "integration");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                \"CaPaKey\",\r\n                                \"AddressPersistentLocalId\",\r\n                                \"Timestamp\"\r\n                            FROM  \"integration\".\"VIEW_ParcelAddressRelations\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.ActiveBuildingUnitWithoutAddress", b =>
                {
                    b.Property<int>("BuildingUnitPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("ActiveBuildingUnitWithoutAddress", "integration");

                    b.ToSqlQuery("SELECT\r\n                                \"BuildingUnitPersistentLocalId\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                                FROM  \"integration\".\"VIEW_ActiveBuildingUnitWithoutAddress\"");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.AddressesLinkedToMultipleBuildingUnits", b =>
                {
                    b.Property<int>("AddressPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<int>("LinkedBuildingUnitCount")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("AddressesLinkedToMultipleBuildingUnits", "integration");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                \"AddressPersistentLocalId\",\r\n                                \"LinkedBuildingUnitCount\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                            FROM \"integration\".\"VIEW_AddressesLinkedToMultipleBuildingUnits\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.BuildingUnitLinkedToMultipleAddresses", b =>
                {
                    b.Property<int>("BuildingUnitPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("BuildingUnitLinkedToMultipleAddresses", "integration");

                    b.ToSqlQuery("SELECT\r\n                                \"BuildingUnitPersistentLocalId\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                                FROM  \"integration\".\"VIEW_BuildingUnitLinkedToMultipleAddresses\"");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.CurrentAddressesOutsideMunicipalityBounds", b =>
                {
                    b.Property<int>("AddressPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("CurrentAddressesOutsideMunicipalityBounds", "integration");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                \"AddressPersistentLocalId\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                            FROM  \"integration\".\"VIEW_CurrentAddressesOutsideMunicipalityBounds\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.CurrentAddressWithoutLinkedParcelOrBuildingUnit", b =>
                {
                    b.Property<int>("AddressPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("CurrentAddressWithoutLinkedParcelOrBuildingUnit", "integration");

                    b.ToSqlQuery("SELECT\r\n                                \"AddressPersistentLocalId\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                                FROM  \"integration\".\"VIEW_CurrentAddressWithoutLinkedParcelOrBuildingUnit\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.CurrentStreetNameWithoutLinkedRoadSegments", b =>
                {
                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<int>("StreetNamePersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("CurrentStreetNameWithoutLinkedRoadSegments", "integration");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                \"StreetNamePersistentLocalId\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                            FROM  \"integration\".\"VIEW_CurrentStreetNameWithoutLinkedRoadSegments\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.ProposedAddressWithoutLinkedParcelOrBuildingUnit", b =>
                {
                    b.Property<int>("AddressPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.ToView("ProposedAddressWithoutLinkedParcelOrBuildingUnit", "integration");

                    b.ToSqlQuery("SELECT\r\n                                \"AddressPersistentLocalId\",\r\n                                \"NisCode\",\r\n                                \"Timestamp\"\r\n                                FROM  \"integration\".\"VIEW_ProposedAddressWithoutLinkedParcelOrBuildingUnit\" ");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.StreetNamesLongerThanTwoYearsProposed", b =>
                {
                    b.Property<string>("NisCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nis_code");

                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer")
                        .HasColumnName("streetname_persistent_local_id");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.ToView("view_streetname_longer_than_two_years_proposed", "integration_suspicious_cases");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                streetname_persistent_local_id,\r\n                                nis_code,\r\n                                timestamp\r\n                            FROM  integration_suspicious_cases.view_streetname_longer_than_two_years_proposed");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases.SuspiciousCaseListItem", b =>
                {
                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.ToView("SuspiciousCaseListItem", "integration");

                    b.ToSqlQuery("select\r\n                                \"NisCode\"\r\n                                ,\"Count\"\r\n                                ,\"Type\"\r\n                            FROM \"integration\".\"VIEW_SuspiciousCaseListItemConfiguration\"");
                });
#pragma warning restore 612, 618
        }
    }
}