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
    using Be.Vlaanderen.Basisregisters.GrAr.Common.SpatialTools.GeometryCoordinates;
    using Be.Vlaanderen.Basisregisters.GrAr.Edit.Contracts;
    using MediatR;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.IO;
    using NetTopologySuite.Utilities;
    using Repositories;

    public sealed class CorrectDerivedFromBuildingUnitPositionsRequestHandler : IRequestHandler<CorrectDerivedFromBuildingUnitPositionsRequest,
        CorrigerenAfgeleidVanGebouwEenhedenResponse>
    {
        private readonly AddressRepository _addressRepository;
        private readonly AddressRegistryApiClient _client;
        private readonly Channel<AddressCorrectionWorkItem> _channel;

        public CorrectDerivedFromBuildingUnitPositionsRequestHandler(
            AddressRepository addressRepository,
            AddressRegistryApiClient client,
            Channel<AddressCorrectionWorkItem> channel)
        {
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
                await _channel.Writer.WriteAsync(new AddressCorrectionWorkItem(addresses.ToList()), cancellationToken);

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
                var geometryAsGml = GetGml(geometry);

                var apiRequest = new CorrectAddressPositionRequest
                {
                    PositieGeometrieMethode = PositieGeometrieMethode.AfgeleidVanObject,
                    PositieSpecificatie = PositieSpecificatie.Gebouweenheid,
                    Positie = geometryAsGml
                };

                await _client.CorrectAddressPosition(address.PersistentLocalId, apiRequest, cancellationToken);
            }
        }

        public static string GetGml(Geometry geometry)
        {
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true };
            using (var xmlWriter = XmlWriter.Create(builder, settings))
            {
                xmlWriter.WriteStartElement("gml", "Point", "http://www.opengis.net/gml/3.2");
                xmlWriter.WriteAttributeString("srsName", "https://www.opengis.net/def/crs/EPSG/0/31370");
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
