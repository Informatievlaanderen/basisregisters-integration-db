﻿// <auto-generated />
using Basisregisters.IntegrationDb.SuspiciousCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Basisregisters.IntegrationDb.SuspiciousCases.Migrations
{
    [DbContext(typeof(SuspiciousCasesContext))]
    [Migration("20240117152458_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Basisregisters.IntegrationDb.SuspiciousCases.Views.StreetNamesLongerThanTwoYearsProposed", b =>
                {
                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("NisCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nis_code");

                    b.Property<string>("PersistentLocalId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("persistent_local_id");

                    b.Property<int>("StreetNamePersistentLocalId")
                        .HasColumnType("integer")
                        .HasColumnName("streetname_persistent_local_id");

                    b.ToView("view_streetname_longer_than_two_years_proposed", "integration_suspicious_cases");

                    b.ToSqlQuery("\r\n                            SELECT\r\n                                persistent_local_id,\r\n                                streetname_persistent_local_id,\r\n                                nis_code,\r\n                                description\r\n                            FROM  integration_suspicious_cases.view_streetname_longer_than_two_years_proposed");
                });

            modelBuilder.Entity("Basisregisters.IntegrationDb.SuspiciousCases.Views.SuspiciousCaseCount", b =>
                {
                    b.Property<int>("Count")
                        .HasColumnType("integer")
                        .HasColumnName("count");

                    b.Property<string>("NisCode")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("nis_code");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.ToView("view_suspicious_cases_counts", "integration_suspicious_cases");

                    b.ToSqlQuery("select\r\n                                nis_code,\r\n                                count,\r\n                                type\r\n                            FROM integration_suspicious_cases.view_suspicious_cases_counts");
                });
#pragma warning restore 612, 618
        }
    }
}
