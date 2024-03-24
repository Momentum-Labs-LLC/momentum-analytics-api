using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Models;
using Moq;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class S3IdentifiedVisitWriterTests
    {
        private Mock<IS3ClientFactory> _s3ClientFactory;
        private Mock<IS3OutputConfiguration> _outputConfiguration;
        private Mock<ILogger<S3IdentifiedVisitWriter>> _logger;

        private Mock<IAmazonS3> _s3Client;

        private S3IdentifiedVisitWriter _writer;

        public S3IdentifiedVisitWriterTests()
        {
            _s3ClientFactory = new Mock<IS3ClientFactory>();
            _outputConfiguration = new Mock<IS3OutputConfiguration>();
            _logger = new Mock<ILogger<S3IdentifiedVisitWriter>>();

            _writer = new S3IdentifiedVisitWriter(
                _outputConfiguration.Object,
                _s3ClientFactory.Object,
                _logger.Object);

            _s3Client = new Mock<IAmazonS3>();
            _s3ClientFactory.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_s3Client.Object);
        } // end method

        [Fact]
        public async Task WriteAsync_Simple()
        {
            var visits = new List<Visit>() 
            { 
                new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    UtcStart = DateTime.UtcNow.AddDays(-1),
                    UtcExpiration = DateTime.UtcNow.Date,
                    FunnelStep = 0,
                    PiiValue = "12345",
                    PiiType = Core.PII.Models.PiiTypeEnum.UserId,
                    UtcIdentifiedTimestamp = DateTime.UtcNow.AddDays(-1)
                }
            };

            var timeRange = new TimeRange()
            {
                UtcStart = DateTime.UtcNow.AddDays(-1),
                UtcEnd = DateTime.UtcNow.Date
            };

            var putResponse = new PutObjectResponse()
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            };
            _s3Client.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(putResponse);

            await _writer.WriteAsync(timeRange, visits).ConfigureAwait(false);

            _s3Client.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class
} // end namespace