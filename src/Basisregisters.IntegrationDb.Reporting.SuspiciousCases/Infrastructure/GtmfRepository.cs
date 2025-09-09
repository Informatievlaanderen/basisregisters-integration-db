namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Npgsql;

public interface IGtmfRepository
{
    IEnumerable<MeldingsobjectDto> GetAllMeldingsobjecten();
}

public class GtmfRepository : IGtmfRepository
{
    private readonly string _connectionString;

    public GtmfRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IEnumerable<MeldingsobjectDto> GetAllMeldingsobjecten()
    {
        const string sql = """
                           select
                           mo.meldingsobject_id
                           ,mo.melding_id
                           ,mo.datum_indiening
                           ,mo.datum_vaststelling
                           ,mo.meldingsorganisatie_id_internal
                           ,mo.meldingsorganisatie_id
                           ,mo.referentie_melder
                           ,mo.onderwerp
                           ,mo.beschrijving
                           ,mo.thema
                           ,mo.oorzaak
                           ,mo.thema||' - '||mo.oorzaak as thema_oorzaak
                           ,o.naam as meldingsorganisatie
                           ,replace(replace(o_actie.naam, 'Stad ', ''), 'Gemeente ', '') as actieorganisatie
                           ,(select tijdstip_wijziging from integration_gtmf.meldingsobject_statuswijziging where meldingsobject_id = mo.meldingsobject_id order by tijdstip_wijziging desc limit 1) as datum_wijziging
                           ,(select nieuwe_status from integration_gtmf.meldingsobject_statuswijziging where meldingsobject_id = mo.meldingsobject_id order by tijdstip_wijziging desc limit 1) as meldingstatus
                           from integration_gtmf.meldingsobject mo
                           join integration_gtmf.organisatie o on mo.meldingsorganisatie_id_internal = o.id_internal
                           join integration_gtmf.organisatie o_actie on mo.ovo_code = o_actie.ovo_code
                           order by mo.datum_indiening, mo.meldingsobject_id
                           """;

        using var connection = new NpgsqlConnection(_connectionString);

        connection.Open();
        connection.BeginTransaction(IsolationLevel.Snapshot);

        var meldingsobjecten = new List<MeldingsobjectDto>();

        var batchSize = 5000;
        var offset = 0;

        var moreDataAvailable = true;
        while (moreDataAvailable)
        {
            var batchSql = $"{sql} offset {offset} limit {batchSize}";
            var batch = connection.Query<MeldingsobjectDto>(batchSql).ToList();
            meldingsobjecten.AddRange(batch);

            moreDataAvailable = batch.Count == batchSize;
            offset += batchSize;
        }

        return meldingsobjecten;
    }
}

public class MeldingsobjectDto
{
    public Guid meldingsobject_id { get; init; }
    public Guid melding_id { get; init; }
    public DateTimeOffset datum_indiening { get; init; }
    public DateTimeOffset datum_vaststelling { get; init; }
    public Guid meldingsorganisatie_id_internal { get; init; }
    public Guid meldingsorganisatie_id { get; init; }
    public string referentie_melder { get; init; }
    public string onderwerp { get; init; }
    public string beschrijving { get; init; }
    public string thema { get; init; }
    public string oorzaak { get; init; }
    public string thema_oorzaak { get; init; }
    public string meldingsorganisatie { get; init; }
    public string actieorganisatie { get; init; }
    public DateTimeOffset datum_wijziging { get; init; }
    public string meldingstatus { get; init; }

    // Needed for dapper
    protected MeldingsobjectDto()
    { }
}
