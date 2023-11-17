DROP MATERIALIZED VIEW "Integration"."VIEW_AddressesLinkedToMultipleBuildingUnits" ;
DROP MATERIALIZED VIEW "Integration"."VIEW_ParcelsLinkedToMultipleHouseNumbers" ;
DROP MATERIALIZED VIEW "Integration"."VIEW_ActiveHouseNumberWithoutLinkedParcel" ;
DROP MATERIALIZED VIEW "Integration"."VIEW_BuildingUnitAddressRelations" ;
DROP MATERIALIZED VIEW "Integration"."VIEW_ParcelAddressRelations" ;

DROP TABLE "Integration"."Addresses";
DROP TABLE "Integration"."BuildingUnits";
DROP TABLE "Integration"."Buildings";
DROP TABLE "Integration"."Municipalities";
DROP TABLE "Integration"."StreetNames";
DROP TABLE "Integration"."PostInfo";
DROP TABLE "Integration"."Parcels";
DROP TABLE "Integration"."__EFMigrationsHistoryIntegration";