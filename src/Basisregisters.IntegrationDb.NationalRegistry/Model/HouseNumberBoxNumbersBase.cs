namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public abstract class HouseNumberBoxNumbersBase
    {
        protected readonly string NisCode;
        protected readonly string SourceSourceHouseNumber;

        public NationalRegistryIndex Index { get; }

        public abstract bool IsMatch();
        public abstract IList<HouseNumberWithBoxNumber> GetValues();

        protected HouseNumberBoxNumbersBase(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
        {
            NisCode = nisCode;
            SourceSourceHouseNumber = sourceHouseNumber.TrimStart('0');
            Index = index;
        }

        protected static bool ContainsOnlyLetters(string input)
        {
            var regex = new Regex("^[a-zA-Z]+$", RegexOptions.Compiled);
            return regex.IsMatch(input);
        }

        protected static bool ContainsOnlyCapitalLetters(char input)
        {
            return ContainsOnlyCapitalLetters(input.ToString());
        }

        protected static bool ContainsOnlyCapitalLetters(string input)
        {
            var regex = new Regex("^[A-Z]+$", RegexOptions.Compiled);
            return regex.IsMatch(input);
        }

        protected static bool ContainsOnlyZeroes(string input)
        {
            var regex = new Regex("^(0{1,3})$", RegexOptions.Compiled);
            return regex.IsMatch(input);
        }

        protected bool IsGreaterThanZero(string input)
        {
            return int.TryParse(input, out var number) && number > 0;
        }

        protected bool IsNumeric(string? input)
        {
            return int.TryParse(input, out _);
        }
    }

    public sealed record HouseNumberWithBoxNumber(string HouseNumber, string? BoxNumber)
    {
        public bool HasBoxNumber => !string.IsNullOrEmpty(BoxNumber);
    }
}
