using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Moq;
using NodaTime;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class S3IdentifiedVisitWriterTests
    {
        private IClockService _clockService;
        private Mock<IS3ClientFactory> _s3ClientFactory;
        private Mock<IS3OutputConfiguration> _outputConfiguration;
        private Mock<IVisitConfiguration> _visitConfiguration;
        private Mock<ILogger<S3IdentifiedVisitWriter>> _logger;

        private Mock<IAmazonS3> _s3Client;

        private S3IdentifiedVisitWriter _writer;

        public S3IdentifiedVisitWriterTests()
        {
            _clockService = new ClockService();
            _s3ClientFactory = new Mock<IS3ClientFactory>();
            _outputConfiguration = new Mock<IS3OutputConfiguration>();
            _visitConfiguration = new Mock<IVisitConfiguration>();
            _logger = new Mock<ILogger<S3IdentifiedVisitWriter>>();

            _writer = new S3IdentifiedVisitWriter(
                _outputConfiguration.Object,
                _s3ClientFactory.Object,
                _visitConfiguration.Object,
                _logger.Object);

            _s3Client = new Mock<IAmazonS3>();
            _s3ClientFactory.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_s3Client.Object);

            SetupTimezone("America/New_York");
        } // end method

        protected void SetupTimezone(string timezoneId)
        {
            var timezone = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneId);
            _visitConfiguration.Setup(x => x.TimeZone).Returns(timezone);
        } // end method

        [Fact]
        public async Task WriteAsync_Simple()
        {
            var now = _clockService.Now;
            var visits = new List<Visit>() 
            { 
                new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    UtcStart = now.Minus(Duration.FromDays(1)),
                    UtcExpiration = now,
                    FunnelStep = 0,
                    PiiValue = "12345",
                    PiiType = Core.PII.Models.PiiTypeEnum.UserId,
                    UtcIdentifiedTimestamp = now.Minus(Duration.FromDays(1))
                }
            };

            var timeRange = new TimeRange()
            {
                UtcStart = now.Minus(Duration.FromDays(1)),
                UtcEnd = now
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