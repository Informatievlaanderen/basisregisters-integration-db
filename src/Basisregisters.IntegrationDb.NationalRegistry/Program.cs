// See https://aka.ms/new-console-template for more information

namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AddressMatching;
    using Destructurama;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Model;
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
                var directory = configuration["OutputDirectory"];
                var sourceFileName = configuration["SourceFileName"];

                var flatFileRecords = ReadFlatFileRecordsRecords(sourceFileName);

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

                var streetNameRepository = new StreetNameRepository(configuration.GetConnectionString("Integration"));
                var addressRepository = new AddressRepository(configuration.GetConnectionString("Integration"));

                var streetNameMatchRunner = new StreetNameMatchRunner(streetNameRepository);
                var (matchedStreetNames, unmatchedStreetNames) =
                    streetNameMatchRunner.Match(validRecords);

                FilesOutput.WriteUnmatchedRecords(unmatchedStreetNames, directory);
                Console.WriteLine("Street name matching DONE");
                Console.WriteLine($"Unmatched records to street names: {unmatchedStreetNames.Count}");

                var addressMatchRunner = new AddressMatchRunner(addressRepository, streetNameRepository);
                var addressMatchResult = addressMatchRunner.Match(matchedStreetNames.ToList());

                FilesOutput.WriteMatchedRecords(
                    addressMatchResult.AddressesWithRegisteredCount,
                    directory);
                FilesOutput.WriteUnmatchedRecords(
                    addressMatchResult.UnmatchedRecords.Select(x => x.Record),
                    directory);
                FilesOutput.WriteRecordsWithMultipleAddresses(
                    addressMatchResult.RecordsMatchedWithMultipleAddresses,
                    directory);
                FilesOutput.WriteAddressesWithMultipleRecords(
                    addressMatchResult.AddressesMatchedWithMultipleRecords,
                    directory);

                Console.WriteLine("Address matching DONE");
                Console.WriteLine($"Matched records: {addressMatchResult.AddressesWithRegisteredCount.Count(x => x.FlatFileRecord is not null)}");
                Console.WriteLine($"Unmatched records: {addressMatchResult.UnmatchedRecords.Count}");
                Console.WriteLine($"Record with multiple matched addresses: {addressMatchResult.RecordsMatchedWithMultipleAddresses.Count}");
                Console.WriteLine($"Addresses matched to multiple records: {addressMatchResult.AddressesMatchedWithMultipleRecords.Count}");

                var result = addressMatchResult.AddressesWithRegisteredCount
                    .OrderBy(x => x.Address.AddressPersistentLocalId)
                    .ToList();
                FilesOutput.WriteDbfFile(result, directory);
                FilesOutput.WriteShapeFile(result, directory);
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

        private static IEnumerable<FlatFileRecord> ReadFlatFileRecordsRecords(string path)
        {
            using TextReader textReader = File.OpenText(path);
            var mapper = FlatFileRecord.Mapper;
            var records = mapper.Read(textReader).ToList();

            return records;
        }
    }
}
