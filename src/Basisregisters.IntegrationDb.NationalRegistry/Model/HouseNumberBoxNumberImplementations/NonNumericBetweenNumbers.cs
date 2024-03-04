namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Slide 34
    /// aangeleverde index waarvan deel 1 numeriek, deel 3 niet-numeriek en deel 4 numeriek
    /// </summary>
    public sealed class NonNumericBetweenNumbers : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            if (int.TryParse(Index.Left, out _)
                && !IsNumeric(Index.RightPartOne) && IsNumeric(Index.RightPartTwo)
                && Indications.FloorNumber.Contains(Index.RightPartOne, StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (int.TryParse(Index.Left, out _)
                && !IsNumeric(Index.RightPartOne) && int.TryParse(Index.RightPartTwo, out _))
            {
                return true;
            }

            return false;
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            // deel 3 aanduiding van verdiepnummer: aangeleverd huisnummer wordt huisnummer, deel 1 + ‘.’ + deel 4 wordt busnummer
            if (int.TryParse(Index.Left, out _)
                && !IsNumeric(Index.RightPartOne) && IsNumeric(Index.RightPartTwo)
                && Indications.FloorNumber.Contains(Index.RightPartOne, StringComparer.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{Index.Left}.{Index.RightPartTwo}")
                };
            }

            // deel 3 aanduiding van bisnummer: aangeleverd huisnummer + deel3 wordt huisnummer, deel 4 wordt busnummer
            if (int.TryParse(Index.Left, out _)
                && !IsNumeric(Index.RightPartOne) && int.TryParse(Index.RightPartTwo, out var rightPartTwo))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        $"{SourceSourceHouseNumber}{Index.RightPartOne}",
                        rightPartTwo.ToString())
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }

        public NonNumericBetweenNumbers(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
