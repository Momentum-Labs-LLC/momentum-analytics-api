using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.DynamoDBEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.DynamoDb.PageViews;
using Momentum.Analytics.Processing.DynamoDb.PageViews.Interfaces;
using Moq;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;

namespace Momentum.Analytics.PageViews.Lambda.Tests
{
    public class FunctionTests
    {
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<ILogger<TestFunction>> _logger;

        private Mock<IDynamoDbPageViewProcessor> _processor;

        private TestFunction _function;
        public FunctionTests()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            _logger = new Mock<ILogger<TestFunction>>();

            _processor = new Mock<IDynamoDbPageViewProcessor>();
            _serviceProvider.Setup(x => x.GetService(typeof(IDynamoDbPageViewProcessor)))
                .Returns(_processor.Object);

            _function = new TestFunction(_serviceProvider.Object, _logger.Object);
        } // end method

        [Fact]
        public void DI_Processor()
        {
            _function = new TestFunction();
            var processor = _function.GetProcessor();

            Assert.NotNull(processor);
            Assert.IsAssignableFrom<IDynamoDbPageViewProcessor>(processor);
        } // end method

        [Fact]
        public async Task FunctionHandlerAsync()
        {
            var pageView = new PageView()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = NodaTime.SystemClock.Instance.GetCurrentInstant(),
                Domain = "test.org",
                Path = "/"
            };
            var input = new DynamoDBEvent()
            {
                Records = new List<DynamodbStreamRecord>()
                {
                    new DynamodbStreamRecord()
                    {
                        EventID = "id",
                        EventName = "name",
                        Dynamodb = new StreamRecord()
                        {
                            NewImage = pageView.ToDynamoDbEvent()
                        }
                    }
                }
            };

            await _function.FunctionHandlerAsync(input);

            _processor.Verify(x => x.ProcessAsync(It.IsAny<PageView>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class

    public class TestFunction : Function
    {
        public TestFunction() : base() { } // end method
        public TestFunction(IServiceProvider serviceProvider, ILogger<TestFunction> logger) : base(serviceProvider, logger) {} // end method

        public IDynamoDbPageViewProcessor GetProcessor()
        {
            return _serviceProvider.GetRequiredService<IDynamoDbPageViewProcessor>();
        } // end method
    } // end method
} // end namespace