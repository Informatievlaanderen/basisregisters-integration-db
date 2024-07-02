﻿// <auto-generated />
using System;
using Basisregisters.IntegrationDb.Gtmf.Meldingen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Migrations
{
    [DbContext(typeof(MeldingenContext))]
    partial class MeldingenContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Basisregisters.IntegrationDb.Gtmf.Meldingen.Meldingsobject", b =>
                {
                    b.Property<Guid>("MeldingsobjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("meldingsobject_id");

                    b.Property<string>("Beschrijving")
                        .HasColumnType("text")
                        .HasColumnName("beschrijving");

                    b.Property<DateTimeOffset>("DatumIndieningAsDateTimeOffset")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datum_indiening");

                    b.Property<string>("DatumIndieningAsString")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("datum_indiening_as_string");

                    b.Property<DateTimeOffset>("DatumVaststellingAsDateTimeOffset")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("datum_vaststelling");

                    b.Property<string>("DatumVaststellingAsString")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("datum_vaststelling_as_string");

                    b.Property<Geometry>("Geometrie")
                        .HasColumnType("geometry")
                        .HasColumnName("geometrie");

                    b.Property<Guid>("MeldingId")
                        .HasColumnType("uuid")
                        .HasColumnName("melding_id");

                    b.Property<string>("Meldingsapplicatie")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("meldingsapplicatie");

                    b.Property<Guid>("MeldingsorganisatieId")
                        .HasColumnType("uuid")
                        .HasColumnName("meldingsorganisatie_id");

                    b.Property<Guid>("MeldingsorganisatieIdInternal")
                        .HasColumnType("uuid")
                        .HasColumnName("meldingsorganisatie_id_internal");

                    b.Property<string>("Onderwerp")
                        .HasColumnType("text")
                        .HasColumnName("onderwerp");

                    b.Property<string>("Oorzaak")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("oorzaak");

                    b.Property<string>("OvoCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("ovo_code");

                    b.Property<string>("Referentie")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("referentie");

                    b.Property<string>("ReferentieMelder")
                        .HasColumnType("text")
                        .HasColumnName("referentie_melder");

                    b.Property<string>("Samenvatting")
                        .HasColumnType("text")
                        .HasColumnName("samenvatting");

                    b.Property<string>("Thema")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("thema");

                    b.HasKey("MeldingsobjectId");

                    b.HasIndex("MeldingId");

                    b.HasIndex("Meldingsapplicatie");

                    b.HasIndex("MeldingsorganisatieId");

                    b.HasIndex("MeldingsorganisatieIdInternal");

                    b.HasIndex("Onderwerp");

                    b.HasIndex("Oorzaak");

                    b.HasIndex("OvoCode");

                    b.HasIndex("Referentie");

                    b.HasIndex("ReferentieMelder");

                    b.HasIndex("Thema");

                    b.ToTable("meldingsobject", "integration_gtmf");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Gtmf.Meldingen.MeldingsobjectStatuswijziging", b =>
                {
                    b.Property<Guid>("MeldingsobjectId")
                        .HasColumnType("uuid")
                        .HasColumnName("meldingsobject_id");

                    b.Property<string>("NieuweStatus")
                        .HasColumnType("text")
                        .HasColumnName("nieuwe_status");

                    b.Property<Guid>("MeldingId")
                        .HasColumnType("uuid")
                        .HasColumnName("melding_id");

                    b.Property<Guid>("OrganisatieIdInternal")
                        .HasColumnType("uuid")
                        .HasColumnName("organisatie_id_internal");

                    b.Property<string>("OudeStatus")
                        .HasColumnType("text")
                        .HasColumnName("oude_status");

                    b.Property<DateTimeOffset>("TijdstipWijzigingAsDateTimeOffset")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("tijdstip_wijziging");

                    b.Property<string>("TijdstipWijzigingAsString")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tijdstip_wijziging_as_string");

                    b.Property<string>("Toelichting")
                        .HasColumnType("text")
                        .HasColumnName("toelichting");

                    b.HasKey("MeldingsobjectId", "NieuweStatus");

                    b.HasIndex("MeldingId");

                    b.HasIndex("NieuweStatus");

                    b.HasIndex("OrganisatieIdInternal");

                    b.HasIndex("OudeStatus");

                    b.ToTable("meldingsobject_statuswijziging", "integration_gtmf");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Gtmf.Meldingen.Organisatie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("IdInternal")
                        .HasColumnType("uuid")
                        .HasColumnName("id_internal");

                    b.Property<string>("KboNummer")
                        .HasColumnType("text")
                        .HasColumnName("kbo_nummer");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("naam");

                    b.Property<string>("OvoCode")
                        .HasColumnType("text")
                        .HasColumnName("ovo_code");

                    b.HasKey("Id");

                    b.HasIndex("IdInternal");

                    b.HasIndex("KboNummer");

                    b.HasIndex("OvoCode");

                    b.ToTable("organisatie", "integration_gtmf");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.Gtmf.Meldingen.ProjectionState", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("naam");

                    b.Property<int>("Position")
                        .HasColumnType("integer")
                        .HasColumnName("position");

                    b.HasKey("Name");

                    b.ToTable("projection_state", "integration_gtmf");
                });
#pragma warning restore 612, 618
        }
    }
}
