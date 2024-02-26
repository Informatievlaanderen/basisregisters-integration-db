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
                if (_record.Index.Left!.Equals("Ap", StringComparison.InvariantCultureIgnoreCase)
                    || _record.Index.Left!.Equals("Vrd", StringComparison.InvariantCultureIgnoreCase)
                    || _record.Index.Left!.Equals("bus", StringComparison.InvariantCultureIgnoreCase))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = _record.Index.Right!.TrimStart('0');
                }
                else if (int.TryParse(_record.Index.Left!, out _)
                         && !IsNumeric(_record.Index.RightPartOne) && IsNumeric(_record.Index.RightPartTwo)
                         && _record.Index.Right!.StartsWith("V.", StringComparison.InvariantCultureIgnoreCase))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = $"{_record.Index.Left}.{_record.Index.RightPartTwo}";
                }
                else if (int.TryParse(_record.Index.Left!, out _)
                         && !IsNumeric(_record.Index.RightPartOne) && int.TryParse(_record.Index.RightPartTwo, out var rightPartTwo))
                {
                    HouseNumber = _record.HouseNumber + _record.Index.RightPartOne;
                    BoxNumber = $"{rightPartTwo}";
                }
                else if (int.TryParse(_record.Index.Left!, out _)
                         && IsNumeric(_record.Index.RightPartOne) && string.IsNullOrEmpty(_record.Index.RightPartTwo))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = _record.Index.SourceValue;
                }
                else if (int.TryParse(_record.Index.Left!, out var left) // Slide 35 + 36
                         && !string.IsNullOrEmpty(_record.Index.RightPartOne) && !IsNumeric(_record.Index.RightPartOne)
                         && string.IsNullOrEmpty(_record.Index.RightPartTwo))
                {
                    if (new[] { "ev", "vrd" }.Any(x => x.Equals(_record.Index.RightPartOne, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        HouseNumber = _record.HouseNumber;
                        BoxNumber = $"{left}.0";
                    }
                    else
                    {
                        HouseNumber = $"{_record.HouseNumber}_{left}";
                        BoxNumber = _record.Index.RightPartOne;
                    }
                }
                else if (int.TryParse(_record.Index.Left!, out left) && string.IsNullOrEmpty(_record.Index.Right))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = $"{left}";
                }
                else if (new HouseNumberWithBisNumberAndNoBoxNumber(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new HouseNumberWithBisNumberAndNoBoxNumber(_record.HouseNumber, _record.Index);
                    HouseNumber = houseNumberWithBoxNumbers.GetValues().First().HouseNumber;
                    BoxNumber = houseNumberWithBoxNumbers.GetValues().First().BoxNumber;
                }
                else if (new NonNumericHouseNumberWithBisNumberAndNumericBoxNumber(_record.HouseNumber, _record.Index).Matches())
                {
                    var houseNumberWithBoxNumbers = new NonNumericHouseNumberWithBisNumberAndNumericBoxNumber(_record.HouseNumber, _record.Index);
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
