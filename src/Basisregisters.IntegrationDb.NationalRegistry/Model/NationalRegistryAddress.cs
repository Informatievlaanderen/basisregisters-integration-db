namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;


    public class NationalRegistryAddress
    {
        private readonly FlatFileRecord _record;

        public string NisCode => _record.NisCode;
        public string PostalCode => _record.PostalCode;
        public string StreetCode => _record.StreetCode;
        public string HouseNumber { get; }
        public string? BoxNumber { get; }

        public string StreetName => _record.StreetName;
        public int RegisteredCount => _record.RegisteredCount;

        public NationalRegistryAddress(FlatFileRecord record)
        {
            _record = record;

            if (_record.HasIndex)
            {
                if (new SpecificPrefix(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new SpecificPrefix(_record.HouseNumber, _record.Index);

                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (new NonNumericBetweenNumbers(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new NonNumericBetweenNumbers(_record.HouseNumber, _record.Index);

                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (new SeparatorBetweenNumbers(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new SeparatorBetweenNumbers(_record.HouseNumber, _record.Index);
                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (new NumericFollowedBySpecificSuffix(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new NumericFollowedBySpecificSuffix(_record.HouseNumber, _record.Index);
                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (new NumericFollowedByNonNumeric(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new NumericFollowedByNonNumeric(_record.HouseNumber, _record.Index);
                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (int.TryParse(_record.Index.Left!, out var left) && string.IsNullOrEmpty(_record.Index.Right))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = $"{left}";
                }
                else if (new NonNumericFollowedByZeros(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new NonNumericFollowedByZeros(_record.HouseNumber, _record.Index);
                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (new NonNumericFollowedByNumberGreaterThanZero(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new NonNumericFollowedByNumberGreaterThanZero(_record.HouseNumber, _record.Index);
                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
            }
            else
            {
                HouseNumber = _record.HouseNumber;
                BoxNumber = null;
            }
        }

        private bool IsNumeric(string? input)
        {
            return int.TryParse(input, out _);
        }

        private bool IsGreaterThanZero(string input)
        {
            return int.TryParse(input, out var number) && number > 0;
        }

        private static bool ContainsOnlyLetters(string input)
        {
            Regex regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(input);
        }

        private static bool ContainsOnlyZeroes(string input)
        {
            Regex regex = new Regex("^(0{1,3})$");
            return regex.IsMatch(input);
        }
    }
}
