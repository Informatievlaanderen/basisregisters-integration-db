namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Sanitizers
{
    public abstract class SanitizerBase
    {
        private readonly SanitizerBase[] _sanitizers;

        protected bool SanitizeStreetName = false;
        protected bool SanitizeSearch = true;

        protected SanitizerBase(SanitizerBase[] sanitizers)
        {
            _sanitizers = sanitizers;
        }

        public (string StreetName, string Search) Sanitize(string streetName, string search)
        {
            var sanitizedStreetName = SanitizeStreetName ? InnerSanitize(streetName) : streetName;
            var sanitizedSearch = SanitizeSearch ? InnerSanitize(search) : search;

            foreach (var sanitizer in _sanitizers)
            {
               (sanitizedStreetName, sanitizedSearch) = sanitizer.Sanitize(sanitizedStreetName, sanitizedSearch);
            }

            return (sanitizedStreetName, sanitizedSearch);
        }

        protected abstract string InnerSanitize(string streetName);
    }
}
