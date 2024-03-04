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

                var flatFileRecords = ReadFlatFileRecordsRecords(Path.Combine(directory, sourceFileName));

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

                FilesOutput.WriteInvalidRecords(invalidRecords, directory);

                Console.WriteLine("Record validation DONE");

                var streetNameMatchRunner = new StreetNameMatchRunner(configuration.GetConnectionString("Integration"));
                var (matchedStreetNames, unmatchedStreetNames) =
                    streetNameMatchRunner.Match(validRecords);

                FilesOutput.WriteUnmatchedRecords(unmatchedStreetNames, directory);
                Console.WriteLine("Street name matching DONE");
                Console.WriteLine($"Unmatched street names: {unmatchedStreetNames.Count()}");

                var addressMatchRunner = new AddressMatchRunner(configuration.GetConnectionString("Integration"));
                var addressMatchResult = addressMatchRunner.Match(matchedStreetNames.ToList());

                FilesOutput.WriteMatchedRecords(addressMatchResult.MatchedRecords, directory);

                FilesOutput.WriteUnmatchedRecords(addressMatchResult.UnmatchedRecords.Select(x => x.Record), directory);

                FilesOutput.WriteRecordsWithMultipleAddresses(addressMatchResult.RecordsMatchedWithMultipleAddresses, directory);
                FilesOutput.WriteAddressesWithMultipleRecords(addressMatchResult.AddressesMatchedWithMultipleRecords, directory);

                Console.WriteLine("Address matching DONE");
                Console.WriteLine($"Matched addresses: {addressMatchResult.MatchedRecords.Count()}");
                Console.WriteLine($"Unmatched addresses: {addressMatchResult.UnmatchedRecords.Count()}");

                await FilesOutput.WriteDbfFile(addressMatchResult.MatchedRecords, directory);
                FilesOutput.WriteShapeFile(addressMatchResult.MatchedRecords, directory);
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

        private static async Task CreateZipFile(ExtractFile file)
        {
            using var archiveStream = new MemoryStream();
            using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, leaveOpen: true);
            await using var entryStream = archive.CreateEntry($"xxx.dbf").Open();
            file.WriteTo(entryStream, CancellationToken.None);
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
