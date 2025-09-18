using Amazon.DynamoDBv2;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Models;

public record IncrementalExportRequest(
    string TableArn, 
    string S3Bucket, 
    string S3Prefix, 
    DateTime FromTime, 
    DateTime ToTime,
    ExportFormat ExportFormat,
    ExportType ExportType)
: IIncremenalExportRequest; // end record