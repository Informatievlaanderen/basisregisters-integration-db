namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases;

using System;
using IntegrationDb.SuspiciousCases;
using IntegrationDb.SuspiciousCases.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SuspiciousCase
{
    public string NisCode { get; set; }
    public string ObjectId { get; set; }
    public Category ObjectType { get; set; }
    public SuspiciousCasesType SuspiciousCaseType { get; set; }
    public DateOnly DateAdded { get; set; }
    public DateOnly? DateClosed { get; set; }
    public SuspiciousCaseStatus Status { get; set; }

    public SuspiciousCase(
        string nisCode,
        string objectId,
        Category objectType,
        SuspiciousCasesType suspiciousCaseType,
        DateOnly dateAdded)
    {
        NisCode = nisCode;
        ObjectId = objectId;
        ObjectType = objectType;
        SuspiciousCaseType = suspiciousCaseType;
        DateAdded = dateAdded;
        Status = SuspiciousCaseStatus.Open;
    }

    public void Solve(DateOnly dateClosed)
    {
        if(DateClosed is not null)
            throw new InvalidOperationException("Cannot solve a case that is already closed.");

        DateClosed = dateClosed;
        Status = SuspiciousCaseStatus.Solved;
    }

    public void CheckOff(DateOnly dateClosed)
    {
        DateClosed = dateClosed;
        Status = SuspiciousCaseStatus.Checked;
    }

    public void Reopen()
    {
        DateAdded = DateOnly.FromDateTime(DateTime.Now);
        DateClosed = null;
        Status = SuspiciousCaseStatus.Open;
    }
}

public sealed class SuspiciousCaseConfiguration : IEntityTypeConfiguration<SuspiciousCase>
{
    public const string TableName = "reporting_suspicious_cases";

    public void Configure(EntityTypeBuilder<SuspiciousCase> builder)
    {
        builder.ToTable(TableName, Schema.SuspiciousCases)
            .HasKey(x => new { x.NisCode, x.ObjectId, x.SuspiciousCaseType });

        builder
            .Property(x => x.NisCode)
            .HasMaxLength(5)
            .HasColumnName("nis_code")
            .IsRequired();

        builder
            .Property(x => x.ObjectId)
            .HasMaxLength(20)
            .HasColumnName("object_id")
            .IsRequired();

        builder
            .Property(x => x.ObjectType)
            .HasConversion<string>()
            .HasColumnName("object_type")
            .IsRequired();

        builder
            .Property(x => x.SuspiciousCaseType)
            .HasConversion<string>()
            .HasColumnName("suspicious_case_type")
            .IsRequired();

        builder
            .Property(x => x.DateAdded)
            .HasColumnName("date_added")
            .IsRequired();

        builder
            .Property(x => x.DateClosed)
            .HasColumnName("date_closed")
            .IsRequired(false);

        builder
            .Property(x => x.Status)
            .HasConversion<string>()
            .HasColumnName("status")
            .IsRequired();

        builder.HasIndex(x => x.NisCode);
        builder.HasIndex(x => x.SuspiciousCaseType);
        builder.HasIndex(x => new { x.ObjectId, x.ObjectType });
        builder.HasIndex(x => x.Status);
    }
}

public enum SuspiciousCaseStatus
{
    Unknown = 0,
    Open = 1,
    Solved = 2,
    Checked = 3,
}
