using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb;

public class IncrementalExporter : IIncrementalExporter
{
    protected readonly IDynamoDBClientFactory _clientFactory;
    protected readonly ILogger _logger;

    public IncrementalExporter(IDynamoDBClientFactory clientFactory, ILogger<IncrementalExporter> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    } // end method

    public async Task ExportAsync(IIncremenalExportRequest request, CancellationToken token = default)
    {
        var exportRequest = new ExportTableToPointInTimeRequest()
        {
            TableArn = request.TableArn,
            S3Bucket = request.S3Bucket,
            S3Prefix = request.S3Prefix,
            ExportFormat = request.ExportFormat,
            ExportType = request.ExportType,
            IncrementalExportSpecification = new IncrementalExportSpecification()
            {
                ExportFromTime = request.FromTime,
                ExportToTime = request.ToTime
            }
        };

        var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
        await client.ExportTableToPointInTimeAsync(exportRequest, token).ConfigureAwait(false);
    } // end method
} // end class