namespace Basisregisters.IntegrationDb.Api.Address.CorrectDerivedFromBuildingUnitPositions
{
    using System.Text;
    using System.Threading;
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

    public sealed class CorrectDerivedFromBuildingUnitPositionsRequestHandler : IRequestHandler<CorrectDerivedFromBuildingUnitPositionsRequest, CorrigerenAfgeleidVanGebouwEenhedenResponse>
    {
        private readonly AddressRepository _addressRepository;
        private readonly AddressRegistryApiClient _client;

        public CorrectDerivedFromBuildingUnitPositionsRequestHandler(
            AddressRepository addressRepository,
            AddressRegistryApiClient client)
        {
            _addressRepository = addressRepository;
            _client = client;
        }

        public async Task<CorrigerenAfgeleidVanGebouwEenhedenResponse> Handle(CorrectDerivedFromBuildingUnitPositionsRequest request, CancellationToken cancellationToken)
        {
            var addresses = await _addressRepository.GetAddressesToCorrectPosition(request.AddressIds);

            foreach (var address in addresses)
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

            return new CorrigerenAfgeleidVanGebouwEenhedenResponse
            {
                Aantal = addresses.Count
            };
        }

        private static string GetGml(Geometry geometry)
        {
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true };
            using (var xmlwriter = XmlWriter.Create(builder, settings))
            {
                xmlwriter.WriteStartElement("gml", "Point", "http://www.opengis.net/gml/3.2");
                xmlwriter.WriteAttributeString("srsName", "https://www.opengis.net/def/crs/EPSG/0/31370");
                Write(geometry.Coordinate, xmlwriter);
                xmlwriter.WriteEndElement();
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
