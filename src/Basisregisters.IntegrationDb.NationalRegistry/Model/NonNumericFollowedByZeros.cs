namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System;
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
        public override bool Matches()
        {
            return ContainsOnlyLetters(Index!.Left!) && ContainsOnlyZeroes(Index.Right!);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{Index!.Left}",
                    null)
            };
        }

        public NonNumericFollowedByZeros(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 30
    /// aangeleverde index waarvan
    ///     deel 1 niet-numeriek
    ///     en deel 2 numeriek en groter dan nul (0)
    /// -> deel 1 aanduiding van bisnummer: aangeleverd huisnummer + deel 1 wordt huisnummer; deel 2 wordt busnummer
    /// </summary>
    public sealed class NonNumericFollowedByNumberGreaterThanZero : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return ContainsOnlyLetters(Index!.Left!) && IsGreaterThanZero(Index.Right!);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{Index!.Left}",
                    Index.Right!.TrimStart('0'))
            };
        }

        public NonNumericFollowedByNumberGreaterThanZero(string sourceHouseNumber, NationalRegistryIndex? index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 31
    /// deel 1 geen aanduiding van bisnummer: aangeleverd huisnummer wordt huisnummer
    /// </summary>
    public sealed class SpecificPrefix : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return Index!.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                   || Index.Left!.Equals("Vrd", StringComparison.InvariantCultureIgnoreCase)
                   || Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            // deel 1 aanduiding van appartementnummer: deel 2 wordt busnummer
            // deel 1 aanduiding van busnummer: deel 2 wordt busnummer
            if (Index!.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                || Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index!.Right)
                };
            }

            // deel 1 aanduiding van verdiepnummer: deel1 + deel2 worden busnummer
            if (Index.Left!.Equals("Vrd", StringComparison.InvariantCultureIgnoreCase))
            {

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        $"{Index!.Left}{Index.Right}")
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }

        public SpecificPrefix(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 32
    /// aangeleverde index waarvan deel 1 numeriek en zonder deel 2 en zonder deel 3
    /// -> aangeleverd huisnummer wordt huisnummer, deel 1 wordt busnummer
    /// </summary>
    public sealed class NumbersOnly : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            throw new NotImplementedException();
        }

        public NumbersOnly(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 33
    /// aangeleverde index waarvan deel 1 numeriek, deel 3 numeriek en zonder deel 4
    /// -> aangeleverd huisnummer wordt huisnummer, aangeleverde index wordt busnummer
    /// </summary>
    public sealed class SeparatorBetweenNumbers : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            return int.TryParse(Index!.Left, out _)
                   && IsNumeric(Index.RightPartOne) && string.IsNullOrEmpty(Index.RightPartTwo);
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            return new[]
            {
                new HouseNumberWithBoxNumber(
                    SourceSourceHouseNumber,
                    Index!.SourceValue)
            };
        }

        public SeparatorBetweenNumbers(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 34
    /// aangeleverde index waarvan deel 1 numeriek, deel 3 niet-numeriek en deel 4 numeriek
    /// </summary>
    public sealed class NonNumericBetweenNumbers : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            if (int.TryParse(Index!.Left, out _)
                && !IsNumeric(Index.RightPartOne) && IsNumeric(Index.RightPartTwo)
                && Index.Right!.StartsWith("V.", StringComparison.InvariantCultureIgnoreCase))
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

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            // deel 3 aanduiding van verdiepnummer: aangeleverd huisnummer wordt huisnummer, deel 1 + ‘.’ + deel 4 wordt busnummer
            if (int.TryParse(Index!.Left, out _)
                && !IsNumeric(Index.RightPartOne) && IsNumeric(Index.RightPartTwo)
                && Index.Right!.StartsWith("V.", StringComparison.InvariantCultureIgnoreCase))
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

    /// <summary>
    /// Slide 35
    /// Deel 1 is numeriek en deel 3 aanduiding van appartement
    /// -> aangeleverd huisnummer + ‘_’ + deel 1 wordt huisnummer, deel 3 wordt busnummer
    /// </summary>
    public sealed class NumericFollowedByNonNumeric : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            throw new NotImplementedException();
        }

        public NumericFollowedByNonNumeric(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }

    /// <summary>
    /// Slide 36
    /// aangeleverde index waarvan deel 1 numeriek, deel 3 niet-numeriek en zonder deel 4
    /// -> deel 3 aanduiding van verdiepnummer: aangeleverd huisnummer wordt huisnummer, deel 1 + ‘.0’ wordt busnummer
    /// </summary>
    public sealed class NumericFollowedBySpecificSuffix : HouseNumberBoxNumbersBase
    {
        public override bool Matches()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HouseNumberWithBoxNumber> GetValues()
        {
            throw new NotImplementedException();
        }

        public NumericFollowedBySpecificSuffix(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
