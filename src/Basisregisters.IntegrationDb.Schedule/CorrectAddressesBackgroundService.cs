namespace Basisregisters.IntegrationDb.Schedule
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Integration.Common.Notification;
    using Microsoft.Extensions.Hosting;

    public class CorrectAddressesBackgroundService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IntegrationDbApiClient _integrationDbApiClient;
        private readonly INotificationService _notificationService;

        public CorrectAddressesBackgroundService(
            IHostApplicationLifetime hostApplicationLifetime,
            IntegrationDbApiClient integrationDbApiClient,
            INotificationService notificationService
        )
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _integrationDbApiClient = integrationDbApiClient;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _integrationDbApiClient.CorrectAddressesDerivedFromBuildingUnitPosition(stoppingToken);
            }
            catch (Exception ex)
            {
                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    nameof(CorrectAddressesBackgroundService),
                    $"Failed: {ex}",
                    "Adressen corrigeren op basis van gebouweenheid posities",
                    NotificationSeverity.Danger));

                throw;
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }
    }
}
