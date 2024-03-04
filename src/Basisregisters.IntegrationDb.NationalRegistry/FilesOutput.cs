namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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

        public static void WriteMatchedRecords(IEnumerable<AddressWithFlatFileRecord> matchedRecords, string directory)
        {
            File.AppendAllLines(
                Path.Combine(directory, MatchedRecordsFileName),
                matchedRecords.Select(x => $"{x.FlatFileRecord.Record.ToSafeString()};{x.HouseNumberBoxNumberType}"));
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

        public static async Task WriteDbfFile(IEnumerable<AddressWithFlatFileRecord> matchedRecords, string directory)
        {
            var dbfFile = await CreateResultFile(matchedRecords, CancellationToken.None);
            await using var fileStream = File.Create(Path.Combine(directory, dbfFile.Name));
            dbfFile.WriteTo(fileStream, CancellationToken.None);
        }

        public static void WriteShapeFile(IEnumerable<AddressWithFlatFileRecord> matchedRecords, string directory)
        {
            var files = CreateShapeFiles(matchedRecords.ToList());
            foreach (var extractFile in files)
            {
                using var fileStream = File.Create(Path.Combine(directory, extractFile.Name));
                extractFile.WriteTo(fileStream, CancellationToken.None);
            }
        }

        private static IEnumerable<ExtractFile> CreateShapeFiles(List<AddressWithFlatFileRecord> matchedRecords)
        {
            var content = matchedRecords
                .Select(x => new PointShapeContent(new Point(x.Address.Position.X, x.Address.Position.Y))
                    .ToBytes())
                .ToList();

            var boundingBox = new BoundingBox3D(
                matchedRecords.Min(x => x.Address.Position.X),
                matchedRecords.Min(x => x.Address.Position.Y),
                matchedRecords.Max(x => x.Address.Position.X),
                matchedRecords.Max(x => x.Address.Position.Y),
                0,
                0,
                double.NegativeInfinity,
                double.PositiveInfinity);

            yield return ExtractBuilder.CreateShapeFile<PointShapeContent>(
                "CLI",
                ShapeType.Point,
                content.Select(x => x),
                ShapeContent.Read,
                content.Select(x => x.Length),
                boundingBox);

            yield return ExtractBuilder.CreateShapeIndexFile(
                "CLI",
                ShapeType.Point,
                content.Select(x => x.Length),
                () => content.Count,
                boundingBox);

            yield return ExtractBuilder.CreateProjectedCoordinateSystemFile(
                "CLI",
                ProjectedCoordinateSystem.Belge_Lambert_1972);
        }

        private static async Task<ExtractFile> CreateResultFile(IEnumerable<AddressWithFlatFileRecord> matchedRecords, CancellationToken ct)
        {
            byte[] TransformRecord(AddressWithFlatFileRecord flatFileRecordWithAddress)
            {
                var streetName = flatFileRecordWithAddress.FlatFileRecord.StreetNames
                    .Single(x => x.StreetNamePersistentLocalId == flatFileRecordWithAddress.Address.StreetNamePersistentLocalId);

                var item = new AddressMatchDatabaseRecord
                {
                    ID = { Value = $"https://data.vlaanderen.be/id/adres/{flatFileRecordWithAddress.Address.AddressPersistentLocalId}" },
                    StraatnaamID = { Value = $"https://data.vlaanderen.be/id/straatnaam/{flatFileRecordWithAddress.Address.StreetNamePersistentLocalId}" },
                    StraatNM = { Value = streetName.NameDutch },
                    HuisNR = { Value = flatFileRecordWithAddress.Address.HouseNumber },
                    BusNR = { Value = flatFileRecordWithAddress.Address.BoxNumber },
                    NisGemCode = { Value = flatFileRecordWithAddress.FlatFileRecord.Record.NisCode },
                    GemNM = { Value = streetName.MunicipalityName },
                    PKanCode = { Value = flatFileRecordWithAddress.Address.PostalCode },
                    Herkomst = { Value = flatFileRecordWithAddress.Address.Specification },
                    Methode = { Value = flatFileRecordWithAddress.Address.Method },
                    Inwoners = { Value = flatFileRecordWithAddress.FlatFileRecord.Record.RegisteredCount },
                    HuisnrStat = { Value = flatFileRecordWithAddress.Address.Status },
                };

                return item.ToBytes(DbaseCodePage.Western_European_ANSI.ToEncoding());
            }

            return ExtractBuilder.CreateDbfFile<AddressWithFlatFileRecord, AddressMatchDatabaseRecord>(
                "CLI",
                new AddressMatchDbaseSchema(),
                matchedRecords,
                matchedRecords.Count,
                TransformRecord);
        }
    }
}
