namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Be.Vlaanderen.Basisregisters.Api.Extract;
    using Be.Vlaanderen.Basisregisters.GrAr.Extracts;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Model;
    using Model.Extract;
    using Repositories;

    public class FilesOutput
    {
        private const string InvalidRecordsFileName = "invalidRecords.csv";
        private const string UnmatchedRecordsFileName = "unmatchedRecords.csv";
        private const string MatchedRecordsFileName = "matchedRecords.csv";
        private const string MultipleAddressesPerRecordFileName = "multipleAddressesPerRecord.csv";
        private const string MultipleRecordsPerAddressFileName = "multipleRecordsPerAddress.csv";

        public static void WriteInvalidRecords(IEnumerable<(FlatFileRecord Record, FlatFileRecordErrorType Error)> invalidRecords, string directory)
        {
            File.WriteAllLines(Path.Combine(directory, InvalidRecordsFileName), invalidRecords.Select(x => $"{x.Record.ToSafeString()};{x.Error.ToString()}"));
        }

        public static void WriteUnmatchedRecords(IEnumerable<FlatFileRecord> unmatchedRecords, string directory)
        {
            File.AppendAllLines(
                Path.Combine(directory, UnmatchedRecordsFileName),
                unmatchedRecords.Select(x => $"{x.ToSafeString()}"));
        }

        public static void WriteMatchedRecords(IEnumerable<AddressWithRegisteredCount> addressesWithRegisteredCount, string directory)
        {
            File.AppendAllLines(
                Path.Combine(directory, MatchedRecordsFileName),
                addressesWithRegisteredCount
                    .Where(x => x.FlatFileRecord is not null)
                    .Select(x => $"{x.FlatFileRecord!.ToSafeString()};{x.Address.AddressPersistentLocalId};{x.HouseNumberBoxNumberTypes}"));
        }

        public static void WriteRecordsWithMultipleAddresses(IDictionary<FlatFileRecordWithStreetNames, List<Address>> records, string directory)
        {
            try
            {
                File.AppendAllLines(
                    Path.Combine(directory, MultipleAddressesPerRecordFileName),
                    records.Select(x =>
                        $"{x.Key.Record.ToSafeString()};{x.Value.Select(y => y.AddressPersistentLocalId.ToString()).Aggregate((i, j) => $"{i};{j}")}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void WriteAddressesWithMultipleRecords(IDictionary<Address, List<FlatFileRecordWithStreetNames>> addresses, string directory)
        {
            try
            {
                File.AppendAllLines(
                    Path.Combine(directory, MultipleRecordsPerAddressFileName),
                    addresses.Select(x =>
                        $"{x.Key.AddressPersistentLocalId};{x.Value.Select(y => y.Record.ToSafeString()).Aggregate((i, j) => $"{i};{j}")}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void WriteDbfFile(IList<AddressWithRegisteredCount> addressesWithRegisteredCount, string directory)
        {
            var dbfFile = CreateResultFile(addressesWithRegisteredCount);
            using var fileStream = File.Create(Path.Combine(directory, dbfFile.Name));
            dbfFile.WriteTo(fileStream, CancellationToken.None);
        }

        public static void WriteShapeFile(IList<AddressWithRegisteredCount> addressesWithRegisteredCount, string directory)
        {
            var files = CreateShapeFiles(addressesWithRegisteredCount);
            foreach (var extractFile in files)
            {
                using var fileStream = File.Create(Path.Combine(directory, extractFile.Name));
                extractFile.WriteTo(fileStream, CancellationToken.None);
            }
        }

        private static IEnumerable<ExtractFile> CreateShapeFiles(IList<AddressWithRegisteredCount> matchedRecords)
        {
            var content = matchedRecords
                .Select(x =>
                {
                    var point = x.Address.Position;
                    var pointShapeContent = new PointShapeContent(new Point(point.X, point.Y));
                    return new
                    {
                        Point = point,
                        Content = pointShapeContent.ToBytes(),
                        ContentLength = pointShapeContent.ContentLength.ToInt32()
                    };
                })
                .ToList();

            var boundingBox = new BoundingBox3D(
                content.Min(x => x.Point.X),
                content.Min(x => x.Point.Y),
                content.Max(x => x.Point.X),
                content.Max(x => x.Point.Y),
                0,
                0,
                double.NegativeInfinity,
                double.PositiveInfinity);

            yield return ExtractBuilder.CreateShapeFile<PointShapeContent>(
                "CLI",
                ShapeType.Point,
                content.Select(x => x.Content),
                ShapeContent.Read,
                content.Select(x => x.ContentLength),
                boundingBox);

            yield return ExtractBuilder.CreateShapeIndexFile(
                "CLI",
                ShapeType.Point,
                content.Select(x => x.ContentLength),
                () => content.Count,
                boundingBox);

            yield return ExtractBuilder.CreateProjectedCoordinateSystemFile(
                "CLI",
                ProjectedCoordinateSystem.Belge_Lambert_1972);
        }

        private static ExtractFile CreateResultFile(
            IList<AddressWithRegisteredCount> addressesWithRegisteredCount)
        {
            byte[] TransformRecord(AddressWithRegisteredCount addressWithRegisteredCount)
            {
                var item = new AddressMatchDatabaseRecord
                {
                    ID = { Value = $"https://data.vlaanderen.be/id/adres/{addressWithRegisteredCount.Address.AddressPersistentLocalId}" },
                    StraatnaamID = { Value = $"https://data.vlaanderen.be/id/straatnaam/{addressWithRegisteredCount.Address.StreetNamePersistentLocalId}" },
                    StraatNM = { Value = addressWithRegisteredCount.StreetName.NameDutch! },
                    HuisNR = { Value = addressWithRegisteredCount.Address.HouseNumber },
                    BusNR = { Value = addressWithRegisteredCount.Address.BoxNumber },
                    NisGemCode = { Value = addressWithRegisteredCount.StreetName.NisCode },
                    GemNM = { Value = addressWithRegisteredCount.StreetName.MunicipalityName },
                    PKanCode = { Value = addressWithRegisteredCount.Address.PostalCode },
                    Herkomst = { Value = addressWithRegisteredCount.Address.Specification },
                    Methode = { Value = addressWithRegisteredCount.Address.Method },
                    Inwoners = { Value = addressWithRegisteredCount.RegisteredCount ?? -99 },
                    HuisnrStat = { Value = addressWithRegisteredCount.Address.Status },
                };

                return item.ToBytes(DbaseCodePage.Western_European_ANSI.ToEncoding());
            }

            return ExtractBuilder.CreateDbfFile<AddressWithRegisteredCount, AddressMatchDatabaseRecord>(
                "CLI",
                new AddressMatchDbaseSchema(),
                addressesWithRegisteredCount,
                () => addressesWithRegisteredCount.Count,
                TransformRecord);
        }

        public static void WriteDbfFile(IList<UnmatchedFlatFileRecord> unmatchedRecords, string directory)
        {
            var dbfFile = CreateResultFile(unmatchedRecords);
            using var fileStream = File.Create(Path.Combine(directory, dbfFile.Name));
            dbfFile.WriteTo(fileStream, CancellationToken.None);
        }

        public static void WriteShapeFile(IList<UnmatchedFlatFileRecord> unmatchedRecords, string directory)
        {
            var files = CreateShapeFiles(unmatchedRecords);
            foreach (var extractFile in files)
            {
                using var fileStream = File.Create(Path.Combine(directory, extractFile.Name));
                extractFile.WriteTo(fileStream, CancellationToken.None);
            }
        }

        private static IEnumerable<ExtractFile> CreateShapeFiles(IList<UnmatchedFlatFileRecord> unmatchedRecords)
        {
            var content = unmatchedRecords
                .Select(x =>
                {
                    var point = x.Position;
                    var pointShapeContent = new PointShapeContent(new Point(point.X, point.Y));
                    return new
                    {
                        Point = point,
                        Content = pointShapeContent.ToBytes(),
                        ContentLength = pointShapeContent.ContentLength.ToInt32()
                    };
                })
                .ToList();

            var boundingBox = new BoundingBox3D(
                content.Min(x => x.Point.X),
                content.Min(x => x.Point.Y),
                content.Max(x => x.Point.X),
                content.Max(x => x.Point.Y),
                0,
                0,
                double.NegativeInfinity,
                double.PositiveInfinity);

            yield return ExtractBuilder.CreateShapeFile<PointShapeContent>(
                "unmatchedAddresses",
                ShapeType.Point,
                content.Select(x => x.Content),
                ShapeContent.Read,
                content.Select(x => x.ContentLength),
                boundingBox);

            yield return ExtractBuilder.CreateShapeIndexFile(
                "unmatchedAddresses",
                ShapeType.Point,
                content.Select(x => x.ContentLength),
                () => content.Count,
                boundingBox);

            yield return ExtractBuilder.CreateProjectedCoordinateSystemFile(
                "unmatchedAddresses",
                ProjectedCoordinateSystem.Belge_Lambert_1972);
        }

        private static ExtractFile CreateResultFile(
            IList<UnmatchedFlatFileRecord> unmatchedRecords)
        {
            string ToPrecisie(UnmatchedFlatFileRecord unmatchedRecord)
            {
                if (unmatchedRecord.HouseNumberAddress is not null)
                {
                    return "Huisnummer";
                }

                if (unmatchedRecord.StreetName is not null)
                {
                    return "Straat";
                }

                return "Gemeente";
            }

            byte[] TransformRecord(UnmatchedFlatFileRecord unmatchedRecord)
            {
                var item = new UnmatchedAddressDbaseRecord
                {
                    RRNiscode = { Value = unmatchedRecord.Record.NisCode },
                    RRPostcode = { Value = unmatchedRecord.Record.PostalCode },
                    RRStraatcode = { Value = unmatchedRecord.Record.StreetCode },
                    RRHuisnummer = { Value = unmatchedRecord.Record.HouseNumber },
                    RRIndex = { Value = unmatchedRecord.Record.Index },
                    RRStraatnaam = { Value = unmatchedRecord.Record.StreetName },
                    GRARAdresID = { Value = unmatchedRecord.HouseNumberAddress is not null
                        ? $"https://data.vlaanderen.be/id/adres/{unmatchedRecord.HouseNumberAddress.AddressPersistentLocalId}"
                        : null
                    },
                    GRARStraatnaamID = { Value = unmatchedRecord.StreetName is not null
                        ? $"https://data.vlaanderen.be/id/straatnaam/{unmatchedRecord.StreetName.StreetNamePersistentLocalId}"
                        : null
                    },
                    GRARNiscode = { Value = unmatchedRecord.Record.NisCode },
                    GRARPostcode = { Value = unmatchedRecord.HouseNumberAddress?.PostalCode },
                    GRARStraatnaam = { Value = unmatchedRecord.StreetName?.NameDutch },
                    GRARHuisnummer = { Value = unmatchedRecord.HouseNumberAddress?.HouseNumber },
                    Precisie = { Value = ToPrecisie(unmatchedRecord) },
                    Inwoners = { Value = unmatchedRecord.Record.RegisteredCount }
                };

                return item.ToBytes(DbaseCodePage.Western_European_ANSI.ToEncoding());
            }

            return ExtractBuilder.CreateDbfFile<UnmatchedFlatFileRecord, UnmatchedAddressDbaseRecord>(
                "unmatchedAddresses",
                new UnmatchedAddressDbaseSchema(),
                unmatchedRecords,
                () => unmatchedRecords.Count,
                TransformRecord);
        }
    }
}
