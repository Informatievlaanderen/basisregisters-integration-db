namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using HouseNumberBoxNumber;

    public class NationalRegistryAddress
    {
        private readonly FlatFileRecord _record;

        public string NisCode => _record.NisCode;
        public string PostalCode => _record.PostalCode;
        public string StreetCode => _record.StreetCode;
        public HouseNumberBoxNumbersBase HouseNumberBoxNumbers { get; }

        public string StreetName => _record.StreetName;
        public int RegisteredCount => _record.RegisteredCount;

        public NationalRegistryAddress(FlatFileRecord record)
        {
            _record = record;

            var types = new List<HouseNumberBoxNumbersBase>
            {
                new NoIndex(_record.HouseNumber, _record.Index),
                new SpecificPrefix(_record.HouseNumber, _record.Index),
                new NonNumericBetweenNumbers(_record.HouseNumber, _record.Index),
                new SeparatorBetweenNumbers(_record.HouseNumber, _record.Index),
                new NumericFollowedBySpecificSuffix(_record.HouseNumber, _record.Index),
                new NumericFollowedByNonNumeric(_record.HouseNumber, _record.Index),
                new NumbersOnly(_record.HouseNumber, _record.Index),
                new NonNumericFollowedByZeros(_record.HouseNumber, _record.Index),
                new NonNumericFollowedByNumberGreaterThanZero(_record.HouseNumber, _record.Index)
            };

            foreach (var type in types)
            {
                if (type.Matches())
                {
                    HouseNumberBoxNumbers = type;
                    break;
                }
            }
        }
    }
}
