﻿namespace Basisregisters.IntegrationDb.Gtmf.Meldingen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Api;
    using Api.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NodaTime;
    using Notifications;
    using GtmfOrganisatie = Api.Organisatie;

    public class MeldingEventConsumer : BackgroundService
    {
        private readonly MeldingenContext _meldingenContext;

        private readonly IGtmfApiProxy _gtmfApiProxy;
        private readonly IMeldingsobjectEventDeserializer _meldingsobjectEventDeserializer;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private List<Organisatie>? _organisaties;

        public MeldingEventConsumer(
            MeldingenContext meldingenContext,
            IGtmfApiProxy gtmfApiProxy,
            IMeldingsobjectEventDeserializer meldingsobjectEventDeserializer,
            INotificationService notificationService,
            ILoggerFactory loggerFactory,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _meldingenContext = meldingenContext;
            _gtmfApiProxy = gtmfApiProxy;
            _meldingsobjectEventDeserializer = meldingsobjectEventDeserializer;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger<MeldingEventConsumer>();
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lastPosition = 0;
            try
            {
                lastPosition = await _meldingenContext.GetLastPosition(stoppingToken);

                var events = (await _gtmfApiProxy.GetEventsFrom(lastPosition)).ToList();
                var eventsProcessedCount = 0;

                while (events.Any())
                {
                    foreach (var @event in events)
                    {
                        lastPosition = @event.Position;
                        await HandleMeldingEvent(@event, stoppingToken);

                        await _meldingenContext.SetLastPosition(@event.Position, stoppingToken);
                        eventsProcessedCount++;
                    }

                    await _meldingenContext.SaveChangesAsync(stoppingToken);

                    events = (await _gtmfApiProxy.GetEventsFrom(lastPosition)).ToList();
                }

                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    nameof(MeldingEventConsumer),
                    $"Success: {eventsProcessedCount} events processed",
                    "IntegrationDb GTMF meldingen",
                    NotificationSeverity.Good));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"An error occured while at event position {lastPosition}.");

                await _notificationService.PublishToTopicAsync(new NotificationMessage(
                    nameof(MeldingEventConsumer),
                    $"Failed: {exception}",
                    "IntegrationDb GTMF meldingen",
                    NotificationSeverity.Danger));

                throw;
            }
            finally
            {
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async Task HandleMeldingEvent(MeldingEvent meldingEvent, CancellationToken ct)
        {
            if (!MeldingEventTypes.All.Contains(meldingEvent.EventType))
            {
                throw new NotImplementedException($"Event type {meldingEvent.EventType} not supported.");
            }

            if (meldingEvent.IsMeldingIngediendEvent())
            {
                await HandleMeldingIngediendEvent(meldingEvent, ct);
                return;
            }

            // Only meldingsobject events provide us with real information
            if (!meldingEvent.IsMeldingsobjectEvent())
            {
                return;
            }

            await HandleMeldingsobjectEvent(meldingEvent, ct);
        }

        private async Task HandleMeldingIngediendEvent(MeldingEvent meldingEvent, CancellationToken ct)
        {
            var ingediendMeldingsobject = await _gtmfApiProxy.GetMeldingsobject(meldingEvent.MeldingId);
            var (meldingsobject, indienerOrganisatie) = ingediendMeldingsobject;

            var organisatie = await GetOrAddOrganisatie(indienerOrganisatie, ct);
            meldingsobject.MeldingsorganisatieIdInternal = organisatie.IdInternal;

            _meldingenContext.Meldingsobjecten.Add(meldingsobject);
            _meldingenContext.MeldingsobjectStatuswijzigingen.Add(
                new MeldingsobjectStatuswijziging(
                    meldingsobject.MeldingsobjectId,
                    meldingsobject.MeldingId,
                    null,
                    MeldingsobjectStatussen.Ingediend,
                    organisatie.IdInternal,
                    meldingsobject.DatumIndieningTimestamp,
                    null));
        }

        private async Task HandleMeldingsobjectEvent(MeldingEvent meldingEvent, CancellationToken ct)
        {
            var meldingsobjectEvent = _meldingsobjectEventDeserializer.DeserializeFrom(meldingEvent);

            var organisatie = await GetOrAddOrganisatie(meldingsobjectEvent.GetInitiatorOrganisatie(), ct);

            _meldingenContext.MeldingsobjectStatuswijzigingen.Add(
                new MeldingsobjectStatuswijziging(
                    meldingsobjectEvent.MeldingsobjectId,
                    meldingsobjectEvent.MeldingId,
                    meldingsobjectEvent.GetOudeStatus(),
                    meldingsobjectEvent.GetNieuweStatus(),
                    organisatie.IdInternal,
                    Instant.FromDateTimeOffset(meldingsobjectEvent.AangemaaktOp),
                    meldingsobjectEvent.ToelichtingMelder));
        }

        private async Task<Organisatie> GetOrAddOrganisatie(GtmfOrganisatie gtmfOrganisatie, CancellationToken ct)
        {
            _organisaties ??= await _meldingenContext.Organisaties.ToListAsync(ct);

            var organisatie = _organisaties.SingleOrDefault(x => x.Id == gtmfOrganisatie.Id);

            if (organisatie is not null)
            {
                return organisatie;
            }

            organisatie = new Organisatie(
                gtmfOrganisatie.Id,
                Guid.NewGuid(),
                gtmfOrganisatie.Naam,
                gtmfOrganisatie.OvoCode, gtmfOrganisatie.KboNummer);

            _meldingenContext.Organisaties.Add(organisatie);
            _organisaties.Add(organisatie);

            return organisatie;
        }
    }
}
