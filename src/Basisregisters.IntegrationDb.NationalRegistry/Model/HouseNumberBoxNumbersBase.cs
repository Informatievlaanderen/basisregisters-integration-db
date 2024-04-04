namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public abstract partial class HouseNumberBoxNumbersBase
    {
        protected readonly string NisCode;
        protected readonly string HouseNumberSourceValue;

        public NationalRegistryIndex Index { get; }

        public abstract bool IsMatch();
        public abstract IList<HouseNumberWithBoxNumber> GetValues();

        protected HouseNumberBoxNumbersBase(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
        {
            NisCode = nisCode;
            HouseNumberSourceValue = sourceHouseNumber.TrimStart('0');
            Index = index;
        }

        [GeneratedRegex("^[A-Z]+$", RegexOptions.Compiled)]
        private static partial Regex CapitalLettersOnlyRegex();

        [GeneratedRegex("^[a-zA-Z]+$", RegexOptions.Compiled)]
        private static partial Regex LettersOnlyRegex();

        [GeneratedRegex("^(0{1,3})$", RegexOptions.Compiled)]
        private static partial Regex ZeroesOnlyRegex();

        protected static bool IsLetter(char input) => char.IsLetter(input);

        protected static bool ContainsOnlyLetters(string input) => LettersOnlyRegex().IsMatch(input);

        protected static bool ContainsOnlyCapitalLetters(char input) => ContainsOnlyCapitalLetters(input.ToString());

        protected static bool ContainsOnlyCapitalLetters(string input) => CapitalLettersOnlyRegex().IsMatch(input);

        protected static bool ContainsOnlyZeroes(string input) => ZeroesOnlyRegex().IsMatch(input);

        protected static bool IsNumberGreaterThanZero(string input) => int.TryParse(input, out var number) && number > 0;

        protected static bool IsNumeric(char input) => IsNumeric(input.ToString());
        protected static bool IsNumeric(string? input) => int.TryParse(input, out _);
    }

    public sealed record HouseNumberWithBoxNumber(string HouseNumber, string? BoxNumber)
    {
        public bool HasBoxNumber => !string.IsNullOrEmpty(BoxNumber);
    }
}
