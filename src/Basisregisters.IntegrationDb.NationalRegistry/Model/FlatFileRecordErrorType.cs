namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    public enum FlatFileRecordErrorType
    {
        Unknown,
        MissingPostalCode,
        InvalidPostalCode,
        PostalCodeNotFound,
        MissingStreetCode,
        InvalidStreetCode,
        StreetCodeNotFound,
    }
}
