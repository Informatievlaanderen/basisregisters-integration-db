﻿namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Detail
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Infrastructure;
    using Newtonsoft.Json;
    using Views;

    [DataContract(Name = "VerdachtGevalCollectie", Namespace = "")]
    public sealed class SuspiciousCasesDetailResponse
    {
        /// <summary>
        /// Collectie van verdachte gevallen.
        /// </summary>
        [DataMember(Name = "resultaat", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<SuspiciousCasesDetailResponseItem> Results { get; set; }

        /// <summary>
        /// De URL voor het ophalen van de volgende verzameling.
        /// </summary>
        [DataMember(Name = "Volgende", Order = 1, EmitDefaultValue = false)]
        [JsonProperty(Required = Required.Default, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Uri? Next { get; set; }

        public SuspiciousCasesDetailResponse(List<SuspiciousCasesDetailResponseItem> results, Uri? next)
        {
            Results = results;
            Next = next;
        }
    }

    [DataContract(Name = "VerdachtGevalCollectieItem", Namespace = "")]
    public sealed class SuspiciousCasesDetailResponseItem
    {
        /// <summary>
        /// De URL die de details van de meest recente versie van het object weergeeft.
        /// </summary>
        [DataMember(Name = "Detail", Order = 0)]
        [JsonProperty(Required = Required.DisallowNull)]
        public Uri Detail { get; set; }

        /// <summary>
        /// De beschrijving van het verdachte geval.
        /// </summary>
        [DataMember(Name = "Beschrijving", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Description { get; set; }

        public SuspiciousCasesDetailResponseItem(Uri detail, string description)
        {
            Detail = detail;
            Description = description;
        }

        public SuspiciousCasesDetailResponseItem(SuspiciousCase suspiciousCase, ResponseOptions responseOptions)
        {
            Detail = suspiciousCase.Category switch
            {
                Category.Address => new Uri(string.Format(responseOptions.AddressDetailUrl, suspiciousCase.PersistentLocalId)),
                Category.Parcel => new Uri(string.Format(responseOptions.ParcelDetailUrl, suspiciousCase.PersistentLocalId)),
                Category.RoadSegment => new Uri(string.Format(responseOptions.RoadSegmentDetailUrl, suspiciousCase.PersistentLocalId)),
                Category.Building => new Uri(string.Format(responseOptions.BuildingDetailUrl, suspiciousCase.PersistentLocalId)),
                Category.BuildingUnit => new Uri(string.Format(responseOptions.BuildingUnitDetailUrl, suspiciousCase.PersistentLocalId)),
                Category.StreetName => new Uri(string.Format(responseOptions.StreetNameDetailUrl, suspiciousCase.PersistentLocalId)),
                _ => throw new ArgumentOutOfRangeException()
            };
            Description = suspiciousCase.Description;
        }
    }
}
