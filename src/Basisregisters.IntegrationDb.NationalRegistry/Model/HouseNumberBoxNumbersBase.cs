namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public abstract class HouseNumberBoxNumbersBase
    {
        protected readonly string PreCalculatedHouseNumber;

        public NationalRegistryIndex? Index { get; }

        public abstract bool Matches();
        public abstract IEnumerable<HouseNumberWithBoxNumber> GetValues();

        protected HouseNumberBoxNumbersBase(string houseNumber, NationalRegistryIndex? index)
        {
            PreCalculatedHouseNumber = houseNumber;
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
    }

    public sealed record HouseNumberWithBoxNumber(string HouseNumber, string? BoxNumber)
    {
        public bool HasBoxNumber => !string.IsNullOrEmpty(BoxNumber);
    }
}
