namespace Basisregisters.IntegrationDb.Api.Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

[DataContract(Name = "CorrectieAfgeleidVanGebouwEenhedenResponse", Namespace = "")]
public sealed class CorrigerenAfgeleidVanGebouwEenhedenResponse
{
    [DataMember(Name = "aantal", Order = 0)]
    [JsonProperty(Required = Required.Always)]
    public required int Aantal { get; set; }
}

public sealed class CorrigerenAfgeleidVanGebouwEenhedenResponseExample : IExamplesProvider<CorrigerenAfgeleidVanGebouwEenhedenResponse>
{
    public CorrigerenAfgeleidVanGebouwEenhedenResponse GetExamples()
    {
        return new CorrigerenAfgeleidVanGebouwEenhedenResponse
        {
            Aantal = 1
        };
    }
}
