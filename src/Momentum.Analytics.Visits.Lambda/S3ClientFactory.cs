using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.Extensions.Logging;

namespace Momentum.Analytics.Visits.Lambda
{
    public class S3ClientFactory : IS3ClientFactory
    {
        protected readonly ILogger _logger;

        public S3ClientFactory(ILogger<S3ClientFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method
        public virtual async Task<IAmazonS3> GetAsync(CancellationToken token = default)
        {
            return new AmazonS3Client();
        } // end method
    } // end class
} // end namespace