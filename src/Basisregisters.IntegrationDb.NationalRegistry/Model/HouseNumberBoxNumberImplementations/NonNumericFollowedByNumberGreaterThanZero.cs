namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Slide 30
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en groter dan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; deel 2 wordt busnummer
    /// </summary>
    public sealed class NonNumericFollowedByNumberGreaterThanZero : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return ContainsOnlyLetters(Index.Left!) && IsGreaterThanZero(Index.Right!);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            var left = Index.Left!;
            if (left.Length == 3
                && (left.EndsWith("bu", StringComparison.InvariantCultureIgnoreCase)
                || left.EndsWith("bt", StringComparison.InvariantCultureIgnoreCase)
                || left.EndsWith("ap", StringComparison.InvariantCultureIgnoreCase)))
            {
                left = left[0].ToString();
            }

            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{left}",
                    Index.Right!.TrimStart('0'))
            };
        }

        public NonNumericFollowedByNumberGreaterThanZero(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
