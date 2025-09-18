using Moq;
using Momentum.Analytics.DynamoDb.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Models;
using NodaTime;
using Momentum.Analytics.DynamoDb.Models;
using Amazon.DynamoDBv2;

namespace Momentum.Analytics.Export.Lambda.Tests;

public class FunctionTests
{
    private Mock<IIncrementalExporter> _incrementalExporter;
    private Mock<ITimeRangeProvider> _timeRangeProvider;
    private Mock<IConfiguration> _configuration;
    private Mock<ILogger<Function>> _logger;
    private Mock<IServiceProvider> _serviceProvider;
    private Function _function;

    public FunctionTests()
    {
        _incrementalExporter = new Mock<IIncrementalExporter>();
        _timeRangeProvider = new Mock<ITimeRangeProvider>();
        _configuration = new Mock<IConfiguration>();
        _logger = new Mock<ILogger<Function>>();

        _serviceProvider = new Mock<IServiceProvider>();
        _serviceProvider.Setup(x => x.GetService(typeof(IIncrementalExporter)))
            .Returns(_incrementalExporter.Object);
        _serviceProvider.Setup(x => x.GetService(typeof(ITimeRangeProvider)))
            .Returns(_timeRangeProvider.Object);
        _serviceProvider.Setup(x => x.GetService(typeof(IConfiguration)))
            .Returns(_configuration.Object);
        _serviceProvider.Setup(x => x.GetService(typeof(ILogger<Function>)))
            .Returns(_logger.Object);

        _function = new Function(_serviceProvider.Object, _logger.Object);
    }

    private void SetupConfigurationValue(string key, string value)
    {
        var configSection = new Mock<IConfigurationSection>();
        configSection.Setup(x => x.Value).Returns(value);
        _configuration.Setup(x => x.GetSection(key)).Returns(configSection.Object);
    } // end method

    [Fact]
    public async Task ExecuteFunction()
    {
        SetupConfigurationValue("EXPORT_BUCKET", "exports");
        SetupConfigurationValue("PAGE_VIEWS_TABLE_ARN", "pageviews_arn");
        SetupConfigurationValue("COLLECTED_PII_TABLE_ARN", "collected_pii_arn");
        SetupConfigurationValue("PII_VALUES_TABLE_ARN", "pii_values_arn");

        var estTimeSpan = TimeSpan.FromHours(-4);
        var startOffset = new DateTimeOffset(2025, 9, 18, 0, 0, 0, estTimeSpan);
        var endOffset = new DateTimeOffset(2025, 9, 19, 0, 0, 0, estTimeSpan);
        _timeRangeProvider.Setup(x => x.TimeRange).Returns(new TimeRange()
        {
            UtcStart = Instant.FromDateTimeOffset(startOffset),
            UtcEnd = Instant.FromDateTimeOffset(endOffset)
        });

        _incrementalExporter
            .Setup(x => x.ExportAsync(It.IsAny<IncrementalExportRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var stream = new MemoryStream();
        await _function.FunctionHandlerAsync(stream).ConfigureAwait(false);

        Assert.Equal(new DateTime(2025, 9, 18, 4, 0, 0), startOffset.UtcDateTime);
        Assert.Equal(new DateTime(2025, 9, 19, 4, 0, 0), endOffset.UtcDateTime);

        _incrementalExporter.Verify(x => x.ExportAsync(It.Is<IncrementalExportRequest>(x => 
            x.TableArn == "pageviews_arn"
            && x.S3Bucket == "exports"
            && x.S3Prefix == $"{startOffset.ToString("yyyy/MM/dd")}/pageviews/"
            && x.FromTime == startOffset.UtcDateTime
            && x.ToTime == endOffset.UtcDateTime
            && x.ExportFormat == ExportFormat.DYNAMODB_JSON
            && x.ExportType == ExportType.INCREMENTAL_EXPORT), It.IsAny<CancellationToken>()), Times.Exactly(1));
        _incrementalExporter.Verify(x => x.ExportAsync(It.Is<IncrementalExportRequest>(x => x.TableArn == "collected_pii_arn"), It.IsAny<CancellationToken>()), Times.Exactly(1));
        _incrementalExporter.Verify(x => x.ExportAsync(It.Is<IncrementalExportRequest>(x => x.TableArn == "pii_values_arn"), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}