// See https://aka.ms/new-console-template for more information

namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AddressMatching;
    using Be.Vlaanderen.Basisregisters.Api.Extract;
    using Be.Vlaanderen.Basisregisters.GrAr.Extracts;
    using Be.Vlaanderen.Basisregisters.Shaperon;
    using Destructurama;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Model;
    using Model.Extract;
    using Repositories;
    using Serilog;
    using Serilog.Debugging;
    using StreetNameMatching;

    public sealed class Program
    {
        protected Program()
        { }

        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
                Log.Debug(
                    eventArgs.Exception,
                    "FirstChanceException event raised in {AppDomain}.",
                    AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
                Log.Fatal((Exception)eventArgs.ExceptionObject, "Encountered a fatal exception, exiting program.");

            var host = new HostBuilder()
                .ConfigureAppConfiguration((_, builder) =>
                {
                    builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureLogging((hostContext, builder) =>
                {
                    SelfLog.Enable(Console.WriteLine);

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithMachineName()
                        .Enrich.WithThreadId()
                        .Enrich.WithEnvironmentUserName()
                        .Destructure.JsonNetTypes()
                        .CreateLogger();

                    builder.ClearProviders();
                    builder.AddSerilog(Log.Logger);
                })
                .UseConsoleLifetime()
                .Build();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            logger.LogInformation("Starting IntegrationDb.NationalRegistry");

            try
            {
                var directory = configuration["Directory"];
                var sourceFileName = configuration["SourceFileName"];

                var flatFileRecords = ReadFlatFileRecordsRecords(directory + sourceFileName);

                var validator = new FlatFileRecordValidator(new PostalCodeRepository(configuration.GetConnectionString("Integration")));
                var invalidRecords = new ConcurrentBag<(FlatFileRecord, FlatFileRecordErrorType)>();
                var validRecords = new ConcurrentBag<FlatFileRecord>();

                Parallel.ForEach(flatFileRecords, record =>
                {
                    var error = validator.Validate(record);
                    if (error.HasValue)
                    {
                        invalidRecords.Add(new(record, error.Value));
                    }
                    else
                    {
                        validRecords.Add(record);
                    }
                });

                WriteInvalidRecords(invalidRecords, configuration["invalidRecordsPath"]);
                Console.WriteLine("Record validation DONE");

                var streetNameMatchRunner = new StreetNameMatchRunner(configuration.GetConnectionString("Integration"));
                var (matchedStreetNames, unmatchedStreetNames) =
                    streetNameMatchRunner.Match(validRecords);

                WriteUnmatchedRecords(unmatchedStreetNames, configuration["unmatchedRecordsPath"]);
                Console.WriteLine("Street name matching DONE");
                Console.WriteLine($"Unmatched street names: {unmatchedStreetNames.Count()}");

                var addressMatchRunner = new AddressMatchRunner(configuration.GetConnectionString("Integration"));
                var addressMatchResult = addressMatchRunner.Match(matchedStreetNames.ToList());

                WriteMatchedRecords(addressMatchResult.MatchedRecords, configuration["matchedRecordsPath"]);
                WriteUnmatchedRecords(addressMatchResult.UnmatchedRecords.Select(x => x.Record), configuration["unmatchedRecordsPath"]);
                WriteRecordsWithMultipleAddresses(addressMatchResult.RecordsMatchedWithMultipleAddresses, configuration["recordsMatchedWithMultipleAddressesPath"]);
                WriteAddressesWithMultipleRecords(addressMatchResult.AddressesMatchedWithMultipleRecords, configuration["addressesMatchedWithMultipleRecordsPath"]);

                Console.WriteLine("Address matching DONE");
                Console.WriteLine($"Matched addresses (includes addresses with multiple records and records with multiple addresses): {addressMatchResult.MatchedRecords.Count()}");
                Console.WriteLine($"Unmatched addresses: {addressMatchResult.UnmatchedRecords.Count()}");


                // TODO: filter double records
                var dbfFile = await CreateResultFile(addressMatchResult.MatchedRecords, CancellationToken.None);
                var fileStream = File.Create(configuration["dbfFilePath"]);
                dbfFile.WriteTo(fileStream, CancellationToken.None);

                var files = CreateShapeFiles(addressMatchResult.MatchedRecords.ToList());
                foreach (var extractFile in files)
                {
                    fileStream = File.Create($"{configuration["shapeFilePath"]}{extractFile.Name}");
                    extractFile.WriteTo(fileStream, CancellationToken.None);
                }
            }
            catch (AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    logger.LogCritical(innerException, "Encountered a fatal exception, exiting program.");
                }
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Encountered a fatal exception, exiting program.");
                Log.CloseAndFlush();

                // Allow some time for flushing before shutdown.
                await Task.Delay(500, default);
                throw;
            }
            finally
            {
                logger.LogInformation("Stopping...");
            }
        }

        private static IEnumerable<ExtractFile> CreateShapeFiles(List<FlatFileRecordWithAddress> matchedRecords)
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

        private static async Task<ExtractFile> CreateResultFile(IEnumerable<FlatFileRecordWithAddress> matchedRecords, CancellationToken ct)
        {
            byte[] TransformRecord(FlatFileRecordWithAddress flatFileRecordWithAddress)
            {
                var streetName = flatFileRecordWithAddress.FlatFileRecordWithStreetNames.StreetNames
                    .Single(x => x.StreetNamePersistentLocalId == flatFileRecordWithAddress.Address.StreetNamePersistentLocalId);

                var item = new AddressMatchDatabaseRecord
                {
                    ID = { Value = $"https://data.vlaanderen.be/id/adres/{flatFileRecordWithAddress.Address.AddressPersistentLocalId}" },
                    StraatnaamID = { Value = $"https://data.vlaanderen.be/id/straatnaam/{flatFileRecordWithAddress.Address.StreetNamePersistentLocalId}" },
                    StraatNM = { Value = streetName.NameDutch },
                    HuisNR = { Value = flatFileRecordWithAddress.Address.HouseNumber },
                    BusNR = { Value = flatFileRecordWithAddress.Address.BoxNumber },
                    NisGemCode = { Value = flatFileRecordWithAddress.FlatFileRecordWithStreetNames.Record.NisCode },
                    GemNM = { Value = streetName.MunicipalityName },
                    PKanCode = { Value = flatFileRecordWithAddress.Address.PostalCode },
                    Herkomst = { Value = flatFileRecordWithAddress.Address.Specification },
                    Methode = { Value = flatFileRecordWithAddress.Address.Method },
                    Inwoners = { Value = flatFileRecordWithAddress.FlatFileRecordWithStreetNames.Record.RegisteredCount },
                    HuisnrStat = { Value = flatFileRecordWithAddress.Address.Status },
                };

                return item.ToBytes(DbaseCodePage.Western_European_ANSI.ToEncoding());
            }

            return ExtractBuilder.CreateDbfFile<FlatFileRecordWithAddress, AddressMatchDatabaseRecord>(
                "CLI",
                new AddressMatchDbaseSchema(),
                matchedRecords,
                matchedRecords.Count,
                TransformRecord);
        }

        private static async Task CreateZipFile(ExtractFile file)
        {
            using var archiveStream = new MemoryStream();
            using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, leaveOpen: true);
            await using var entryStream = archive.CreateEntry($"xxx.dbf").Open();
            file.WriteTo(entryStream, CancellationToken.None);
        }

        private static void WriteInvalidRecords(IEnumerable<(FlatFileRecord Record, FlatFileRecordErrorType Error)> invalidRecords, string path)
        {
            File.WriteAllLines(path, invalidRecords.Select(x => $"{x.Record.ToSafeString()};{x.Error.ToString()}"));
        }

        private static void WriteMatchedRecords(IEnumerable<FlatFileRecordWithAddress> matchedRecords, string path)
        {
            File.AppendAllLines(
                path,
                matchedRecords.Select(x => $"{x.FlatFileRecordWithStreetNames.Record.ToSafeString()};{x.HouseNumberBoxNumberType}"));
        }

        private static void WriteUnmatchedRecords(IEnumerable<FlatFileRecord> unmatchedRecords, string path)
        {
            File.AppendAllLines(
                path,
                unmatchedRecords.Select(x => $"{x.ToSafeString()}"));
        }

        private static void WriteRecordsWithMultipleAddresses(IDictionary<FlatFileRecordWithStreetNames, List<Address>> records, string path)
        {
            try
            {
                File.AppendAllLines(
                    path,
                    records.Select(x =>
                        $"{x.Key.Record.ToSafeString()};{x.Value.Select(y => y.AddressPersistentLocalId.ToString()).Aggregate((i, j) => $"{i};{j}")}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void WriteAddressesWithMultipleRecords(IDictionary<Address, List<FlatFileRecordWithStreetNames>> addresses, string path)
        {
            try
            {
                File.AppendAllLines(
                    path,
                    addresses.Select(x =>
                        $"{x.Key.AddressPersistentLocalId};{x.Value.Select(y => y.Record.ToSafeString()).Aggregate((i, j) => $"{i};{j}")}"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static IEnumerable<FlatFileRecord> ReadFlatFileRecordsRecords(string path)
        {
            using TextReader textReader = File.OpenText(path);
            var mapper = FlatFileRecord.Mapper;
            var records = mapper.Read(textReader).ToList();

            return records;
        }
    }
}
