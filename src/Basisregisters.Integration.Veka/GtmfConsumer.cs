namespace Basisregisters.Integration.Veka
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Notification;
    using Gtmf;
    using Microsoft.Extensions.Hosting;

    public class GtmfConsumer : BackgroundService
    {
        private readonly IProjectionState _projectionState;
        private readonly IGtmfApiProxy _gtmfApiProxy;
        private readonly IEmailSender _emailSender;
        private readonly INotificationService _notificationService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public GtmfConsumer(
            IProjectionState projectionState,
            IGtmfApiProxy gtmfApiProxy,
            IEmailSender emailSender,
            INotificationService notificationService,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _projectionState = projectionState;
            _gtmfApiProxy = gtmfApiProxy;
            _emailSender = emailSender;
            _notificationService = notificationService;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var lastPosition = await _projectionState.GetLastPosition(stoppingToken);
                lastPosition = lastPosition == 0 ? 1 : lastPosition;

                var events = (await _gtmfApiProxy.GetMeldingEventsFrom(lastPosition)).ToList();
                var emailsSent = 0;
                while (events.Any())
                {
                    foreach (var meldingEvent in events)
                    {
                        if (!meldingEvent.Type.Equals("MeldingAfgerondEvent", StringComparison.InvariantCultureIgnoreCase))
                        {
                            await _projectionState.SetLastPosition(meldingEvent.Position, stoppingToken);
                            lastPosition = meldingEvent.Position;
                            continue;
                        }

                        var melding = await _gtmfApiProxy.GetMelding(meldingEvent.MeldingId);

                        if (!melding.IsIngediendDoorVeka)
                        {
                            await _projectionState.SetLastPosition(meldingEvent.Position, stoppingToken);
                            lastPosition = meldingEvent.Position;
                            continue;
                        }

                        await _emailSender.SendEmailFor(melding);
                        emailsSent++;

                        await _projectionState.SetLastPosition(meldingEvent.Position, stoppingToken);
                        lastPosition = meldingEvent.Position;
                    }

                    events = (await _gtmfApiProxy.GetMeldingEventsFrom(lastPosition)).ToList();
                }

                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    nameof(GtmfConsumer),
                    $"Success: with last position {lastPosition} and amount of emails sent {emailsSent}",
                    "Veka gtmf email",
                    NotificationSeverity.Good));
            }
            catch (Exception e)
            {
                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    nameof(GtmfConsumer),
                    $"Failed: {e}",
                    "Veka gtmf email",
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
