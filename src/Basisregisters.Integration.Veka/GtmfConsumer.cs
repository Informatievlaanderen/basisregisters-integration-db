namespace Basisregisters.Integration.Veka
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Gtmf;
    using Microsoft.Extensions.Hosting;

    public class GtmfConsumer : BackgroundService
    {
        private readonly IProjectionState _projectionState;
        private readonly IGtmfApiProxy _gtmfApiProxy;
        private readonly IEmailSender _emailSender;

        public GtmfConsumer(
            IProjectionState projectionState,
            IGtmfApiProxy gtmfApiProxy,
            IEmailSender emailSender)
        {
            _projectionState = projectionState;
            _gtmfApiProxy = gtmfApiProxy;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastPosition = await _projectionState.GetLastPosition(stoppingToken);

            var events = await _gtmfApiProxy.GetMeldingEventsFrom(lastPosition);

            foreach (var meldingEvent in events)
            {
                if (!meldingEvent.Type.Equals("MeldingAfgerondEvent", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var melding = await _gtmfApiProxy.GetMelding(meldingEvent.MeldingId);

                if (!melding.IsIngediendDoorVeka)
                {
                    continue;
                }

                await _emailSender.SendEmailFor(melding);

                await _projectionState.SetLastPosition(meldingEvent.Position, stoppingToken);
            }
        }
    }
}
