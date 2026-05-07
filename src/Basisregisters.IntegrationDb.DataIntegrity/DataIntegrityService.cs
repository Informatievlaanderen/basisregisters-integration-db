namespace Basisregisters.IntegrationDb.DataIntegrity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
    using Feeds;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public sealed class DataIntegrityService : BackgroundService
    {
        private readonly SuspiciousCasesIntegrityRepository _suspiciousCasesRepository;
        private readonly IEnumerable<IFeedIntegrityRepository> _feedIntegrityRepositories;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly INotificationService _notificationService;
        private readonly ILoggerFactory _loggerFactory;

        public DataIntegrityService(
            SuspiciousCasesIntegrityRepository suspiciousCasesRepository,
            IEnumerable<IFeedIntegrityRepository> feedIntegrityRepositories,
            IHostApplicationLifetime hostApplicationLifetime,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            _suspiciousCasesRepository = suspiciousCasesRepository;
            _feedIntegrityRepositories = feedIntegrityRepositories;
            _hostApplicationLifetime = hostApplicationLifetime;
            _notificationService = notificationService;
            _loggerFactory = loggerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var logger = _loggerFactory.CreateLogger<DataIntegrityService>();

            var errors = await _suspiciousCasesRepository.GetErrors();

            logger.LogInformation($"{errors.Count} Errors found");

            var message = FormatSuspiciousCasesMessage(errors);
            var severity = errors.Any() ? NotificationSeverity.Danger : NotificationSeverity.Good;

            logger.LogInformation("Sending notification...");

            await _notificationService.PublishToTopicAsync(new NotificationMessage(
                "DataIntegrity",
                message,
                "Data Integrity",
                severity));

            await ProcessFeedIntegrityErrors(stoppingToken);

            logger.LogInformation("Stopping application...");

            _hostApplicationLifetime.StopApplication();
        }

        private async Task ProcessFeedIntegrityErrors(CancellationToken stoppingToken)
        {
            var feedErrorsFound = false;
            foreach (var feedIntegrityRepository in _feedIntegrityRepositories)
            {
                await feedIntegrityRepository.RefreshViewAsync(stoppingToken);
                var feedErrors = (await feedIntegrityRepository.GetIntegrityErrorsAsync(stoppingToken)).ToList();
                if(feedErrors.Any())
                {
                    feedErrorsFound = true;
                    await _notificationService.PublishToTopicAsync(new NotificationMessage(
                        "DataIntegrity",
                        $"Feed Integrity Errors ({feedIntegrityRepository.RegisterName}): " + string.Join(", ", feedErrors.Take(10)),
                        "Data Integrity",
                        NotificationSeverity.Danger));
                }
            }

            if (!feedErrorsFound)
            {
                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    "DataIntegrity",
                    "No feed integrity errors found.",
                    "Data Integrity",
                    NotificationSeverity.Good));
            }
        }

        private static string FormatSuspiciousCasesMessage(IList<DataIntegrityError> errors)
        {
            if (!errors.Any())
            {
                return "No suspicious cases data errors found.";
            }

            var message = new StringBuilder();
            message.AppendLine("Suspicious cases data errors found:");
            foreach (var error in errors)
            {
                message.AppendLine($"{error.Message}: {error.Count}");
            }

            return message.ToString();
        }
    }
}
