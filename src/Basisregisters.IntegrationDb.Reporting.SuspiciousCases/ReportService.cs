namespace Basisregisters.IntegrationDb.Reporting.SuspiciousCases;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Be.Vlaanderen.Basisregisters.GrAr.Notifications;
using CsvHelper;
using CsvHelper.Configuration;
using Infrastructure;
using Infrastructure.Options;
using IntegrationDb.SuspiciousCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class ReportService : BackgroundService
{
    private readonly SuspiciousCaseReportingContext _reportingContext;
    private readonly ISuspiciousCasesRepository _suspiciousCasesRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly INotificationService _notificationService;
    private readonly BlobServiceClient _azureBlobServiceClient;
    private readonly AzureBlobOptions _azureBlobOptions;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        SuspiciousCaseReportingContext reportingContext,
        ISuspiciousCasesRepository suspiciousCasesRepository,
        IMunicipalityRepository municipalityRepository,
        IHostApplicationLifetime applicationLifetime,
        INotificationService notificationService,
        BlobServiceClient azureBlobServiceClient,
        IOptions<AzureBlobOptions> azureBlobOptions,
        ILoggerFactory loggerFactory)
    {
        _reportingContext = reportingContext;
        _suspiciousCasesRepository = suspiciousCasesRepository;
        _municipalityRepository = municipalityRepository;
        _applicationLifetime = applicationLifetime;
        _notificationService = notificationService;
        _azureBlobServiceClient = azureBlobServiceClient;
        _azureBlobOptions = azureBlobOptions.Value;
        _logger = loggerFactory.CreateLogger<ReportService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting report service...");

        try
        {
            await UpdateSuspiciousCasesReport(stoppingToken);

            // make reports
            using var allCasesReportStream = await GenerateSuspiciousCasesReport(stoppingToken);
            using var monthlyReportStream = await GenerateMonthlyReport(stoppingToken);

            // upload to azure
            await UploadCsvToAzure("verdachte gevallen.csv", allCasesReportStream, stoppingToken);
            await UploadCsvToAzure("Monthly Snapshot.csv", monthlyReportStream, stoppingToken);

            await _notificationService.PublishToTopicAsync(
                new NotificationMessage(
                    "Basisregisters.IntegrationDb.Reporting.SuspiciousCases",
                    "New suspicious cases report has been generated successfully.",
                    "SuspiciousCases Report",
                    NotificationSeverity.Good));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while processing suspicious cases.");
            await _notificationService.PublishToTopicAsync(
                new NotificationMessage(
                    "Basisregisters.IntegrationDb.Reporting.SuspiciousCases",
                    "An error occurred while processing suspicious cases.",
                    "SuspiciousCases Report",
                    NotificationSeverity.Danger));
        }
        finally
        {
            _logger.LogInformation("Stopping report service...");
            _applicationLifetime.StopApplication();
        }
    }

    private async Task UploadCsvToAzure(string csvFileName, MemoryStream fileStream, CancellationToken cancellationToken)
    {
        var blobContainerClient = _azureBlobServiceClient.GetBlobContainerClient(_azureBlobOptions.ContainerName);

        var blobClient = blobContainerClient.GetBlockBlobClient(csvFileName);
        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "text/csv"
                }
            }, cancellationToken);
    }

    private async Task<MemoryStream> GenerateSuspiciousCasesReport(CancellationToken stoppingToken)
    {
        var municipalities = _municipalityRepository.GetAll()
            .ToDictionary(x => x.NisCode, x => x.DutchName);

        var cfgOut = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = System.Text.Encoding.UTF8
        };

        var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, leaveOpen: true);
        await using var csvWriter = new CsvWriter(writer, cfgOut);

        // header
        csvWriter.WriteField("nis_code");
        csvWriter.WriteField("object_id");
        csvWriter.WriteField("object_type");
        csvWriter.WriteField("vg_type_id");
        csvWriter.WriteField("vg_type");
        csvWriter.WriteField("Gemeentenaam");
        csvWriter.WriteField("Datum toegevoegd");
        csvWriter.WriteField("Datum gesloten");
        csvWriter.WriteField("Status");
        await csvWriter.NextRecordAsync();

        const int take = 10_000;
        var offset = 0;

        List<SuspiciousCase> GetSuspiciousCases()
        {
            return _reportingContext.SuspiciousCases
                .OrderBy(x => x.NisCode)
                .ThenBy(x => x.SuspiciousCaseType)
                .ThenBy(x => x.DateAdded)
                .ThenBy(x => x.DateClosed)
                .Skip(offset)
                .Take(take)
                .ToList();
        }

        var suspiciousCases = GetSuspiciousCases();
        while (suspiciousCases.Any() && !stoppingToken.IsCancellationRequested)
        {
            foreach (var report in suspiciousCases)
            {
                csvWriter.WriteField(report.NisCode);
                csvWriter.WriteField(report.ObjectId);
                csvWriter.WriteField(report.ObjectType.ToDutchString());
                csvWriter.WriteField((int)report.SuspiciousCaseType);
                csvWriter.WriteField(GetDutchDescription(report.SuspiciousCaseType));
                csvWriter.WriteField(municipalities[report.NisCode]);
                csvWriter.WriteField(report.DateAdded.ToString("dd/MM/yyyy"));
                csvWriter.WriteField(report.DateClosed?.ToString("dd/MM/yyyy") ?? string.Empty);
                switch (report.Status)
                {
                    case SuspiciousCaseStatus.Open:
                        csvWriter.WriteField("open");
                        break;
                    case SuspiciousCaseStatus.Solved:
                        csvWriter.WriteField("opgelost");
                        break;
                    case SuspiciousCaseStatus.Checked:
                        csvWriter.WriteField("afgevinkt");
                        break;
                    case SuspiciousCaseStatus.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await csvWriter.NextRecordAsync();
            }

            offset += take;

            await csvWriter.FlushAsync();
            await writer.FlushAsync(stoppingToken);

            suspiciousCases = GetSuspiciousCases();
        }

        await csvWriter.FlushAsync();
        await writer.FlushAsync(stoppingToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    private static string GetDutchDescription(SuspiciousCasesType type)
    {
        if (Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.SuspiciousCase.AllCases.TryGetValue(type, out var suspiciousCase))
        {
            return suspiciousCase.Description;
        }

        throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }

    private async Task<MemoryStream> GenerateMonthlyReport(CancellationToken stoppingToken)
    {
        var municipalities = _municipalityRepository.GetAll()
            .ToDictionary(x => x.NisCode, x => x.DutchName);

        var monthlyReports = _reportingContext.SuspiciousCaseReports
            .AsNoTracking()
            .ToList()
            .Union(
                _reportingContext
                    .SuspiciousCaseReports
                    .Local, new SuspiciousCaseReportEqualityComparer())
            .ToList()
            .Where(x => x.Month <= DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)) //exclude next month(s), except if it's the last day of the month
            .OrderBy(x => x.NisCode)
            .ThenBy(x => x.SuspiciousCaseType)
            .ThenBy(x => x.Month);

        var cfgOut = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = System.Text.Encoding.UTF8
        };

        var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, leaveOpen: true);
        await using var csvWriter = new CsvWriter(writer, cfgOut);

        // header
        csvWriter.WriteField("Gemeente");
        csvWriter.WriteField("VG_type_id");
        csvWriter.WriteField("VG_type");
        csvWriter.WriteField("Date_BoM");
        csvWriter.WriteField("Jaar-Maand");
        csvWriter.WriteField("Open_BoM");
        csvWriter.WriteField("Resolved");
        await csvWriter.NextRecordAsync();

        foreach (var report in monthlyReports)
        {
            if (report is { OpenCases: 0, ClosedCases: 0 }) continue;

            csvWriter.WriteField(municipalities[report.NisCode]);
            csvWriter.WriteField((int)report.SuspiciousCaseType);
            csvWriter.WriteField(GetDutchDescription(report.SuspiciousCaseType));
            csvWriter.WriteField(report.Month.ToString("dd/MM/yyyy"));
            csvWriter.WriteField(report.Month.ToString("yyyy-MM"));
            csvWriter.WriteField(report.OpenCases);
            csvWriter.WriteField(report.ClosedCases);
            await csvWriter.NextRecordAsync();
        }

        await csvWriter.FlushAsync();
        await writer.FlushAsync(stoppingToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    private async Task UpdateSuspiciousCasesReport(CancellationToken stoppingToken)
    {
        await _reportingContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, stoppingToken);

        var allSuspiciousCases = (await _suspiciousCasesRepository.GetSuspiciousCasesAsync())
            .ToDictionary(x => new { x.NisCode, x.ObjectId, x.SuspiciousCaseType });

        var reportedCases = (await _reportingContext.SuspiciousCases.ToListAsync(stoppingToken))
            .ToDictionary(x => new { x.NisCode, x.ObjectId, x.SuspiciousCaseType });

        // find cases that need to be reported
        var casesToReport = allSuspiciousCases
            .Where(x => !reportedCases.ContainsKey(x.Key))
            .Select(x => new SuspiciousCase(
                x.Value.NisCode,
                x.Value.ObjectId,
                x.Value.ObjectType,
                x.Value.SuspiciousCaseType,
                DateOnly.FromDateTime(DateTime.UtcNow)))
            .ToList();

        // close cases (solved or checked off) = they are in reported, but not in all cases
        reportedCases
            .Where(x => !allSuspiciousCases.ContainsKey(x.Key) && x.Value.DateClosed is null)
            .ToList()
            .ForEach(x => x.Value.Solve(DateOnly.FromDateTime(DateTime.UtcNow)));

        // reopen cases = they are in all cases and in reported, but reported is solved
        reportedCases
            .Where(x => allSuspiciousCases.ContainsKey(x.Key) && x.Value.Status == SuspiciousCaseStatus.Solved)
            .ToList()
            .ForEach(x => x.Value.Reopen());

        await _reportingContext.SuspiciousCases.AddRangeAsync(casesToReport, stoppingToken);
        await _reportingContext.SaveChangesAsync(stoppingToken);

        await UpdateMonthlyReport(stoppingToken);

        await _reportingContext.Database.CommitTransactionAsync(stoppingToken);
    }

    private async Task UpdateMonthlyReport(CancellationToken stoppingToken)
    {
        var startMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var startMonthOpenCases = startMonth.AddMonths(1);

        var monthlyReports = await _reportingContext.SuspiciousCaseReports
            .Where(x => x.Month == startMonth || x.Month == startMonthOpenCases)
            .ToListAsync(stoppingToken);

        // get all suspicious cases still open before the start of the next month, and count them by niscode, type
        var openCases = await _reportingContext
            .SuspiciousCases
            .AsNoTracking()
            .Where(x => x.DateAdded < startMonthOpenCases && x.DateClosed == null)
            .GroupBy(x => new { x.NisCode, x.SuspiciousCaseType })
            .Select(x => new { x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken: stoppingToken);

        // get all suspicious cases closed in this month, and count them by niscode, type
        var closedCases = await _reportingContext
            .SuspiciousCases
            .AsNoTracking()
            .Where(x => x.DateClosed != null && x.DateClosed >= startMonth && x.DateClosed <= DateOnly.FromDateTime(DateTime.Today))
            .GroupBy(x => new { x.NisCode, x.SuspiciousCaseType })
            .Select(x => new { x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken: stoppingToken);

        foreach (var openCase in openCases)
        {
            var report = monthlyReports
                .Where(x => x.Month == startMonthOpenCases)
                .FirstOrDefault(x => x.NisCode == openCase.Key.NisCode && x.SuspiciousCaseType == openCase.Key.SuspiciousCaseType);

            if (report is null)
            {
                report = new SuspiciousCaseReport(openCase.Key.NisCode, openCase.Key.SuspiciousCaseType, startMonthOpenCases);
                monthlyReports.Add(report);
                _reportingContext.SuspiciousCaseReports.Add(report);
            }

            report.OpenCases = openCase.Count;
        }

        foreach (var closedCase in closedCases)
        {
            var report = monthlyReports
                .Where(x => x.Month == startMonth)
                .FirstOrDefault(x => x.NisCode == closedCase.Key.NisCode && x.SuspiciousCaseType == closedCase.Key.SuspiciousCaseType);

            if (report is null)
            {
                report = new SuspiciousCaseReport(closedCase.Key.NisCode, closedCase.Key.SuspiciousCaseType, startMonth);
                monthlyReports.Add(report);
                _reportingContext.SuspiciousCaseReports.Add(report);
            }

            report.ClosedCases = closedCase.Count;
        }

        await _reportingContext.SaveChangesAsync(stoppingToken);
    }
}
