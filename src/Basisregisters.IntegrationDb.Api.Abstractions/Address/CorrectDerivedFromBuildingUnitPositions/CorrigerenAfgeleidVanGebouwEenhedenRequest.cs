namespace Basisregisters.IntegrationDb.Api.Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

[DataContract(Name = "CorrectieAfgeleidVanGebouwEenhedenRequest", Namespace = "")]
public sealed class CorrigerenAfgeleidVanGebouwEenhedenRequest
{
    [DataMember(Name = "adressen", Order = 0)]
    [JsonProperty(Required = Required.Default)]
    public List<string>? Adressen { get; set; }
}

public sealed class CorrigerenAfgeleidVanGebouwEenhedenRequestExamples : IExamplesProvider<CorrigerenAfgeleidVanGebouwEenhedenRequest>
{
    public CorrigerenAfgeleidVanGebouwEenhedenRequest GetExamples()
    {
        return new CorrigerenAfgeleidVanGebouwEenhedenRequest
        {
            Adressen = new List<string> { "https://data.vlaanderen.be/id/adres/200001" }
        };
    }
}
