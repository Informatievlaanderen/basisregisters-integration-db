namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;

    public class Drogenbos : HouseNumberBoxNumbersBase
    {
        public Drogenbos(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "23098" &&
            (
                Index.SourceValue!.Contains("e/", StringComparison.InvariantCultureIgnoreCase)
                ||
                Index.SourceValue!.StartsWith("RCH", StringComparison.InvariantCultureIgnoreCase)
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.Contains("e/", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue!.Replace("e/", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim()
                    )
                };
            }

            if (Index.SourceValue!.StartsWith("RCH", StringComparison.InvariantCultureIgnoreCase))
            {
                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        Index.SourceValue!.Trim()
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
