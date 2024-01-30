namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.List
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Converters;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Swashbuckle.AspNetCore.Filters;

    [DataContract(Name = "VerdachteGevallenCollectie", Namespace = "")]
    public sealed class SuspiciousCasesListResponse
    {
        /// <summary>
        /// De verzameling van verdachte gevallen.
        /// </summary>
        [DataMember(Name = "resultaat", Order = 1)]
        [JsonProperty(Required = Required.DisallowNull)]
        public List<SuspiciousCasesListResponseItem> Results { get; set; }

        public SuspiciousCasesListResponse(List<SuspiciousCasesListResponseItem> results)
        {
            Results = results;
        }
    }

    public class SuspiciousCasesListResponseExample : IExamplesProvider<SuspiciousCasesListResponse>
    {
        private readonly IOptions<ResponseOptions> _responseOptions;

        public SuspiciousCasesListResponseExample(IOptions<ResponseOptions> responseOptions)
        {
            _responseOptions = responseOptions;
        }

        public SuspiciousCasesListResponse GetExamples()
        {
            return new SuspiciousCasesListResponse(
                new List<SuspiciousCasesListResponseItem>
                {
                    new SuspiciousCasesListResponseItem(
                        SuspiciousCase.AllCases[SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits].Description,
                        SuspiciousCase.AllCases[SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits].Category.Map(),
                        SuspiciousCase.AllCases[SuspiciousCasesType.CurrentAddressWithoutLinkedParcelsOrBuildingUnits].Severity.Map(),
                        45,
                        new Uri(string.Format(_responseOptions.Value.SuspiciousCasesTypeUrl, 1))),
                    new SuspiciousCasesListResponseItem(
                        SuspiciousCase.AllCases[SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed].Description,
                        SuspiciousCase.AllCases[SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed].Category.Map(),
                        SuspiciousCase.AllCases[SuspiciousCasesType.StreetNamesLongerThanTwoYearsProposed].Severity.Map(),
                        5,
                        new Uri(string.Format(_responseOptions.Value.SuspiciousCasesTypeUrl, 5)))
                });
        }
    }
}
