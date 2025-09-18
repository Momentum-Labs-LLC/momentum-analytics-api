using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core;
using Momentum.Analytics.DynamoDb.Client;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb;
using Momentum.Analytics.DynamoDb.Models;
using Amazon.DynamoDBv2;

namespace Momentum.Analytics.Export.Lambda;

public class Function
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ILogger _logger;

    public Function()
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        var services = new ServiceCollection();
        services
            .AddLogging(config => 
                {
                    config.AddFilter("Microsoft", LogLevel.Warning);
                    config.AddFilter("System", LogLevel.Warning);
                    config.SetMinimumLevel(LogLevel.Debug);
                    config.AddLambdaLogger();
                })
            .AddMemoryCache()
            .AddSingleton<IConfiguration>(configuration)
            .AddNodaTime()
            .AddSingleton<ITimeRangeProvider, TimeRangeProvider>()
            .AddDynamoDbClientFactory()
            .AddTransient<IIncrementalExporter, IncrementalExporter>()
            ;

        _serviceProvider = services.BuildServiceProvider();    
        _logger = _serviceProvider.GetRequiredService<ILogger<Function>>();
    } // end method

    public async Task FunctionHandlerAsync(Stream input)
    {
        var timeRangeProvider = _serviceProvider.GetRequiredService<ITimeRangeProvider>();
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        var incrementalExporter = _serviceProvider.GetRequiredService<IIncrementalExporter>();

        var timeRange = timeRangeProvider.TimeRange;
        var dateValue = timeRangeProvider.TimeRange.UtcStart.ToDateTimeOffset().ToString("yyyy/MM/dd");
        var s3ExportBucket = configuration.GetValue<string>("EXPORT_BUCKET", "momentum-prd-exports");

        var pageViewExportRequest = new IncrementalExportRequest(
            TableArn: configuration.GetValue<string>("PAGE_VIEWS_TABLE_ARN"),
            S3Bucket: s3ExportBucket,
            S3Prefix: $"{dateValue}/pageviews/",
            FromTime: timeRange.UtcStart.ToDateTimeUtc(),
            ToTime: timeRange.UtcEnd.ToDateTimeUtc(),
            ExportFormat: ExportFormat.DYNAMODB_JSON,
            ExportType: ExportType.INCREMENTAL_EXPORT);

        var pageViewExportSuccess = await TryExportAsync(incrementalExporter, pageViewExportRequest).ConfigureAwait(false);
        if(pageViewExportSuccess)
        {
            _logger.LogInformation("Exported page views.");
        }

        var collectedPiiExportRequest = new IncrementalExportRequest(
            TableArn: configuration.GetValue<string>("COLLECTED_PII_TABLE_ARN"),
            S3Bucket: s3ExportBucket,
            S3Prefix: $"{dateValue}/collected_pii/",
            FromTime: timeRange.UtcStart.ToDateTimeUtc(),
            ToTime: timeRange.UtcEnd.ToDateTimeUtc(),
            ExportFormat: ExportFormat.DYNAMODB_JSON,
            ExportType: ExportType.INCREMENTAL_EXPORT);

        await incrementalExporter.ExportAsync(collectedPiiExportRequest).ConfigureAwait(false);
        var collectedPiiExportSuccess = await TryExportAsync(incrementalExporter, collectedPiiExportRequest).ConfigureAwait(false);
        if(collectedPiiExportSuccess)
        {
            _logger.LogInformation("Exported collected pii.");
        }

        var piiValueExportRequest = new IncrementalExportRequest(
            TableArn: configuration.GetValue<string>("PII_VALUES_TABLE_ARN"),
            S3Bucket: s3ExportBucket,
            S3Prefix: $"{dateValue}/pii_values/",
            FromTime: timeRange.UtcStart.ToDateTimeUtc(),
            ToTime: timeRange.UtcEnd.ToDateTimeUtc(),
            ExportFormat: ExportFormat.DYNAMODB_JSON,
            ExportType: ExportType.INCREMENTAL_EXPORT);

        var piiValueExportSuccess = await TryExportAsync(incrementalExporter, piiValueExportRequest).ConfigureAwait(false);
        if(piiValueExportSuccess)
        {
            _logger.LogInformation("Exported pii values.");
        }
    } // end method

    protected async Task<bool> TryExportAsync(IIncrementalExporter incrementalExporter, IncrementalExportRequest exportRequest, CancellationToken token = default)
    {
        var success = false;
        try
        {
            await incrementalExporter.ExportAsync(exportRequest, token).ConfigureAwait(false);
            success = true;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to export {0}.", exportRequest.TableArn);
        }

        return success;
    } // end method
} // end class