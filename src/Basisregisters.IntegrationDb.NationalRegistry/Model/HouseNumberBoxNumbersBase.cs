namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public abstract class HouseNumberBoxNumbersBase
    {
        protected readonly string SourceSourceHouseNumber;

        public NationalRegistryIndex? Index { get; }

        public abstract bool Matches();
        public abstract IEnumerable<HouseNumberWithBoxNumber> GetValues();

        protected HouseNumberBoxNumbersBase(string sourceHouseNumber, NationalRegistryIndex? index)
        {
            SourceSourceHouseNumber = sourceHouseNumber;
            Index = index;
        }

        protected static bool ContainsOnlyLetters(string input)
        {
            var regex = new Regex("^[a-zA-Z]+$", RegexOptions.Compiled);
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
