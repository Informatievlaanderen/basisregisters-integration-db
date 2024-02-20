namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    using System;

    public sealed class AbbreviationSanitizer : SanitizerBase
    {
        public AbbreviationSanitizer() : this(Array.Empty<SanitizerBase>())
        { }

        public AbbreviationSanitizer(SanitizerBase[] sanitizers) : base(sanitizers)
        { }

        protected override string InnerSanitize(string streetName)
        {
            var replaced = streetName.ToLowerInvariant();

            if (replaced.Contains("onze lieve vrouw"))
                return replaced.Replace("onze lieve vrouw", "O.L. Vrouw");

            if (replaced.Contains("o.l.v."))
                return replaced.Replace("o.l.v.", "Onze Lieve Vrouw");

            if (replaced.Contains("o.-l.-"))
                return replaced.Replace("o.-l.-", "Onze-Lieve-");

            if (replaced.Contains("onze-lieve-"))
                return replaced.Replace("onze-lieve-", "O.L. ");

            if (replaced.Contains("sint"))
                return replaced.Replace("sint", "st.");

            if (replaced.Contains("st."))
                return replaced.Replace("st.", "sint");

            if (replaced.Contains("dr."))
                return replaced.Replace("dr.", "dokter");

            if (replaced.Contains("dokter"))
                return replaced.Replace("dokter", "dr.");

            if (replaced.Contains("stwg."))
                return replaced.Replace("stwg.", "steenweg");

            if (replaced.Contains("stwg"))
                return replaced.Replace("stwg", "steenweg");

            if (replaced.Contains("stw."))
                return replaced.Replace("stw.", "steenweg");

            if (replaced.Contains("stw"))
                return replaced.Replace("stw", "steenweg");

            if (replaced.Contains("burg."))
                return replaced.Replace("burg.", "Burgemeester");

            if (replaced.Contains("burgem."))
                return replaced.Replace("burg.", "Burgemeester");

            if (replaced.Contains("burgemeester"))
                return replaced.Replace("burgemeester", "burg.");

            if (replaced.EndsWith("str"))
                return replaced.Replace("str", "straat");

            if (replaced.EndsWith("str."))
                return replaced.Replace("str.", "straat");

            if (replaced.StartsWith("heilige"))
                return replaced.Replace("heilige", "h");

            if (replaced.StartsWith("heilig"))
                return replaced.Replace("heilig", "h");

            if (replaced.StartsWith("k."))
                return replaced.Replace("k.", "koning");

            if (replaced.StartsWith("kon."))
                return replaced.Replace("kon.", "koning");

            if (replaced.StartsWith("mgr."))
                return replaced.Replace("mgr.", "Monseigneur");

            if (replaced.StartsWith("volksv."))
                return replaced.Replace("volksv.", "Volksvertegenwoordiger");

            return replaced;
        }
    }
}
