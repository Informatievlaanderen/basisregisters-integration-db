namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Views;

    public class RefreshCountService : BackgroundService
    {
        private readonly SuspiciousCasesContext _context;
        private readonly SemaphoreSlim _isRunningSemaphore = new SemaphoreSlim(1, 1);
        private readonly int _refreshInMinutes;
        private readonly ILogger<RefreshCountService> _logger;

        public RefreshCountService(
            IConfiguration configuration,
            SuspiciousCasesContext context,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _refreshInMinutes = configuration.GetValue<int>("RefreshCountInMinutes", 10);
            _logger = loggerFactory.CreateLogger<RefreshCountService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RefreshCountService is starting.");

            await Task.Delay(10000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork(stoppingToken);

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(_refreshInMinutes), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // This exception is expected when the task is cancelled.
                }
            }

            _logger.LogInformation("RefreshCountService is stopping.");
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            if (!await _isRunningSemaphore.WaitAsync(0, stoppingToken))
            {
                _logger.LogInformation("Skipping execution because the previous task is still running.");
                return;
            }

            try
            {
                _logger.LogInformation("Refreshing count");

                _context.Database.SetCommandTimeout(Math.Max(1, _refreshInMinutes - 1) * 60);
                await _context.Database.ExecuteSqlRawAsync(SuspiciousCaseCountConfiguration.Refresh, stoppingToken);

                _logger.LogInformation("Refresh count completed");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "An error occurred while executing the background task.");
            }
            finally
            {
                _isRunningSemaphore.Release();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background service is stopping.");

            await base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _isRunningSemaphore.Dispose();
            base.Dispose();
        }
    }
}
