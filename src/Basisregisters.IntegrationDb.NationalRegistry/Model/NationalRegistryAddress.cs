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

            foreach (var type in GetHouseNumberBoxNumbers())
            {
                if (type.Matches())
                {
                    HouseNumberBoxNumbers = type;
                    break;
                }
            }
        }

        private IEnumerable<HouseNumberBoxNumbersBase> GetHouseNumberBoxNumbers()
        {
            yield return new NoIndex(_record.HouseNumber, _record.Index);
            yield return new SpecificPrefix(_record.HouseNumber, _record.Index);
            yield return new NonNumericBetweenNumbers(_record.HouseNumber, _record.Index);
            yield return new SeparatorBetweenNumbers(_record.HouseNumber, _record.Index);
            yield return new NumericFollowedBySpecificSuffix(_record.HouseNumber, _record.Index);
            yield return new NumericFollowedByNonNumeric(_record.HouseNumber, _record.Index);
            yield return new NumbersOnly(_record.HouseNumber, _record.Index);
            yield return new NonNumericFollowedByZeros(_record.HouseNumber, _record.Index);
            yield return new NonNumericFollowedByNumberGreaterThanZero(_record.HouseNumber, _record.Index);
        }
    }
}
