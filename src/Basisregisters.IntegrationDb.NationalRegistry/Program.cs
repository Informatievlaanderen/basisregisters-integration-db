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
    using FlatFiles;
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
                var path = configuration["filePath"];

                var flatFileRecords = ReadFlatFileRecordsRecords(path);

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

                WriteInvalids(invalidRecords, configuration["invalidRecordsPath"]);
                Console.WriteLine("Postal validation DONE");

                var matchStreetNameRunner = new StreetNameMatchRunner(configuration.GetConnectionString("Integration"));
                var (matchedStreetNames, unmatchedStreetNames) =
                    matchStreetNameRunner.Match(validRecords);

                WriteUnmatched(unmatchedStreetNames, configuration["unmatchedRecordsPath"]);
                Console.WriteLine("Matching StreetName DONE");

                var matchAddressRunner = new AddressMatchRunner(configuration.GetConnectionString("Integration"));
                var  (matchedAddresses, unmatchedAddresses) =
                    matchAddressRunner.Match(matchedStreetNames.ToList());

                WriteUnmatched(unmatchedAddresses.Select(x => x.Record), configuration["unmatchedRecordsPath"]);
                WriteMatched(matchedAddresses.Select(x => x.FlatFileRecordWithStreetNames.Record), configuration["matchedRecordsPath"]);

                // var unmatchedWithoutHouseNumberMatch = new List<FlatFileRecord>();
                // foreach (var unmatch in unmatchedAddresses)
                // {
                //     if (!matchedAddresses.Any(x =>
                //             x.FlatFileRecordWithStreetNames.Record.NisCode == unmatch.Record.NisCode
                //             && x.FlatFileRecordWithStreetNames.Record.PostalCode == unmatch.Record.PostalCode
                //             && x.FlatFileRecordWithStreetNames.Record.StreetName == unmatch.Record.StreetName
                //             && x.FlatFileRecordWithStreetNames.Record.HouseNumber == unmatch.Record.HouseNumber))
                //     {
                //         unmatchedWithoutHouseNumberMatch.Add(unmatch.Record);
                //     }
                // }
                //
                // WriteUnmatched(unmatchedWithoutHouseNumberMatch, configuration["unmatchedWithoutHouseNumberMatchRecordsPath"]);

                // var path = configuration["filePath"];
                //
                // var streetNames = ReadNisCodeStreetNameRecords(path);
                //
                // var matchStreetNameRunner = new StreetNameMatchRunner(configuration.GetConnectionString("Integration"));
                // matchStreetNameRunner.Match(streetNames);
                Console.WriteLine("DONE");
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

        private static void WriteUnmatched(IEnumerable<FlatFileRecord> unmatchedRecords, string path)
        {
            File.AppendAllLines(path, unmatchedRecords.Select(x => $"{x.ToSafeString()}"));
        }

        private static void WriteMatched(IEnumerable<FlatFileRecord> unmatchedRecords, string path)
        {
            File.AppendAllLines(path, unmatchedRecords.Select(x => $"{x.ToSafeString()}"));
        }

        private static void WriteInvalids(IEnumerable<(FlatFileRecord Record, FlatFileRecordErrorType Error)> invalidRecords, string path)
        {
            File.WriteAllLines(path, invalidRecords.Select(x => $"{x.Record.ToSafeString()};{x.Error.ToString()}"));
        }

        private static List<FlatFileRecord> ReadFlatFileRecordsRecords(string path)
        {
            using TextReader textReader = File.OpenText(path);
            var mapper = FlatFileRecord.Mapper;
            var records = mapper.Read(textReader).ToList();

            return records;
        }

        private static List<NisCodeStreetNameRecord> ReadNisCodeStreetNameRecords(string path)
        {
            using var reader = new StreamReader(path);
            var options = new DelimitedOptions
            {
                IsFirstRecordSchema = false,
                Separator = ";"
            };
            var nisCodeStreetNameRecords = NisCodeStreetNameRecord.Mapper.Read(reader, options).ToList();
            return nisCodeStreetNameRecords;
        }
    }
}
