namespace Basisregisters.IntegrationDb.Meldingen
{
    using System;

    public class Organisatie
    {
        public Guid Id { get; set; }
        // Provided the Organisatie with an own Id besides the GTMF Id. Can be useful in some cases: fusies, clean up GTMF organizations (Geosecure), ...
        public Guid IdInternal { get; set; }
        public string Naam { get; set; }
        public string? OvoCode { get; set; }
        public string? KboNummer { get; set; }

        private Organisatie()
        { }

        public Organisatie(
            Guid id,
            Guid idInternal,
            string naam,
            string? ovoCode,
            string? kboNummer)
        {
            IdInternal = idInternal;
            Id = id;
            Naam = naam;
            OvoCode = ovoCode;
            KboNummer = kboNummer;
        }
    }
}
