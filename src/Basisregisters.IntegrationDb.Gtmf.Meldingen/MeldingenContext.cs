namespace Basisregisters.IntegrationDb.Gtmf.Meldingen
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class MeldingenContext : DbContext
    {
        public const string MigrationsTableName = "__EFMigrationsHistoryIntegration";
        public const string Schema = "integration_gtmf";

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
            var projectionState = await ProjectionStates.FindAsync([ProjectionState.GtmfProjectionName], cancellationToken: ct);

            if (projectionState is null)
            {
                projectionState = new ProjectionState
                {
                    Name = ProjectionState.GtmfProjectionName
                };
                ProjectionStates.Add(projectionState);
            }

            projectionState.Position = position;
        }

        public async Task<int> GetLastPosition(CancellationToken ct)
        {
            var projectionState = await ProjectionStates.FindAsync([ProjectionState.GtmfProjectionName], cancellationToken: ct);

            return projectionState is null || projectionState.Position == 0
                ? 1
                : projectionState.Position;
        }
    }

    public class ConfigBasedMeldingenContextFactory : IDesignTimeDbContextFactory<MeldingenContext>
    {
        public MeldingenContext CreateDbContext(string[] args)
        {
            const string connectionStringName = "IntegrationAdmin";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true)
                .AddJsonFile($"appsettings.development.json", true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new DbContextOptionsBuilder<MeldingenContext>();

            var connectionString = configuration
                                       .GetConnectionString(connectionStringName)
                                   ?? throw new InvalidOperationException($"Could not find a connection string with name '{connectionStringName}'");

            builder
                .UseNpgsql(connectionString, npgSqlOptions =>
                {
                    npgSqlOptions.EnableRetryOnFailure();
                    npgSqlOptions.MigrationsHistoryTable(
                        MeldingenContext.MigrationsTableName,
                        MeldingenContext.Schema);
                    npgSqlOptions.UseNetTopologySuite();
                });

            return new MeldingenContext(builder.Options);
        }
    }
}
