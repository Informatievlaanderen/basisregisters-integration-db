namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.List
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract(Name = "VerdachteGevallenCollectie", Namespace = "")]
    public sealed class SuspiciousCasesListResponse
    {
        /// <summary>
        /// Collectie van verdachte gevallen.
        /// </summary>
        [DataMember(Name = "resultaat", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<SuspiciousCasesListResponseItem> VerdachteGevallen { get; set; }

        public SuspiciousCasesListResponse(List<SuspiciousCasesListResponseItem> verdachteGevallen)
        {
            VerdachteGevallen = verdachteGevallen;
        }
    }
}
