namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract(Name = "VerdachtGevalCollectie", Namespace = "")]
    public sealed class SuspiciousCasesDetailResponse
    {
        /// <summary>
        /// De URL die de details van de meest recente versie van het object weergeeft.
        /// </summary>
        [DataMember(Name = "Details", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<Uri> Details { get; set; }

        public SuspiciousCasesDetailResponse(List<Uri> details)
        {
            Details = details;
        }
    }
}
