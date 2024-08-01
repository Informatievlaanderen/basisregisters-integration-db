namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class AddressLongerThanTwoYearsProposed : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }

        public override Category Category => Category.Address;
    }

    public sealed class AddressLongerThanTwoYearsProposedConfiguration : IEntityTypeConfiguration<AddressLongerThanTwoYearsProposed>
    {
        public void Configure(EntityTypeBuilder<AddressLongerThanTwoYearsProposed> builder)
        {
            builder
                .ToView(ViewName, Schema.SuspiciousCases)
                .HasNoKey()
                .ToSqlQuery(@$"
                            SELECT
                                persistent_local_id,
                                address_persistent_local_id,
                                nis_code,
                                description
                            FROM  {Schema.SuspiciousCases}.{ViewName}");

            builder.Property(x => x.PersistentLocalId).HasColumnName("persistent_local_id");
            builder.Property(x => x.AddressPersistentLocalId).HasColumnName("address_persistent_local_id");
            builder.Property(x => x.NisCode).HasColumnName("nis_code");
            builder.Property(x => x.Description).HasColumnName("description");
        }

        public const string ViewName = "view_address_longer_than_two_years_proposed";

        public const string Create = $@"
            CREATE OR REPLACE VIEW {Schema.SuspiciousCases}.{ViewName} AS
	        SELECT
		        CAST(a.persistent_local_id as varchar) AS persistent_local_id,
		        a.persistent_local_id AS address_persistent_local_id,
		        s.nis_code AS nis_code,
                {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
	        FROM {SchemaLatestItems.Address} AS a
            JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id
            JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
	        WHERE
                a.status = 1
	            AND a.removed = false
	            AND a.version_timestamp <= CURRENT_TIMESTAMP - INTERVAL '2 years'
            ;";
    }
}
