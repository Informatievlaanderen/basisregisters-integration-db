namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases.Infrastructure;

using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using IntegrationDb.SuspiciousCases;
using Microsoft.Data.SqlClient;
using Npgsql;

public interface ISuspiciousCasesRepository
{
    Task<IEnumerable<SuspiciousCaseDto>> GetSuspiciousCasesAsync();
}

public sealed class SuspiciousCasesRepository : ISuspiciousCasesRepository
{
    private readonly string _connectionString;

    public SuspiciousCasesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<SuspiciousCaseDto>> GetSuspiciousCasesAsync()
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<SuspiciousCaseDto>(SqlStatement, commandTimeout: 60 * 60);
        }
    }

    public static readonly string SqlStatement = $$"""
SELECT DISTINCT q.nis_code as NisCode, q.object_id as ObjectId, q.object_type as ObjectType, q.type as SuspiciousCaseType FROM (
    SELECT v1.nis_code, v1.streetname_persistent_local_id as object_id, '{{ nameof(Category.StreetName) }}' as object_type, {{(int)SuspiciousCasesType.CurrentStreetNameWithoutLinkedRoadSegment}} AS type FROM integration_suspicious_cases.view_current_street_name_without_linked_road_segments as v1
    UNION ALL SELECT v2.nis_code, v2.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits}} AS type FROM integration_suspicious_cases.view_current_address_without_linked_parcel_or_building_unit as v2
    UNION ALL SELECT v3.nis_code, v3.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.ActiveAddressLinkedToMultipleBuildingUnits}} AS type FROM integration_suspicious_cases.view_active_address_linked_to_multiple_building_units as v3
    UNION ALL SELECT v4.nis_code, v4.streetname_persistent_local_id as object_id, '{{ nameof(Category.StreetName) }}' as object_type, {{(int)SuspiciousCasesType.StreetNameLongerThanTwoYearsProposed}} AS type FROM integration_suspicious_cases.view_streetname_longer_than_two_years_proposed as v4
    UNION ALL SELECT v5.nis_code, v5.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.ActiveAddressOutsideOfMunicipalityBounds}} AS type FROM integration_suspicious_cases.view_active_address_outside_municipality_bounds as v5
    UNION ALL SELECT v6.nis_code, v6.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.AddressLongerThanTwoYearsProposed}} AS type FROM integration_suspicious_cases.view_address_longer_than_two_years_proposed as v6
    UNION ALL SELECT v7.nis_code, v7.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.CurrentAddressesWithSpecificationDerivedFromBuildingUnitWithoutLinkedBuildingUnit}} AS type FROM integration_suspicious_cases.view_current_address_with_specification_derived_from_building_u as v7
    UNION ALL SELECT v8.nis_code, v8.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.CurrentAddressLinkedWithBuildingUnitButNotWithParcel}} AS type FROM integration_suspicious_cases.view_current_address_linked_with_building_unit_but_not_with_par as v8
    UNION ALL SELECT v9.nis_code, v9.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.ProposedAddressWithoutLinkedParcelOrBuildingUnit}} AS type FROM integration_suspicious_cases.view_proposed_address_without_linked_parcel_or_building_unit as v9
    UNION ALL SELECT v10.nis_code, v10.building_persistent_local_id as object_id, '{{ nameof(Category.Building) }}' as object_type, {{(int)SuspiciousCasesType.BuildingLongerThanTwoYearsPlanned}} AS type FROM integration_suspicious_cases.view_buildings_longer_than_two_years_planned as v10
    UNION ALL SELECT v12.nis_code, v12.building_unit_persistent_local_id as object_id, '{{ nameof(Category.BuildingUnit) }}' as object_type, {{(int)SuspiciousCasesType.BuildingUnitLongerThanTwoYearsPlanned}} AS type FROM integration_suspicious_cases.view_building_unit_longer_than_two_years_planned as v12
    UNION ALL SELECT v13.nis_code, v13.road_segment_persistent_local_id as object_id, '{{ nameof(Category.RoadSegment) }}' as object_type, {{(int)SuspiciousCasesType.RoadSegmentLongerThanTwoYearsWithPermit}} AS type FROM integration_suspicious_cases.view_road_segment_longer_than_two_years_with_permit as v13
    UNION ALL SELECT v14.nis_code, v14.road_segment_persistent_local_id as object_id, '{{ nameof(Category.RoadSegment) }}' as object_type, {{(int)SuspiciousCasesType.RoadSegmentWithSingleLinkedStreetName}} AS type FROM integration_suspicious_cases.view_road_segment_with_single_linked_streetname as v14
    UNION ALL SELECT v16.nis_code, v16.address_persistent_local_id as object_id, '{{ nameof(Category.Address) }}' as object_type, {{(int)SuspiciousCasesType.CurrentAddressLinkedToProposedStreetName}} AS type FROM integration_suspicious_cases.view_current_address_linked_to_proposed_streetname as v16
    UNION ALL SELECT v17.nis_code, v17.road_segment_persistent_local_id as object_id, '{{ nameof(Category.RoadSegment) }}' as object_type, {{(int)SuspiciousCasesType.RoadSegmentLinkedToRetiredStreetName}} AS type FROM integration_suspicious_cases.view_road_segment_linked_to_retired_streetname as v17
) as q
ORDER BY q.nis_code, q.type
""";
}

public sealed class SuspiciousCaseDto
{
    public string NisCode { get; set; } = null!;
    public string ObjectId { get; set; } = null!;
    public Category ObjectType { get; set; }
    public SuspiciousCasesType SuspiciousCaseType { get; set; }
}
