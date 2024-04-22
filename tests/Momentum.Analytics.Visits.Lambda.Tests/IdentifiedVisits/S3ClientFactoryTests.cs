using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits;
using Moq;

namespace Momentum.Analytics.Visits.Lambda.Tests.IdentifiedVisits
{
    public class S3ClientFactoryTests
    {
        private Mock<ILogger<S3ClientFactory>> _logger;
        private S3ClientFactory _factory;
    
        public S3ClientFactoryTests()
        {
            _logger = new Mock<ILogger<S3ClientFactory>>();
            _factory = new S3ClientFactory(_logger.Object);
        } // end method

        [Fact]
        public async Task GetAsync()
        {
            var result = _factory.GetAsync();
            Assert.NotNull(result);
        } // end method
    } // end class
} // end namespace