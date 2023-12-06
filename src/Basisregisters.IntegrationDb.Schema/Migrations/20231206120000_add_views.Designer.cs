﻿// <auto-generated />
using System;
using Basisregisters.IntegrationDb.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    [DbContext(typeof(IntegrationContext))]
    [Migration("20231206120000_add_views")]
    partial class add_views
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Address", b =>
                {
                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("BoxNumber")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("geometry")
                        .HasComputedColumnSql("ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", true);

                    b.Property<string>("GeometryGml")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HouseNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsOfficiallyAssigned")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NisCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PositionMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PositionSpecification")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PostalCode")
                        .HasColumnType("text");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StreetNamePersistentLocalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PersistentLocalId");

                    b.HasIndex("BoxNumber");

                    b.HasIndex("Geometry");

                    b.HasIndex("HouseNumber");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("NisCode");

                    b.HasIndex("PostalCode");

                    b.HasIndex("Status");

                    b.HasIndex("StreetNamePersistentLocalId");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("Addresses", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Building", b =>
                {
                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("geometry")
                        .HasComputedColumnSql("ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", true);

                    b.Property<string>("GeometryGml")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GeometryMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PersistentLocalId");

                    b.HasIndex("Geometry");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("Buildings", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.BuildingUnit", b =>
                {
                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("Addresses")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("BuildingPersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("Function")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Geometry>("Geometry")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("geometry")
                        .HasComputedColumnSql("ST_GeomFromGML(REPLACE(\"GeometryGml\",'https://www.opengis.net/def/crs/EPSG/0/', 'EPSG:')) ", true);

                    b.Property<string>("GeometryGml")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("GeometryMethod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("HasDeviation")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PersistentLocalId");

                    b.HasIndex("Geometry");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("BuildingUnits", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Municipality", b =>
                {
                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<bool>("FacilityLanguageDutch")
                        .HasColumnType("boolean");

                    b.Property<bool>("FacilityLanguageEnglish")
                        .HasColumnType("boolean");

                    b.Property<bool>("FacilityLanguageFrench")
                        .HasColumnType("boolean");

                    b.Property<bool>("FacilityLanguageGerman")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("NameDutch")
                        .HasColumnType("text");

                    b.Property<string>("NameEnglish")
                        .HasColumnType("text");

                    b.Property<string>("NameFrench")
                        .HasColumnType("text");

                    b.Property<string>("NameGerman")
                        .HasColumnType("text");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("OfficialLanguageDutch")
                        .HasColumnType("boolean");

                    b.Property<bool>("OfficialLanguageEnglish")
                        .HasColumnType("boolean");

                    b.Property<bool>("OfficialLanguageFrench")
                        .HasColumnType("boolean");

                    b.Property<bool>("OfficialLanguageGerman")
                        .HasColumnType("boolean");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("NisCode");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("NameDutch");

                    b.HasIndex("NameEnglish");

                    b.HasIndex("NameFrench");

                    b.HasIndex("NameGerman");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("Municipalities", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Parcel", b =>
                {
                    b.Property<string>("CaPaKey")
                        .HasColumnType("text");

                    b.Property<string>("Addresses")
                        .HasColumnType("text");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("CaPaKey");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("Parcels", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.PostInfo", b =>
                {
                    b.Property<string>("PostalCode")
                        .HasColumnType("text");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<string>("PostalNameDutch")
                        .HasColumnType("text");

                    b.Property<string>("PostalNameFrench")
                        .HasColumnType("text");

                    b.Property<string>("PostalNameGerman")
                        .HasColumnType("text");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PostalCode");

                    b.HasIndex("NisCode");

                    b.HasIndex("PostalNameDutch");

                    b.HasIndex("PostalNameFrench");

                    b.HasIndex("PostalNameGerman");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("PostInfo", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.StreetName", b =>
                {
                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("HomonymAdditionDutch")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HomonymAdditionFrench")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HomonymAdditionGerman")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean");

                    b.Property<string>("NameDutch")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NameFrench")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NameGerman")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Namespace")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

                    b.Property<string>("PuriId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VersionString")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("VersionTimestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("PersistentLocalId");

                    b.HasIndex("IsRemoved");

                    b.HasIndex("NameDutch");

                    b.HasIndex("NameFrench");

                    b.HasIndex("NameGerman");

                    b.HasIndex("NisCode");

                    b.HasIndex("Status");

                    b.HasIndex("VersionTimestamp");

                    b.ToTable("StreetNames", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.AddressesLinkedToMultipleBuildingUnits", b =>
                {
                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToView("AddressesLinkedToMultipleBuildingUnits", "Integration");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Schema.Models.Views.ParcelsLinkedToMultipleHouseNumbers", b =>
                {
                    b.Property<string>("Addresses")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CaPaKey")
                        .HasColumnType("integer");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToView("ParcelsLinkedToMultipleHouseNumbers", "Integration");
                });
#pragma warning restore 612, 618
        }
    }
}