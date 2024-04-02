namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System;
    using System.Collections.Generic;
    using HouseNumberBoxNumberImplementations;
    using HouseNumberBoxNumberImplementations.Municipalities;

    public class NationalRegistryAddress
    {
        private readonly FlatFileRecord _record;

        public string NisCode => _record.NisCode;
        public string PostalCode => _record.PostalCode;
        public string StreetCode => _record.StreetCode;
        public HouseNumberBoxNumbersBase? HouseNumberBoxNumbers { get; }
        public Type? HouseNumberBoxNumbersType => HouseNumberBoxNumbers?.GetType();

        public string StreetName => _record.StreetName;
        public int RegisteredCount => _record.RegisteredCount;

        public NationalRegistryAddress(FlatFileRecord record)
        {
            _record = record;

            foreach (var houseNumberBoxNumbers in GetHouseNumberBoxNumbers())
            {
                if (houseNumberBoxNumbers.IsMatch())
                {
                    HouseNumberBoxNumbers = houseNumberBoxNumbers;
                    break;
                }
            }
        }

        private IEnumerable<HouseNumberBoxNumbersBase> GetHouseNumberBoxNumbers()
        {
            yield return new NoIndex(_record.NisCode, _record.HouseNumber, _record.Index);

            yield return new Turnhout(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Lier(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Aartselaar(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Wommelgem(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Hemiksem(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Vilvoorde(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Drogenbos(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Borsbeek(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Grimbergen(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Beveren(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Zwijndrecht(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Wemmel(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Boom(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Ieper(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Tongeren(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Lint(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Machelen(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Beerse(_record.NisCode, _record.HouseNumber, _record.Index);

            yield return new BisIndication(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new SpecificPrefix(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new NonNumericBetweenNumbers(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new SeparatorBetweenNumbers(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new NumericFollowedBySpecificSuffix(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new NumericFollowedByNonNumeric(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new NumbersOnly(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new NonNumericFollowedByZeros(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new NonNumericFollowedByNumberGreaterThanZero(_record.NisCode, _record.HouseNumber, _record.Index);
        }
    }
}
