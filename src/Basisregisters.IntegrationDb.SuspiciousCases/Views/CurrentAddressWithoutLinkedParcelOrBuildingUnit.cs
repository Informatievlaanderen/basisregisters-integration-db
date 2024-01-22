// namespace Basisregisters.IntegrationDb.Schema.Views.SuspiciousCases
// {
//     using System;
//     using Microsoft.EntityFrameworkCore;
//     using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
//     public class CurrentAddressWithoutLinkedParcelOrBuildingUnit
//     {
//         public int AddressPersistentLocalId { get; set; }
//         public int NisCode { get; set; }
//         public DateTimeOffset Timestamp { get; set; }
//
//         public CurrentAddressWithoutLinkedParcelOrBuildingUnit()
//         { }
//     }
//
//     public sealed class CurrentAddressWithoutLinkedParcelOrBuildingUnitsConfiguration : IEntityTypeConfiguration<CurrentAddressWithoutLinkedParcelOrBuildingUnit>
//     {
//         public void Configure(EntityTypeBuilder<CurrentAddressWithoutLinkedParcelOrBuildingUnit> builder)
//         {
//             builder
//                 .ToView(nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit), SuspiciousCasesContext.Schema)
//                 .HasNoKey()
//                 .ToSqlQuery(@$"SELECT
//                                 ""AddressPersistentLocalId"",
//                                 ""NisCode"",
//                                 ""Timestamp""
//                                 FROM  {ViewName} ");
//         }
//
//         public const string ViewName = @$"""{SuspiciousCasesContext.Schema}"".""VIEW_{nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit)}""";
//
//         public const string Create = $@"
//             CREATE MATERIALIZED VIEW IF NOT EXISTS {ViewName} AS
//                 SELECT
//                         a.""PersistentLocalId"" AS ""AddressPersistentLocalId"",
//                         a.""NisCode"",
//                         CURRENT_TIMESTAMP AS ""Timestamp""
//                 FROM ""Integration"".""Addresses"" AS a
//                 WHERE EXISTS (
//                     SELECT 1
//                     FROM ""Integration"".""Addresses"" AS address
//                     LEFT JOIN {ParcelAddressRelationConfiguration.ViewName} AS parcelRelations
//                         ON address.""PersistentLocalId"" = parcelRelations.""AddressPersistentLocalId""
//                     LEFT JOIN {BuildingUnitAddressRelationConfiguration.ViewName} AS buildingUnitRelations
//                         ON address.""PersistentLocalId"" = buildingUnitRelations.""AddressPersistentLocalId""
//                     WHERE address.""PersistentLocalId"" = a.""PersistentLocalId""
//                         AND (parcelRelations.""AddressPersistentLocalId"" IS NULL AND buildingUnitRelations.""AddressPersistentLocalId"" IS NULL)
//                         AND address.""Status"" ILIKE 'inGebruik'
//                         AND address.""IsRemoved"" = false
//                         AND address.""PositionSpecification"" = 'ligplaats'
//                 )
//             ORDER BY a.""PersistentLocalId"";
//
//             CREATE INDEX ""IX_{nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit)}_AddressPersistentLocalId"" ON {ViewName} USING btree (""{nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit.AddressPersistentLocalId)}"");
//             CREATE INDEX ""IX_{nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit)}_NisCode"" ON {ViewName} USING btree (""{nameof(CurrentAddressWithoutLinkedParcelOrBuildingUnit.NisCode)}"");
//           ";
//     }
// }
