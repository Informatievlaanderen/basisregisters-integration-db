namespace Basisregisters.IntegrationDb.DataIntegrity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using Microsoft.Extensions.Hosting;

    public class DataIntegrityService(
        DataIntegrityRepository repo,
        IHostApplicationLifetime hostApplicationLifetime,
        INotificationService notificationService
        )
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var errors = await repo.GetErrors();

            var message = FormatMessage(errors);
            var severity = errors.Any() ? NotificationSeverity.Danger : NotificationSeverity.Good;

            await notificationService.PublishToTopicAsync(new NotificationMessage(
                "DataIntegrity",
                message,
                "Data Integrity",
                severity));

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
