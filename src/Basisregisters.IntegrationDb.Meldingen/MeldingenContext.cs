namespace Basisregisters.IntegrationDb.Meldingen
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class MeldingenContext : DbContext
    {
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";
        public const string Schema = "integration_meldingen";

        public DbSet<Meldingsobject> Meldingsobjecten => Set<Meldingsobject>();
        public DbSet<MeldingsobjectStatuswijziging> MeldingsobjectStatuswijzigingen => Set<MeldingsobjectStatuswijziging>();
        public DbSet<Organisatie> Organisaties  => Set<Organisatie>();
        public DbSet<ProjectionState> ProjectionStates  => Set<ProjectionState>();


        public MeldingenContext()
        { }

        public MeldingenContext(DbContextOptions<MeldingenContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeldingenContext).Assembly);
        }

        public async Task SetLastPosition(int position, CancellationToken ct)
        {
            var projectionState = await ProjectionStates.SingleAsync(ct);
            projectionState.Position = position;
        }

        public async Task<int> GetLastPosition(CancellationToken ct)
        {
            return (await ProjectionStates.SingleAsync(ct)).Position;
        }
    }
}
