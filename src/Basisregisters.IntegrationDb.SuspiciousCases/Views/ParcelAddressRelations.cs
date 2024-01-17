// namespace Basisregisters.IntegrationDb.Schema.Views
// {
//     using System;
//     using Microsoft.EntityFrameworkCore;
//     using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
//     public class ParcelAddressRelations
//     {
//         public string CaPaKey { get; set; }
//         public int? AddressPersistentLocalId { get; set; }
//         public DateTimeOffset Timestamp { get; set; }
//
//         public ParcelAddressRelations()
//         { }
//     }
//
//     public sealed class ParcelAddressRelationConfiguration : IEntityTypeConfiguration<ParcelAddressRelations>
//     {
//         public void Configure(EntityTypeBuilder<ParcelAddressRelations> builder)
//         {
//             builder
//                 .ToView(nameof(ParcelAddressRelations), SuspiciousCasesContext.Schema)
//                 .HasNoKey()
//                 .ToSqlQuery(@$"
//                             SELECT
//                                 ""CaPaKey"",
//                                 ""AddressPersistentLocalId"",
//                                 ""Timestamp""
//                             FROM  {ViewName} ");
//
//             builder.HasIndex(x => x.CaPaKey);
//             builder.HasIndex(x => x.AddressPersistentLocalId);
//         }
//
//
//         public const string ViewName = @$"""{SuspiciousCasesContext.Schema}"".""VIEW_{nameof(ParcelAddressRelations)}""";
//
//         public const string Create = $@"
//             CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
//             SELECT
//                 p.""CaPaKey"",
//                 unnested_address_id AS ""AddressPersistentLocalId"",
//                 CURRENT_TIMESTAMP AS ""Timestamp""
//             FROM
//                 ""Integration"".""Parcels"" p
//             WHERE
//                 p.""IsRemoved"" = false
//             CROSS JOIN
//                 unnest(string_to_array(p.""Addresses"", ', ')::int[]) AS unnested_address_id;
//
//             CREATE CLUSTERED INDEX ""IX_AddressPersistentLocalId"" ON ""Integration"".""{ViewName}""(""{nameof(ParcelAddressRelations.AddressPersistentLocalId)}"")
//             CREATE INDEX ""IX_CaPaKey"" ON ""Integration"".""{ViewName}""(""{nameof(ParcelAddressRelations.CaPaKey)}"")
//             ";
//     }
// }
