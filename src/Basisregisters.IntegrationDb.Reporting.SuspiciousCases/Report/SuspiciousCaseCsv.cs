namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases.Report;

using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

public sealed class SuspiciousCaseCsv
{
    public int NisCode { get; set; }
    public long ObjectId { get; set; }
    public string ObjectType { get; set; }
    public int VgType { get; set; }
    public string Gemeente { get; set; }
    public DateTime Added { get; set; }
    public DateTime? Closed { get; set; }
    public string Status { get; set; }

    public SuspiciousCaseCsv(
        int nisCode,
        long objectId,
        string objectType,
        int vgType,
        string gemeente,
        DateTime added,
        DateTime? closed,
        string status)
    {
        NisCode = nisCode;
        ObjectId = objectId;
        ObjectType = objectType;
        VgType = vgType;
        Gemeente = gemeente;
        Added = added;
        Closed = closed;
        Status = status;
    }
}

public sealed class SuspiciousCaseCsvMap : ClassMap<SuspiciousCaseCsv>
{
    public SuspiciousCaseCsvMap()
    {
        Map(m => m.NisCode).Name("nis_code");
        Map(m => m.ObjectId).Name("object_id");
        Map(m => m.ObjectType).Name("object_type");
        Map(m => m.VgType).Name("vg_type");
        Map(m => m.Gemeente).Name("Gemeentenaam");

        Map(m => m.Added)
            .Name("Datum toegevoegd")
            .TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeLocal)
            .TypeConverterOption.Format("dd/MM/yyyy");

        Map(m => m.Closed)
            .Name("Datum gesloten")
            .TypeConverterOption.DateTimeStyles(DateTimeStyles.AssumeLocal)
            .TypeConverterOption.Format("dd/MM/yyyy");

        Map(m => m.Status).Name("Status");
    }
}
