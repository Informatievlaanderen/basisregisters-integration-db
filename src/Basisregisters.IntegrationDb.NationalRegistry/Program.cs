// See https://aka.ms/new-console-template for more information

namespace Basisregisters.IntegrationDb.NationalRegistry
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            logger.LogInformation("Starting IntegrationDb.NationalRegistry");

            try
            {
                var sourceFilePath = configuration["sourceFilePath"];
                var flatFileRecords = ReadFlatFileRecordsRecords(sourceFilePath);

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
