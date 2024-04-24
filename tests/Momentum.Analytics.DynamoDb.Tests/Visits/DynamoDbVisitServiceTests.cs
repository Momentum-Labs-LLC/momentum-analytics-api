using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Models;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Moq;
using NodaTime;
using NodaTime.Extensions;

namespace Momentum.Analytics.DynamoDb.Tests.Visits
{
    public class DynamoDbVisitServiceTests
    {
        /*
        IVisitWindowCalculator visitWindowCalculator,
                IDynamoDbVisitStorage visitStorage, 
                IMemoryCache memoryCache, 
                ILogger<DynamoDbVisitService> logger        
        */
        private Mock<IVisitWindowCalculator> _visitWindowCalculator;
        private Mock<IDynamoDbVisitStorage> _visitStorage;
        private Mock<ILogger<DynamoDbVisitService>> _logger;
        private DynamoDbVisitService _service;

        public DynamoDbVisitServiceTests()
        {
            _visitWindowCalculator = new Mock<IVisitWindowCalculator>();
            _visitStorage = new Mock<IDynamoDbVisitStorage>();
            _logger = new Mock<ILogger<DynamoDbVisitService>>();
            _service = new DynamoDbVisitService(
                _visitWindowCalculator.Object,
                _visitStorage.Object,
                _logger.Object);
        } // end method

        [Fact]
        public async Task GetIdentifiedAsync_MultipleHours()
        {
            _visitStorage.SetupSequence(x => x.GetIdentifiedAsync(It.IsAny<Instant>(), It.IsAny<Dictionary<string, AttributeValue>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DynamoSearchResponse<Visit>()
                {
                    NextPage = null,
                    Data = new List<Visit>()
                    {
                        new Visit()
                        {
                            Id = Guid.NewGuid(),
                            CookieId = Guid.NewGuid(),
                            UtcIdentifiedTimestamp = DateTime.UtcNow.ToInstant().Minus(Duration.FromMinutes(90))
                        }
                    }
                })
                .ReturnsAsync(new DynamoSearchResponse<Visit>()
                {
                    NextPage = null,
                    Data = new List<Visit>()
                    {
                        
                    }
                });

            var now = DateTime.UtcNow.ToInstant();
            var timeRange = new TimeRange()
            {
                UtcStart = now.Minus(Duration.FromHours(2)),
                UtcEnd = now
            };
            var visits = await _service.GetIdentifiedAsync(timeRange, null).ConfigureAwait(false);

            Assert.NotNull(visits.Data);
            Assert.NotEmpty(visits.Data);
            Assert.Equal(1, visits.Data.Count());

            _visitStorage.Verify(x => x.GetIdentifiedAsync(It.IsAny<Instant>(), It.IsAny<Dictionary<string, AttributeValue>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        } // end method

        [Fact]
        public async Task GetUnidentifiedAsync_MultipleHours()
        {
            _visitStorage.SetupSequence(x => x.GetUnidentifiedAsync(It.IsAny<Instant>(), It.IsAny<Dictionary<string, AttributeValue>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DynamoSearchResponse<Visit>()
                {
                    NextPage = null,
                    Data = new List<Visit>()
                    {
                        new Visit()
                        {
                            Id = Guid.NewGuid(),
                            CookieId = Guid.NewGuid(),
                            UtcStart = DateTime.UtcNow.ToInstant().Minus(Duration.FromMinutes(90))
                        }
                    }
                })
                .ReturnsAsync(new DynamoSearchResponse<Visit>()
                {
                    NextPage = null,
                    Data = new List<Visit>()
                    {
                        
                    }
                });

            var now = DateTime.UtcNow.ToInstant();
            var timeRange = new TimeRange()
            {
                UtcStart = now.Minus(Duration.FromHours(2)),
                UtcEnd = now
            };
            var visits = await _service.GetUnidentifiedAsync(timeRange, null).ConfigureAwait(false);

            Assert.NotNull(visits.Data);
            Assert.NotEmpty(visits.Data);
            Assert.Equal(1, visits.Data.Count());

            _visitStorage.Verify(x => x.GetUnidentifiedAsync(It.IsAny<Instant>(), It.IsAny<Dictionary<string, AttributeValue>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        } // end method
    } // end class
} // end namespace