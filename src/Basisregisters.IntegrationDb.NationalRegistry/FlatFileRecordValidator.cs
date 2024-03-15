namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Repositories;

    public sealed class FlatFileRecordValidator
    {
        private readonly IEnumerable<string> _postalCodes;

        public FlatFileRecordValidator(IPostalCodeRepository postalCodeRepository)
        {
            ArgumentNullException.ThrowIfNull(postalCodeRepository);

            _postalCodes = postalCodeRepository.GetAllPostalCodes();
        }

        public FlatFileRecordErrorType? Validate(FlatFileRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.PostalCode))
            {
                return FlatFileRecordErrorType.MissingPostalCode;
            }

            if (record.PostalCode.Length != 4 || !int.TryParse(record.PostalCode, out _))
            {
                return FlatFileRecordErrorType.InvalidPostalCode;
            }

            if (!_postalCodes.Contains(record.PostalCode))
            {
                return FlatFileRecordErrorType.PostalCodeNotFound;
            }

            if (string.IsNullOrWhiteSpace(record.StreetCode))
            {
                return FlatFileRecordErrorType.MissingStreetCode;
            }

            if (record.StreetCode.Length != 4 || !int.TryParse(record.StreetCode, out _))
            {
                return FlatFileRecordErrorType.InvalidStreetCode;
            }

            return null;
        }
    }
}
