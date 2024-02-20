namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;

    public class AsIsSanitizer : SanitizerBase
    {
        public AsIsSanitizer() : base(Array.Empty<SanitizerBase>())
        { }

        protected override string InnerSanitize(string streetName)
        {
            return streetName;
        }
    }
}
