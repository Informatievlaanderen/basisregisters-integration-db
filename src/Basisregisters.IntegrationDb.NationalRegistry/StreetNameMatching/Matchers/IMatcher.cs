namespace Basisregisters.IntegrationDb.NationalRegistry.StreetNameMatching.Matchers
{
    public interface IMatcher
    {
        bool Match(string? streetName, string search);
    }
}
