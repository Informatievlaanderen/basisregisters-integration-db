﻿// <auto-generated />
using System;
using Basisregisters.Integration.Db.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Basisregisters.Integration.Db.Schema.Migrations
{
    [DbContext(typeof(IntegrationContext))]
    [Migration("20231117100404_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Basisregisters.Integration.Db.Schema.Models.Address", b =>
                {
                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

                    b.Property<string>("BoxNumber")
                        .HasColumnType("text");

                    b.Property<string>("FullNameDutch")
                        .HasColumnType("text");

                    b.Property<string>("FullNameEnglish")
                        .HasColumnType("text");

                    b.Property<string>("FullNameFrench")
                        .HasColumnType("text");

                    b.Property<string>("FullNameGerman")
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

                    b.Property<int>("NisCode")
                        .HasColumnType("integer");

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

                    b.Property<string>("VerionString")
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

            modelBuilder.Entity("Basisregisters.Integration.Db.Schema.Models.Building", b =>
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

                    b.Property<string>("VerionString")
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

            modelBuilder.Entity("Basisregisters.Integration.Db.Schema.Models.BuildingUnit", b =>
                {
                    b.Property<int>("PersistentLocalId")
                        .HasColumnType("integer");

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

                    b.Property<string>("VerionString")
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

            modelBuilder.Entity("Basisregisters.Integration.Db.Schema.Models.Municipality", b =>
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

                    b.Property<string>("VerionString")
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

            modelBuilder.Entity("Basisregisters.Integration.Db.Schema.Models.PostInfo", b =>
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

                    b.Property<string>("VerionString")
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

            modelBuilder.Entity("Basisregisters.Integration.Db.Schema.Models.StreetName", b =>
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

                    b.Property<string>("VerionString")
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
#pragma warning restore 612, 618
        }
    }
}
