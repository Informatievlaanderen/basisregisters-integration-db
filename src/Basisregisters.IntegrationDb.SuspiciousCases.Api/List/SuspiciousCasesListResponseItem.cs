namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.List
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [DataContract(Name = "VerdachtGeval", Namespace = "")]
    public sealed class SuspiciousCasesListResponseItem
    {
        /// <summary>
        /// Type van het verdacht geval. Dit is uitbreidbaar in de toekomst met nieuwe verdachte gevallen.
        /// </summary>
        [DataMember(Name = "Type", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Type { get; set; }

        /// <summary>
        /// Categorie van het verdacht geval. Mogelijkheden: Adres, Perceel, Wegverbinding, Gebouw en Gebouweenheid.
        /// </summary>
        [DataMember(Name = "Categorie", Order = 2)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Categorie Category { get; set; }

        /// <summary>
        /// Ernst van het verdacht geval: Foutief, Verdacht en Verbeterbaar.
        /// </summary>
        [DataMember(Name = "Ernst", Order = 3)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Ernst Severity { get; set; }

        /// <summary>
        /// Naam van het verdacht geval. Dit is uitbreidbaar in de toekomst met nieuwe verdachte gevallen.
        /// </summary>
        [DataMember(Name = "Aantal", Order = 4)]
        [JsonProperty(Required = Required.DisallowNull)]
        public int Count { get; set; }

        /// <summary>
        /// De URL die de details van het verdacht geval weergeeft.
        /// </summary>
        [DataMember(Name = "Detail", Order = 5)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Uri Detail { get; set; }

        public SuspiciousCasesListResponseItem(string type, Categorie category, Ernst severity, int count, Uri detail)
        {
            Type = type;
            Category = category;
            Severity = severity;
            Count = count;
            Detail = detail;
        }
    }
}
