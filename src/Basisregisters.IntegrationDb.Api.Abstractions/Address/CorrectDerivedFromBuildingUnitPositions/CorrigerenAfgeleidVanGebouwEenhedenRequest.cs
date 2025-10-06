namespace Basisregisters.IntegrationDb.Api.Abstractions.Address.CorrectDerivedFromBuildingUnitPositions;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

[DataContract(Name = "CorrectieAfgeleidVanGebouwEenhedenRequest", Namespace = "")]
public sealed class CorrigerenAfgeleidVanGebouwEenhedenRequest
{
    [DataMember(Name = "adressen", Order = 0)]
    [JsonProperty(Required = Required.Default)]
    public List<int>? Adressen { get; set; }
}
