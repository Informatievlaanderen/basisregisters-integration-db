namespace Basisregisters.IntegrationDb.Api.Address.CorrectDerivedFromBuildingUnitPositions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using System.Xml;
    using Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;
    using AddressRegistry.Api.BackOffice.Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Be.Vlaanderen.Basisregisters.GrAr.Common.SpatialTools.GeometryCoordinates;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit.Contracts;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;
    using NetTopologySuite.Utilities;
    using Repositories;

    public sealed class CorrectDerivedFromBuildingUnitPositionsRequestHandler : IRequestHandler<CorrectDerivedFromBuildingUnitPositionsRequest,
        CorrigerenAfgeleidVanGebouwEenhedenResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AddressRepository _addressRepository;
        private readonly AddressRegistryApiClient _client;
        private readonly Channel<AddressCorrectionWorkItem> _channel;

        public CorrectDerivedFromBuildingUnitPositionsRequestHandler(
            IHttpContextAccessor httpContextAccessor,
            AddressRepository addressRepository,
            AddressRegistryApiClient client,
            Channel<AddressCorrectionWorkItem> channel)
        {
            _httpContextAccessor = httpContextAccessor;
            _addressRepository = addressRepository;
            _client = client;
            _channel = channel;
        }

        public async Task<CorrigerenAfgeleidVanGebouwEenhedenResponse> Handle(CorrectDerivedFromBuildingUnitPositionsRequest request,
            CancellationToken cancellationToken)
        {
            var addresses = await _addressRepository.GetAddressesToCorrectPosition(request.AddressIds);
            if (request.AddressIds is null)
            {
                await _channel.Writer.WriteAsync(
                    new AddressCorrectionWorkItem(
                        addresses.ToList(),
                        _httpContextAccessor.HttpContext!.Response.Headers[AddCorrelationIdToResponseMiddleware.HeaderName],
                        _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization]
                    ), cancellationToken);

                return new CorrigerenAfgeleidVanGebouwEenhedenResponse
                {
                    Aantal = addresses.Count
                };
            }

            await ProcessAddresses(addresses, cancellationToken);

            return new CorrigerenAfgeleidVanGebouwEenhedenResponse
            {
                Aantal = addresses.Count
            };
        }

        private async Task ProcessAddresses(ICollection<AddressWithGeometry> addressesToProcess, CancellationToken cancellationToken)
        {
            foreach (var address in addressesToProcess)
            {
                var geometry = new WKTReader().Read(address.Geometry);
                var geometryAsGml = GetGml(geometry, address.Srid);

                var apiRequest = new CorrectAddressPositionRequest
                {
                    PositieGeometrieMethode = PositieGeometrieMethode.AfgeleidVanObject,
                    PositieSpecificatie = PositieSpecificatie.Gebouweenheid,
                    Positie = geometryAsGml
                };

                await _client.CorrectAddressPosition(
                    address.PersistentLocalId,
                    apiRequest,
                    _httpContextAccessor.HttpContext!.Response.Headers[AddCorrelationIdToResponseMiddleware.HeaderName],
                    _httpContextAccessor.HttpContext!.Request.Headers[HeaderNames.Authorization],
                    cancellationToken);
            }
        }

        private const int SridLambert72 = 31370;
        private const int SridLambert08 = 3812;

        /// <summary>
        /// Builds the GML for the address registry. The srsName must match the SRID of the coordinates:
        /// the address registry interprets the coordinates according to the srsName.
        /// </summary>
        public static string GetGml(Geometry geometry, int srid)
        {
            if (srid is not (SridLambert72 or SridLambert08))
            {
                throw new System.InvalidOperationException($"Unsupported SRID {srid}, expected {SridLambert72} or {SridLambert08}.");
            }

            var builder = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true };
            using (var xmlWriter = XmlWriter.Create(builder, settings))
            {
                xmlWriter.WriteStartElement("gml", "Point", "http://www.opengis.net/gml/3.2");
                xmlWriter.WriteAttributeString("srsName", $"https://www.opengis.net/def/crs/EPSG/0/{srid}");
                Write(geometry.Coordinate, xmlWriter);
                xmlWriter.WriteEndElement();
            }

            return builder.ToString();
        }

        private static void Write(Coordinate coordinate, XmlWriter writer)
        {
            writer.WriteStartElement("gml", "pos", "http://www.opengis.net/gml/3.2");
            writer.WriteValue(string.Format(Global.GetNfi(),
                "{0} {1}",
                coordinate.X.ToPointGeometryCoordinateValueFormat(),
                coordinate.Y.ToPointGeometryCoordinateValueFormat()));
            writer.WriteEndElement();
        }
    }
}
