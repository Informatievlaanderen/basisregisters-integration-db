﻿namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Slide 36
    /// aangeleverde index waarvan deel 1 numeriek, deel 3 niet-numeriek en zonder deel 4
    /// -> deel 3 aanduiding van verdiepnummer: aangeleverd huisnummer wordt huisnummer, deel 1 + ‘.0’ wordt busnummer
    /// </summary>
    public sealed class NumericFollowedBySpecificSuffix : HouseNumberBoxNumbersBase
    {

        public override bool IsMatch()
        {
            return int.TryParse(Index.Left, out _)
                   && !string.IsNullOrEmpty(Index.RightPartOne) && !IsNumeric(Index.RightPartOne)
                   && Indications.FloorNumber.Any(x => x.Equals(Index.RightPartOne, StringComparison.InvariantCultureIgnoreCase))
                   && string.IsNullOrEmpty(Index.RightPartTwo);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    HouseNumberSourceValue,
                    $"{int.Parse(Index.Left!)}.0"
                )
            };
        }

        public NumericFollowedBySpecificSuffix(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
