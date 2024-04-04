namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Collections.Generic;
    using HouseNumberBoxNumberImplementations;
    using HouseNumberBoxNumberImplementations.Municipalities;

    public class NationalRegistryAddress
    {
        private readonly FlatFileRecord _record;

        public IList<HouseNumberBoxNumbersBase> HouseNumberBoxNumbers { get; }

        public NationalRegistryAddress(FlatFileRecord record)
        {
            _record = record;
            HouseNumberBoxNumbers = new List<HouseNumberBoxNumbersBase>();

            var noIndex = new NoIndex(_record.NisCode, _record.HouseNumber, _record.Index);
            if (noIndex.IsMatch())
            {
                HouseNumberBoxNumbers.Add(noIndex);
            }
            else
            {
                foreach (var houseNumberBoxNumbers in GetHouseNumberBoxNumbersByMunicipality())
                {
                    if (houseNumberBoxNumbers.IsMatch())
                    {
                        HouseNumberBoxNumbers.Add(houseNumberBoxNumbers);
                        break;
                    }
                }

                foreach (var houseNumberBoxNumbers in GetHouseNumberBoxNumbers())
                {
                    if (houseNumberBoxNumbers.IsMatch())
                    {
                        HouseNumberBoxNumbers.Add(houseNumberBoxNumbers);
                        break;
                    }
                }
            }
        }

        private IEnumerable<HouseNumberBoxNumbersBase> GetHouseNumberBoxNumbersByMunicipality()
        {
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
            yield return new Zoersel(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Diest(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new SintNiklaas(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Vosselaar(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new DePanne(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Torhout(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Zele(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Aalter(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Ravels(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new SintGillisWaas(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Geraardsbergen(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Bornem(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Brecht(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Houthalen(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Olen(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Koksijde(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Lokeren(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Nijlen(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Mortsel(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Vorselaar(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Stabroek(_record.NisCode, _record.HouseNumber, _record.Index);
            yield return new Antwerpen(_record.NisCode, _record.HouseNumber, _record.Index);
        }

        private IEnumerable<HouseNumberBoxNumbersBase> GetHouseNumberBoxNumbers()
        {
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
