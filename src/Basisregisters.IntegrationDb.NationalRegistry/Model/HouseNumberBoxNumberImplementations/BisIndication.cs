﻿namespace Basisregisters.IntegrationDb.NationalRegistry.Model.HouseNumberBoxNumberImplementations
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
                    $"{HouseNumberSourceValue}{bisNumber}",
                    null)
            };
        }

        public BisIndication(
            string nisCode,
            string sourceHouseNumber,
            NationalRegistryIndex index)
            : base(nisCode, sourceHouseNumber, index)
        { }
    }
}
