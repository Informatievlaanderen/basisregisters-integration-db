﻿namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api
{
    using System;

    public abstract class Organisatie
    {
        public Guid Id { get; }
        public string Naam { get; }
        public string? OvoCode { get; }
        public string? KboNummer { get; }

        protected Organisatie(Guid id, string naam, string? ovoCode, string? kboNummer)
        {
            Id = id;
            Naam = naam;
            OvoCode = ovoCode;
            KboNummer = kboNummer;
        }
    }

    public class BehandelendeOrganisatie : Organisatie
    {
        public BehandelendeOrganisatie(Guid id, string naam, string? ovoCode, string? kboNummer)
            : base(id, naam, ovoCode, kboNummer)
        { }
    }

    public class IndienerOrganisatie : Organisatie
    {
        private IndienerOrganisatie(Guid id, string naam, string? ovoCode, string? kboNummer)
            : base(id, naam, ovoCode, kboNummer)
        { }

        public static IndienerOrganisatie CreatePubliekeOrganisatie(Guid id, string naam, string ovoCode)
            => new(
                id,
                naam,
                ovoCode,
                null);

        public static IndienerOrganisatie CreateGeregistreerdeOrganisatie(Guid id, string naam, string kboNummer, string? ovoCode)
            => new(
                id,
                naam,
                ovoCode,
                kboNummer);
    }
}
