namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations.Municipalities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Tongeren : HouseNumberBoxNumbersBase
    {
        public Tongeren(string nisCode, string sourceHouseNumber, NationalRegistryIndex index) : base(nisCode, sourceHouseNumber, index)
        { }

        public override bool IsMatch() =>
            NisCode == "73083" &&
            (
                Index.SourceValue!.StartsWith("00b") || Index.SourceValue!.StartsWith("0b")
            );

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            if (Index.SourceValue!.StartsWith("00b") || Index.SourceValue!.StartsWith("0b"))
            {
                var busNumber = Index.SourceValue!.Split('b').Last();

                return new[]
                {
                    new HouseNumberWithBoxNumber(
                        SourceSourceHouseNumber,
                        busNumber
                    )
                };
            }

            throw new InvalidOperationException("Invalid use of matches");
        }
    }
}
