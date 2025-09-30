using Amazon.S3;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits
{
    public class S3ClientFactory : IS3ClientFactory
    {
        protected readonly ILogger _logger;

        public S3ClientFactory(ILogger<S3ClientFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Environment.SetEnvironmentVariable("AWS_REGION", "us-east-1");
        } // end method
        public virtual Task<IAmazonS3> GetAsync(CancellationToken token = default)
        {
            return Task.FromResult<IAmazonS3>(new AmazonS3Client());
        } // end method
    } // end class
} // end namespace