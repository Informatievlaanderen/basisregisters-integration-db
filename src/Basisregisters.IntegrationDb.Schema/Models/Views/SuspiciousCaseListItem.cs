namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SuspiciousCaseListItem
    {
        public string NisCode { get; set; }
        public int Count { get; set; }
        public SuspiciousCasesType Type { get; set; }

        public SuspiciousCaseListItem()
        { }

        public sealed class ActiveHouseNumberWithoutLinkedParcelConfiguration : IEntityTypeConfiguration<SuspiciousCaseListItem>
        {
            public void Configure(EntityTypeBuilder<SuspiciousCaseListItem> builder)
            {
                builder
                    .ToView(nameof(SuspiciousCaseListItem), IntegrationContext.Schema)
                    .HasNoKey()
                    .ToSqlQuery(@"select
                                ""NisCode""
                                ,""Count""
                                ,""Type""
                            FROM ""Integration"".""VIEW_SuspiciousCaseListItem"";");
            }

            string SqlView = @$"SELECT ""NisCode"", count(*) as ""Count"", {SuspiciousCasesType.CurrentHouseNumbersWithoutLinkedParcel} as ""Type""
                                            ""FROM ""Integration"".""VIEW_ActiveAddressWithoutLinkedParcels""
                                            ""group by ""NisCode""";
        }
    }
}
