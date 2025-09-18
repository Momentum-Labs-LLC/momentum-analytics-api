using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.Abstractions;

public interface IIncremenalExportRequest
{
    string TableArn { get; }
    string S3Bucket { get; }
    string S3Prefix { get; }
    DateTime FromTime { get; }
    DateTime ToTime { get; }
    ExportFormat ExportFormat { get; }
    ExportType ExportType { get; }
}

public interface IIncrementalExporter
{
    Task ExportAsync(IIncremenalExportRequest request, CancellationToken token = default);
} // end interface