namespace Basisregisters.IntegrationDb.Api.Address.CorrectDerivedFromBuildingUnitPositions;

using System.Collections.Generic;
using Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;
using MediatR;

public sealed record CorrectDerivedFromBuildingUnitPositionsRequest(ICollection<int>? AddressIds)
    : IRequest<CorrigerenAfgeleidVanGebouwEenhedenResponse>;
