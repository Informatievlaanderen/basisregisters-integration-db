namespace Basisregisters.IntegrationDb.Gtmf.Meldingen
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProjectionState
    {
        public const string GtmfProjectionName = "GTMF";

        public required string Name { get; set; }
        public int Position { get; set; }

        public ProjectionState()
        { }

        public ProjectionState(string name, int position)
        {
            Name = name;
            Position = position;
        }
    }

    public sealed class ProjectionStateConfiguration : IEntityTypeConfiguration<ProjectionState>
    {
        private const string TableName = "projection_state";

        public void Configure(EntityTypeBuilder<ProjectionState> builder)
        {
            builder.ToTable(TableName, MeldingenContext.Schema)
                .HasKey(x => x.Name);
            // .IsClustered()

            builder.Property(x => x.Name).HasColumnName("naam");
            builder.Property(x => x.Position).HasColumnName("position");
        }
    }
}
