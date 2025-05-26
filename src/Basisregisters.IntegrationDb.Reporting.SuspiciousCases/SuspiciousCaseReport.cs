namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases;

using System;
using IntegrationDb.SuspiciousCases;
using IntegrationDb.SuspiciousCases.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SuspiciousCaseReport
{
    public string NisCode { get; set; }
    public SuspiciousCasesType SuspiciousCaseType { get; set; }
    public DateOnly Month { get; set; }
    public int OpenCases { get; set; }
    public int ClosedCases { get; set; }

    public SuspiciousCaseReport(
        string nisCode,
        SuspiciousCasesType suspiciousCaseType,
        DateOnly month,
        int openCases = 0,
        int closedCases = 0)
    {
        NisCode = nisCode;
        SuspiciousCaseType = suspiciousCaseType;
        Month = month;
        OpenCases = openCases;
        ClosedCases = closedCases;
    }
}

public sealed class SuspiciousCaseReportConfiguration : IEntityTypeConfiguration<SuspiciousCaseReport>
{
    public const string TableName = "suspicious_case_reports";

    public void Configure(EntityTypeBuilder<SuspiciousCaseReport> builder)
    {
        builder
            .ToTable(TableName, Schema.SuspiciousCases)
            .HasKey(x => new { x.NisCode, x.SuspiciousCaseType, x.Month });

        builder
            .Property(x => x.NisCode)
            .HasMaxLength(5)
            .HasColumnName("nis_code")
            .IsRequired();

        builder
            .Property(x => x.SuspiciousCaseType)
            .HasConversion<string>()
            .HasColumnName("suspicious_case_type")
            .IsRequired();

        builder
            .Property(x => x.Month)
            .HasColumnType("date")
            .HasColumnName("month")
            .IsRequired();

        builder
            .Property(x => x.OpenCases)
            .HasColumnName("open_cases")
            .IsRequired()
            .HasDefaultValue(0);

        builder
            .Property(x => x.ClosedCases)
            .HasColumnName("closed_cases")
            .IsRequired()
            .HasDefaultValue(0);
    }
}
