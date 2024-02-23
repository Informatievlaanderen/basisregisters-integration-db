namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System;
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
                         && IsNumeric(_record.Index.RightPartOne) && string.IsNullOrEmpty(_record.Index.RightPartTwo))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = _record.Index.SourceValue;
                }
                else if (int.TryParse(_record.Index.Left!, out _) && string.IsNullOrEmpty(_record.Index.Right))
                {
                    HouseNumber = _record.HouseNumber;
                    BoxNumber = _record.Index.Left.TrimStart('0');
                }
                else if (ContainsOnlyLetters(_record.Index.Left!) && ContainsOnlyZeroes(_record.Index.Right!))
                {
                    HouseNumber = _record.HouseNumber + _record.Index.Left;
                    BoxNumber = null;
                }
                else if (ContainsOnlyLetters(_record.Index.Left!) && IsGreaterThanZero(_record.Index.Right!))
                {
                    HouseNumber = _record.HouseNumber + _record.Index.Left;
                    BoxNumber = _record.Index.Right!.TrimStart('0');
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
