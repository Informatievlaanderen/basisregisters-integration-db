-- Query om de comma seperated adresobjectids te splitten
SELECT   REPLACE(value, ' ', '') as Adres_ObjectID
FROM [geolocation].[ParcelOsloGeolocation]  
    CROSS APPLY STRING_SPLIT([ADRESSEN_OBJECTIDS], ',') 
	WHERE RTRIM(value) <> '' ;
---------------------------------------------------------------


-- Adressen die aan meerdere gebouweenheden gekoppeld zijn.
select a.[VOLLEDIGADRES], b.[IDENTIFICATOR_OBJECTID], b.adressen_objectids, b.gebouweenheidstatus
from [geolocation].[BuildingUnitOsloGeolocation] b
inner join [geolocation].[AddressOsloGeolocation] a on a.[IDENTIFICATOR_OBJECTID] = b.adressen_objectids
where adressen_objectids in (

select REPLACE(value, ' ', '') as Adres_ObjectID
from (
select adressen_objectids
from [geolocation].[BuildingUnitOsloGeolocation]
where gebouweenheidstatus <> 'gehistoreerd'
and adressen_objectids <> ''
group by adressen_objectids
having count(adressen_objectids) > 1
)a CROSS APPLY STRING_SPLIT([ADRESSEN_OBJECTIDS], ',') 
	WHERE RTRIM(value) <> ''
)
order by adressen_objectids

--INTEGRATION DB RE-WRITE--
SELECT a."FullName", b."PersistentLocalId", b."Status"
FROM "Integration"."BuildingUnits" b
INNER JOIN "Integration"."Addresses" a ON a."PersistentLocalId"::TEXT = b."Addresses"
WHERE b."Addresses" IN (
    SELECT REPLACE(value, ' ', '') AS Adres_ObjectID
    FROM (
        SELECT unnest(string_to_array("Addresses", ',')) AS value
        FROM "Integration"."BuildingUnits"
        WHERE "Status" <> 'gehistoreerd' AND "Addresses" <> ''
        GROUP BY "Addresses"
        HAVING count("Addresses") > 1
    ) a
    WHERE rtrim(value) <> ''
)
ORDER BY b."Addresses";


-- AddressPersistentLocalId, BuildingUnitPersistentLocalId

SELECT REPLACE(AddressPersistentLocalId, ' ', '') AS Adres_ObjectID
FROM (
    SELECT unnest(string_to_array("Addresses", ',')) AS AddressPersistentLocalId
    FROM "Integration"."BuildingUnits"
    WHERE "Status" <> 'gehistoreerd' AND "Addresses" <> ''
    GROUP BY "Addresses"
    HAVING count("Addresses") > 1
) a
WHERE rtrim(AddressPersistentLocalId) <> ''

------------------------------------------------------------------------

-- Straatnamen met status "in gebruik" dienen gekoppeld te worden aan ��n of meerdere wegverbindingen. (Foutief)
select *
from [geolocation].[StreetNameOsloGeolocation]
where straatnaamstatus='ingebruik'
and [IDENTIFICATOR_OBJECTID] not in
 (select [LEFTSIDESTREETNAMEID]
 from [geolocation].[RoadSegmentGeolocation])
intersect
 select *
from [geolocation].[StreetNameOsloGeolocation]
where straatnaamstatus='ingebruik'
and [IDENTIFICATOR_OBJECTID] not in
 (select [RIGHTSIDESTREETNAMEID]
 from [geolocation].[RoadSegmentGeolocation])
 --Check
 select *
 from [geolocation].[RoadSegmentGeolocation]
 where [LEFTSIDESTREETNAMEID] =352
 or [RIGHTSIDESTREETNAMEID] = 352

--INTEGRATION DB RE-WRITE--
SELECT *
FROM "Integration"."StreetName"
WHERE "Status" = 'ingebruik'
  AND "PersistentLocalId" NOT IN (
    SELECT "LEFTSIDESTREETNAMEID"
    FROM "Integration"."RoadSegment"
  )

INTERSECT

SELECT *
FROM "Integration"."StreetName"
WHERE "Status" = 'ingebruik'
  AND "PersistentLocalId" NOT IN (
    SELECT "RIGHTSIDESTREETNAMEID"
    FROM "Integration"."RoadSegment"
  );

SELECT *
FROM "Integration"."RoadSegment"
WHERE "LEFTSIDESTREETNAMEID" = 352
   OR "RIGHTSIDESTREETNAMEID" = 352;



 ----------------------------------------------------------------------------------------------------------

 --Percelen gekoppeld aan meerdere huisnummers
 select a.[VOLLEDIGADRES], p.[IDENTIFICATOR_OBJECTID], p.adressen_objectids
from [geolocation].[ParcelOsloGeolocation] p
inner join [geolocation].[AddressOsloGeolocation] a on a.[IDENTIFICATOR_OBJECTID] = p.adressen_objectids
where adressen_objectids in
(
select REPLACE(value, ' ', '') as Adres_ObjectID
FROM (
 select adressen_objectids
 from [geolocation].[ParcelOsloGeolocation]
 where perceelstatus = 'gerealiseerd'
 and adressen_objectids <> ''
 group by adressen_objectids
having count(adressen_objectids) > 1
)a CROSS APPLY STRING_SPLIT([ADRESSEN_OBJECTIDS], ',') 
	WHERE RTRIM(value) <> ''
)
order by adressen_objectids


SELECT   REPLACE(value, ' ', '') as Adres_ObjectID
FROM [geolocation].[ParcelOsloGeolocation]  
    CROSS APPLY STRING_SPLIT([ADRESSEN_OBJECTIDS], ',') 
	WHERE RTRIM(value) <> '' ;


--INTEGRATION DB RE-WRITE--
SELECT a."FullName", p."PersistentLocalId", p."Addresses"
FROM "Integration"."Parcels" p
INNER JOIN "Integration"."Addresses" a ON a."PersistentLocalId" = p."Addresses"
WHERE p."Addresses" IN (
    SELECT REPLACE(value, ' ', '') AS Adres_ObjectID
    FROM (
        SELECT "Addresses"
        FROM "Integration"."Parcels"
        WHERE "Status" = 'gerealiseerd' AND "Addresses" <> ''
        GROUP BY "Addresses"
        HAVING count("Addresses") > 1
    ) a
    CROSS JOIN LATERAL unnest(string_to_array("Addresses", ',')) AS value
    WHERE rtrim(value) <> ''
)
ORDER BY p."Addresses";

-- Second query
SELECT REPLACE(value, ' ', '') AS Adres_ObjectID
FROM "Integration"."Parcels"
CROSS JOIN LATERAL unnest(string_to_array("Addresses", ',')) AS value
WHERE rtrim(value) <> '';


------------------------------------------------------


--Wegverbindingen met slechts 1 kant
--Linkerkant
select *
from [geolocation].[RoadSegmentGeolocation]
where [LEFTSIDESTREETNAMEID] = -9
order by [LEFTSIDEMUNICIPALITYNISCODE]

--Rechterkant
select *
from [geolocation].[RoadSegmentGeolocation]
where [RIGHTSIDESTREETNAMEID] = -9
order by [RIGHTSIDEMUNICIPALITYNISCODE]

--beide kanten geen koppeling met straatnaam
select *
from [geolocation].[RoadSegmentGeolocation]
where [RIGHTSIDESTREETNAMEID] = -9
and [LEFTSIDESTREETNAMEID] = -9
order by maintainerid
-----------------------------------------------

-- Huisnummers in gebruik zonder koppeling met terrein 
select *
from [geolocation].[AddressOsloGeolocation]
where adresstatus = 'ingebruik'
and [IDENTIFICATOR_OBJECTID] not in 
(
SELECT   REPLACE(value, ' ', '') as Adres_ObjectID
FROM [geolocation].[ParcelOsloGeolocation]  
    CROSS APPLY STRING_SPLIT([ADRESSEN_OBJECTIDS], ',') 
	WHERE RTRIM(value) <> '' 
and perceelstatus = 'gerealiseerd'
and removed=0
)
-- aanduiding adrespositie perceel
select *
from [geolocation].[AddressOsloGeolocation]
where adresstatus = 'ingebruik'
and [IDENTIFICATOR_OBJECTID] not in 
(
SELECT   REPLACE(value, ' ', '') as Adres_ObjectID
FROM [geolocation].[ParcelOsloGeolocation]  
    CROSS APPLY STRING_SPLIT([ADRESSEN_OBJECTIDS], ',') 
	WHERE RTRIM(value) <> '' 
and perceelstatus = 'gerealiseerd'
and removed=0
)
and adrespositie_positiespecificatie = 'perceel'
--controle
select *
from [geolocation].[ParcelOsloGeolocation]
where [ADRESSEN_OBJECTIDS] like '%1274940%'


--INTEGRATION DB RE-WRITE--
SELECT *
FROM "Integration"."Addresses"
WHERE "Status" = 'ingebruik'
  AND "PersistentLocalId" NOT IN (
    SELECT REPLACE(value, ' ', '') AS Address_ObjectID
    FROM "Integration"."Parcels"
    CROSS JOIN LATERAL unnest(string_to_array("ADDRESSES", ',')) AS value
    WHERE rtrim(value) <> '' 
      AND "Status" = 'gerealiseerd'
      AND "IsRemoved" = 0
  );

SELECT *
FROM "Integration"."Addresses"
WHERE "Status" = 'ingebruik'
  AND "PersistentLocalId" NOT IN (
    SELECT REPLACE(value, ' ', '') AS Address_ObjectID
    FROM "Integration"."Parcels"
    CROSS JOIN LATERAL unnest(string_to_array("ADDRESSES", ',')) AS value
    WHERE rtrim(value) <> '' 
      AND "Status" = 'gerealiseerd'
      AND "IsRemoved" = 0
  )
  AND "PositionSpecification" = 'perceel';

SELECT *
FROM "Integration"."Parcels"
WHERE "ADDRESSES" LIKE '%1274940%';


----------------------------------------------------------
