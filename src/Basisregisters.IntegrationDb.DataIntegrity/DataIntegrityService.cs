namespace Basisregisters.IntegrationDb.DataIntegrity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class DataIntegrityService(
        DataIntegrityRepository repo,
        IHostApplicationLifetime hostApplicationLifetime,
        INotificationService notificationService,
        ILoggerFactory loggerFactory
        )
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logger = loggerFactory.CreateLogger<DataIntegrityService>();

            var errors = await repo.GetErrors();

            logger.LogInformation($"{errors.Count} Errors found");

            var message = FormatMessage(errors);
            var severity = errors.Any() ? NotificationSeverity.Danger : NotificationSeverity.Good;

            logger.LogInformation("Sending notification...");

            await notificationService.PublishToTopicAsync(new NotificationMessage(
                "DataIntegrity",
                message,
                "Data Integrity",
                severity));

            logger.LogInformation("Stopping application...");

            hostApplicationLifetime.StopApplication();
        }

        private static string FormatMessage(IList<DataIntegrityError> errors)
        {
            if (!errors.Any())
            {
                return "No data errors found.";
            }

            var message = new StringBuilder();
            message.AppendLine("Data errors found:");
            foreach (var error in errors)
            {
                message.AppendLine($"{error.Message}: {error.Count}");
            }

            return message.ToString();
        }
    }
}
