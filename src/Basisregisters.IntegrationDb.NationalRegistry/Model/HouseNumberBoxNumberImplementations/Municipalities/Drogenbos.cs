namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Drogenbos : MunicipalityHouseNumberBoxNumbersBase
    {
        public Drogenbos(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "23098" &&
            (
                IndexSourceValue!.Contains("e/", StringComparison.InvariantCultureIgnoreCase)
                ||
                IndexSourceValue!.StartsWith("RCH", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (IndexSourceValue!.Contains("e/", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue!.Replace("e/", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim()
                    )
                };
            }

            if (IndexSourceValue!.StartsWith("RCH", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        IndexSourceValue!.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
