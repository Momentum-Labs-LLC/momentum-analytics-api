using System.Text.Json;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.DynamoDb;
using Momentum.Analytics.DynamoDb.Pii;
using Momentum.Analytics.Processing.DynamoDb.Pii.Interfaces;
using Moq;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;

namespace Momentum.Analytics.Pii.Lambda.Tests
{
    public class FunctionTests
    {
        private Mock<IConfiguration> _configuration;
        private Mock<IForceFailureProvider> _forceFailure;
        private Mock<IDynamoDbCollectedPiiProcessor> _processor;
        private Mock<IPiiService> _piiService;
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<ILogger<TestFunction>> _logger;

        private TestFunction _function;

        public FunctionTests()
        {
            _serviceProvider = new Mock<IServiceProvider>();
            _logger = new Mock<ILogger<TestFunction>>();

            _processor = new Mock<IDynamoDbCollectedPiiProcessor>();
            _serviceProvider.Setup(x => x.GetService(typeof(IDynamoDbCollectedPiiProcessor)))
                .Returns(_processor.Object);
            _piiService = new Mock<IPiiService>();
            _serviceProvider.Setup(x => x.GetService(typeof(IPiiService)))
                .Returns(_piiService.Object);

            _configuration = new Mock<IConfiguration>();
            _serviceProvider.Setup(x => x.GetService(typeof(IConfiguration)))
                .Returns(_configuration.Object);

            _forceFailure = new Mock<IForceFailureProvider>();
            _serviceProvider.Setup(x => x.GetService(typeof(IForceFailureProvider)))
                .Returns(_forceFailure.Object);

            SetupForceFailure();

            _function = new TestFunction(
                _serviceProvider.Object,
                _logger.Object);
        } // end method

        protected void SetupForceFailure(bool forceFailure = false)
        {
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x.Value).Returns(forceFailure.ToString().ToLower());

            _configuration.Setup(x => x.GetSection(ForceFailureProvider.FORCE_FAILURE_KEY))
                .Returns(configSection.Object);

            _forceFailure.Setup(x => x.ShouldForceFailure).Returns(forceFailure);
        } // end method

        protected string Serialize<T>(T input)
        {
            return JsonSerializer.Serialize(input);
        } // end method

        protected Stream ToStream<T>(T input)
        {
            MemoryStream result = new MemoryStream();

            var json = Serialize(input);
            
            using(StreamWriter writer = new StreamWriter(result, leaveOpen: true))
            {
                writer.Write(json);
                writer.Flush();
            } // end using

            result.Position = 0;

            return result;
        } // end method

        [Fact]
        public async Task DI_PiiService()
        {
            _function = new TestFunction();
            var piiService = _function.GetPiiService();

            Assert.NotNull(piiService);
            Assert.IsAssignableFrom<IPiiService>(piiService);
        } // end method

        [Fact]
        public async Task DI_Processor()
        {
            _function = new TestFunction();
            var processor = _function.GetProcessor();

            Assert.NotNull(processor);
            Assert.IsAssignableFrom<IDynamoDbCollectedPiiProcessor>(processor);
        } // end method

        [Fact]
        public async Task FunctionHandlerAsync()
        {
            var now = NodaTime.SystemClock.Instance.GetCurrentInstant();
            var piiId = Guid.NewGuid();
            var piiValue = new PiiValue()
            {
                Id = piiId,
                Value = "12345",
                PiiType = PiiTypeEnum.Email,
                UtcTimestamp = now
            };
            _piiService.Setup(x => x.GetPiiAsync(piiId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(piiValue);

            _processor.Setup(x => x.ProcessAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var input = new DynamoDBEvent()
            {
                Records = new List<DynamodbStreamRecord>()
                {
                    new DynamodbStreamRecord()
                    {
                        EventID = "EventId",
                        EventName = "Name",
                        Dynamodb = new Amazon.DynamoDBv2.Model.StreamRecord()
                        {
                            NewImage = new Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>()
                                .AddField(CollectedPiiConstants.PII_ID, piiId)
                                .AddField(CollectedPiiConstants.COOKIE_ID, Guid.NewGuid())
                                .AddField(CollectedPiiConstants.UTC_TIMESTAMP, now)
                        }
                    }
                }
            };

            var inputStream = ToStream(input);
            await _function.FunctionHandlerAsync(inputStream).ConfigureAwait(false);

            _piiService.Verify(x => x.GetPiiAsync(piiId, It.IsAny<CancellationToken>()), Times.Once);
            _processor.Verify(x => x.ProcessAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task FunctionHandlerAsync_SQSEvent()
        {
            var now = NodaTime.SystemClock.Instance.GetCurrentInstant();
            var piiId = Guid.NewGuid();
            var piiValue = new PiiValue()
            {
                Id = piiId,
                Value = "12345",
                PiiType = PiiTypeEnum.Email,
                UtcTimestamp = now
            };
            _piiService.Setup(x => x.GetPiiAsync(piiId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(piiValue);

            _processor.Setup(x => x.ProcessAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var dynamoDbEvent = new DynamoDBEvent()
            {
                Records = new List<DynamodbStreamRecord>()
                {
                    new DynamodbStreamRecord()
                    {
                        EventID = "EventId",
                        EventName = "Name",
                        Dynamodb = new Amazon.DynamoDBv2.Model.StreamRecord()
                        {
                            NewImage = new Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>()
                                .AddField(CollectedPiiConstants.PII_ID, piiId)
                                .AddField(CollectedPiiConstants.COOKIE_ID, Guid.NewGuid())
                                .AddField(CollectedPiiConstants.UTC_TIMESTAMP, now)
                        }
                    }
                }
            };

            var dynamoDbEventJson = Serialize(dynamoDbEvent);

            var sqsEvent = new SQSEvent()
            {
                Records = new List<SQSEvent.SQSMessage>()
                {
                    new SQSEvent.SQSMessage()
                    {
                        Body = dynamoDbEventJson
                    }
                }
            };

            var inputStream = ToStream(sqsEvent);
            await _function.FunctionHandlerAsync(inputStream).ConfigureAwait(false);

            _piiService.Verify(x => x.GetPiiAsync(piiId, It.IsAny<CancellationToken>()), Times.Once);
            _processor.Verify(x => x.ProcessAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    
        [Fact]
        public async Task FunctionHandlerAysnc_PiiNotFound()
        {
            
            var now = NodaTime.SystemClock.Instance.GetCurrentInstant();
            var piiId = Guid.NewGuid();
            _piiService.Setup(x => x.GetPiiAsync(piiId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PiiValue)null);

            _processor.Setup(x => x.ProcessAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var input = new DynamoDBEvent()
            {
                Records = new List<DynamodbStreamRecord>()
                {
                    new DynamodbStreamRecord()
                    {
                        EventID = "EventId",
                        EventName = "Name",
                        Dynamodb = new Amazon.DynamoDBv2.Model.StreamRecord()
                        {
                            NewImage = new Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>()
                                .AddField(CollectedPiiConstants.PII_ID, piiId)
                                .AddField(CollectedPiiConstants.COOKIE_ID, Guid.NewGuid())
                                .AddField(CollectedPiiConstants.UTC_TIMESTAMP, now)
                        }
                    }
                }
            };

            var inputStream = ToStream(input);
            await Assert.ThrowsAsync<Exception>(() => _function.FunctionHandlerAsync(inputStream));

            _piiService.Verify(x => x.GetPiiAsync(piiId, It.IsAny<CancellationToken>()), Times.Once);
            _processor.Verify(x => x.ProcessAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method
    
        [Fact]
        public async Task FunctionHandlerAsync_ForcedFailure()
        {      
            SetupForceFailure(true);      
            try
            {
                var stream = new MemoryStream();
                await _function.FunctionHandlerAsync(stream).ConfigureAwait(false);
                Assert.Fail("should be an exception");
            }
            catch(Exception ex)
            {
                Assert.Equal("FORCE_FAILURE=true", ex.Message);
                _serviceProvider.Verify(x => x.GetService(typeof(IForceFailureProvider)), Times.Once);
            } // end try/catch
        } // end method
    } // end class

    public class TestFunction : Function
    {
        public TestFunction() : base() {} // end method

        public TestFunction(IServiceProvider serviceProvider, ILogger<TestFunction> logger)
            : base(serviceProvider, logger)
        {

        } // end method
        public IPiiService GetPiiService()
        {
            return _serviceProvider.GetService<IPiiService>();
        } // end method

        public IDynamoDbCollectedPiiProcessor GetProcessor()
        {
            return _serviceProvider.GetService<IDynamoDbCollectedPiiProcessor>();
        } // end method
    } // end class
} // end namespace