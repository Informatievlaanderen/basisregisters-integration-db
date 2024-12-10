namespace Basisregisters.IntegrationDb.SuspiciousCases.Views
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public sealed class CurrentAddressLinkedToProposedStreetName : SuspiciousCase
    {
        public int AddressPersistentLocalId { get; set; }

        public override Category Category => Category.Address;
    }

    public sealed class CurrentAddressLinkedToProposedStreetNameConfiguration : IEntityTypeConfiguration<CurrentAddressLinkedToProposedStreetName>
    {
        public void Configure(EntityTypeBuilder<CurrentAddressLinkedToProposedStreetName> builder)
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

        public const string ViewName = "view_current_address_linked_to_proposed_streetname";

        public const string Create = $@"
            CREATE OR REPLACE VIEW {Schema.SuspiciousCases}.{ViewName} AS
	        SELECT
		        CAST(a.persistent_local_id as varchar) AS persistent_local_id,
		        a.persistent_local_id AS address_persistent_local_id,
		        s.nis_code AS nis_code,
                {Schema.FullAddress}(s.name_dutch, a.house_number, a.box_number, a.postal_code, m.name_dutch) as description
	        FROM {SchemaLatestItems.Address} AS a
            JOIN {SchemaLatestItems.StreetName} s ON s.persistent_local_id = a.street_name_persistent_local_id AND s.status = 0
            JOIN {SchemaLatestItems.Municipality} m ON s.municipality_id = m.municipality_id
	        WHERE
                a.status = 2
	            AND a.removed = false
            ;";
    }
}
