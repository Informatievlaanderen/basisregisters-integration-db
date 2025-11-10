namespace Basisregisters.IntegrationDb.Api.Address;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;
using AddressRegistry.Api.BackOffice.Abstractions.Requests;
using Be.Vlaanderen.Basisregisters.GrAr.Common.SpatialTools.GeometryCoordinates;
using Be.Vlaanderen.Basisregisters.GrAr.Edit.Contracts;
using CorrectDerivedFromBuildingUnitPositions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Utilities;
using Repositories;

public sealed class AddressCorrectionBackgroundService : BackgroundService
{
    private readonly Channel<AddressCorrectionWorkItem> _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AddressCorrectionBackgroundService> _logger;

    public AddressCorrectionBackgroundService(
        Channel<AddressCorrectionWorkItem> channel,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AddressCorrectionBackgroundService> logger)
    {
        _channel = channel;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var workItem in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<AddressRegistryApiClient>();

                await ProcessAddresses(workItem.Addresses, client, stoppingToken);

                _logger.LogInformation("Processed {Count} addresses", workItem.Addresses.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process addresses");
            }
        }
    }

    private async Task ProcessAddresses(
        IReadOnlyCollection<AddressWithGeometry> addresses,
        AddressRegistryApiClient client,
        CancellationToken cancellationToken)
    {
        foreach (var address in addresses)
        {
            var geometry = new WKTReader().Read(address.Geometry);
            var geometryAsGml = CorrectDerivedFromBuildingUnitPositionsRequestHandler.GetGml(geometry);

            var apiRequest = new CorrectAddressPositionRequest
            {
                PositieGeometrieMethode = PositieGeometrieMethode.AfgeleidVanObject,
                PositieSpecificatie = PositieSpecificatie.Gebouweenheid,
                Positie = geometryAsGml
            };

            await client.CorrectAddressPosition(address.PersistentLocalId, apiRequest, cancellationToken);
        }
    }
}

public sealed class AddressCorrectionWorkItem
{
    public IReadOnlyCollection<AddressWithGeometry> Addresses { get; }

    public AddressCorrectionWorkItem(IReadOnlyCollection<AddressWithGeometry> addresses)
    {
        Addresses = addresses;
    }
}
