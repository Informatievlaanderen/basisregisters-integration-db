namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching
{
    public record HardCodedStreetName(string NisCode, string SearchValue, string GrarValue);

    public static class HardCodedStreetNames
    {
        public static readonly HardCodedStreetName[] StreetNames =
        {
            new HardCodedStreetName("11023", "OUDE -GRACHTLAAN", "Oude Gracht"),
            new HardCodedStreetName("23002", "DOORNVELD", "Z. 3 Doornveld"),
            new HardCodedStreetName("23081", "TRANSITCENTRUM 127-HAACHTSESTWG", "Haachtsesteenweg"),
            new HardCodedStreetName("34022", "MARCONILAAN(KOR)", "Guglielmo Marconilaan"),
            new HardCodedStreetName("71072", "ABDIS DE GOOR", "Abdis de Goorstraat"),
            new HardCodedStreetName("71072", "ABDIS DE MOMBEEK", "Abdis de Mombeekstraat"),
            new HardCodedStreetName("73066", "HOLSTRAAT-HEUKELOM", "Holstraat"),
            new HardCodedStreetName("34022", "STEYTS KOER(KOR)(NIEUWE STRAAT)", "Steyts Koer")
        };
    }
}
