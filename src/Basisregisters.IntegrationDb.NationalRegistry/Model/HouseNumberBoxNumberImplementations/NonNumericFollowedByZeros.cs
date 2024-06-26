﻿namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System.Collections.Generic;

    /// <summary>
    /// Slide 29
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en gelijk aan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; geen busnummer
    /// </summary>
    public sealed class NonNumericFollowedByZeros : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return ContainsOnlyLetters(Index.Left!) && ContainsOnlyZeroes(Index.Right!);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{HouseNumberSourceValue}{Index.Left}",
                    null)
            };
        }

        public NonNumericFollowedByZeros(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
