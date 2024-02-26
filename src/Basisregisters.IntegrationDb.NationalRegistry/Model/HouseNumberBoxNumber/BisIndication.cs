namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumber
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public sealed class BisIndication : HouseNumberBoxNumbersBase
    {
        public override bool IsMatch()
        {
            return Index.SourceValue!.Contains("bis", StringComparison.InvariantCultureIgnoreCase)
                || Index.SourceValue!.Contains("ter", StringComparison.InvariantCultureIgnoreCase);
        }

        public override IList<HouseNumberWithBoxNumber> GetValues()
        {
            const string pattern = "(bis|ter)";

            var bisNumber = Regex.Replace(Index.SourceValue!, pattern, string.Empty, RegexOptions.IgnoreCase);

            return new[]
            {
                new HouseNumberWithBoxNumber(
                    $"{SourceSourceHouseNumber}{bisNumber}",
                    null)
            };
        }

        public BisIndication(string sourceHouseNumber, NationalRegistryIndex index)
            : base(sourceHouseNumber, index)
        { }
    }
}
