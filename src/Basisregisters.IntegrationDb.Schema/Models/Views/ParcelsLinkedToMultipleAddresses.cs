﻿namespace Basisregisters.IntegrationDb.Schema.Models.Views
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ParcelsLinkedToMultipleAddresses
    {
        public string CaPaKey { get; set; }
        public int NisCode { get; set; }
        public int LinkedAddressCount { get; set; }
        public DateTime Timestamp { get; set; }

        public ParcelsLinkedToMultipleAddresses() { }
    }

    public sealed class ParcelsLinkedToMultipleAddressesConfiguration : IEntityTypeConfiguration<ParcelsLinkedToMultipleAddresses>
    {
        public void Configure(EntityTypeBuilder<ParcelsLinkedToMultipleAddresses> builder)
        {
            builder
                .ToView(nameof(ParcelsLinkedToMultipleAddresses), IntegrationContext.Schema)
                .HasNoKey()
                .ToSqlQuery(@$"
                            select
                                ""CaPaKey"",
                                ""NisCode"",
                                ""LinkedAddressCount""
                            FROM ""{IntegrationContext.Schema}"".""{nameof(ViewQueries.VIEW_ParcelsLinkedToMultipleAddresses)}"" ");
        }
    }
}