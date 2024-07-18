namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Exceptions
{
    using System;

    public class MeldingsobjectDeserializationFailedException : Exception
    {
        public int Position { get; set; }
        public string Json { get; set; }

        public MeldingsobjectDeserializationFailedException(
            int position,
            string json,
            Exception innerException)
            : base($"Failed to deserialize meldingsobject event at position {position}", innerException)
        {
            Position = position;
            Json = json;
        }
    }
}
