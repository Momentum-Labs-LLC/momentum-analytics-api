using System.Globalization;
using Amazon.S3.Model;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Visits.Lambda
{
    public class S3IdentifiedVisitWriter : IS3IdentifiedVisitWriter
    {
        protected readonly IS3OutputConfiguration _outputConfiguration;
        protected readonly IS3ClientFactory _s3ClientFactory;
        protected readonly IVisitConfiguration _visitConfiguration;
        protected readonly ILogger _logger;
        public S3IdentifiedVisitWriter(
            IS3OutputConfiguration outputConfiguration,
            IS3ClientFactory s3ClientFactory,
            IVisitConfiguration visitConfiguration,
            ILogger<S3IdentifiedVisitWriter> logger)
        {
            _outputConfiguration = outputConfiguration ?? throw new ArgumentNullException(nameof(outputConfiguration));
            _s3ClientFactory = s3ClientFactory ?? throw new ArgumentNullException(nameof(s3ClientFactory));
            _visitConfiguration = visitConfiguration ?? throw new ArgumentNullException(nameof(visitConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task WriteAsync(
            ITimeRange timeRange, 
            IEnumerable<Visit> visits, 
            CancellationToken token = default)
        {
            var s3Client = await _s3ClientFactory.GetAsync(token);
            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = _outputConfiguration.Bucket,
                Key = await _outputConfiguration.BuildKeyAsync(timeRange, token).ConfigureAwait(false),
                InputStream = await BuildStreamAsync(visits, token).ConfigureAwait(false)
            };
            var putObjectResponse = await s3Client.PutObjectAsync(putObjectRequest, token).ConfigureAwait(false);

            if(putObjectResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("failed to write s3 file.");
            }
        } // end method

        protected virtual async Task<Stream> BuildStreamAsync(
            IEnumerable<Visit> visits, 
            CancellationToken token = default)
        {
            var rows = visits.Select(x => new 
            {
                Pii = x.PiiValue,
                PiiTypeId = (int)x.PiiType,
                VisitStart = x.UtcStart.InZone(_visitConfiguration.TimeZone).ToDateTimeOffset().ToString("yyyy-MM-dd HH:mm:ss"),
                FunnelStep = x.FunnelStep,
                Referer = x.Referer,
                Source = x.Source,
                Medium = x.Medium
            });

            var result = new MemoryStream();
            using (var stream = new StreamWriter(result, leaveOpen: true))
            using (var csv = new CsvWriter(stream, CultureInfo.InvariantCulture))
            {	
                await csv.WriteRecordsAsync(rows, token);
                
                await csv.FlushAsync().ConfigureAwait(false);
                await stream.FlushAsync(token).ConfigureAwait(false);
            } // end using

            return result;
        } // end method
    } // end class
} // end namespace