using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.PII.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.DynamoDb.Pii;
using Momentum.Analytics.Processing.DynamoDb.Pii.Interfaces;
using Momentum.Analytics.Processing.DynamoDb.Pii;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.Processing.Cookies;
using Amazon.Lambda.Serialization.SystemTextJson;
using Momentum.Analytics.Pii.Lambda;
using System.Text.Json;
using Amazon.Lambda.SQSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<CustomSerializer>))]

namespace Momentum.Analytics.Pii.Lambda
{
    public class Function
    {
        private const string SQS_UNIQUE_VALUE = "Md5OfBody";
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;

        public Function()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            IServiceCollection services = new ServiceCollection();
            _serviceProvider = services
                .AddMemoryCache()
                .AddLogging(config => 
                    {
                        config.AddFilter("Microsoft", LogLevel.Warning);
                        config.AddFilter("System", LogLevel.Warning);
                        config.SetMinimumLevel(LogLevel.Debug);
                        config.AddLambdaLogger();
                    })
                .AddSingleton<IConfiguration>(config)                
                .AddNodaTime()
                .AddSingleton<IForceFailureProvider, ForceFailureProvider>()
                .AddVisitWindowCalculator()
                .AddDynamoDbPiiService()
                .AddDynamoDbVisitService()
                .AddSharedCookieConfiguration()
                .AddTransient<IDynamoDbCollectedPiiProcessor, DynamoDbCollectedPiiProcessor>()
                .BuildServiceProvider();

            _logger = _serviceProvider.GetRequiredService<ILogger<Function>>();
        } // end method

        public Function(IServiceProvider serviceProvider, ILogger<Function> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task FunctionHandlerAsync(Stream input)
        {
            var forceFailureProvider = _serviceProvider.GetRequiredService<IForceFailureProvider>();
            if(forceFailureProvider.ShouldForceFailure)
            {
                _logger.LogCritical("FORCE_FAILURE=true");
                throw new Exception("FORCE_FAILURE=true");
            } // end if

            var dynamoEvents = new List<DynamoDBEvent>();
            using(StreamReader reader = new StreamReader(input))
            {
                var json = await reader.ReadToEndAsync().ConfigureAwait(false);

                if(!json.Contains(SQS_UNIQUE_VALUE, StringComparison.OrdinalIgnoreCase))
                {
                    var dynamoEvent = JsonSerializer.Deserialize<DynamoDBEvent>(json);
                    if(dynamoEvent != null)
                    {
                        dynamoEvents.Add(dynamoEvent);
                    } // end if
                }
                else
                {
                    try
                    {
                        var sqsEvent = JsonSerializer.Deserialize<SQSEvent>(json);
                        if(sqsEvent != null)
                        {
                            var events = await ReadEventsFromSqsAsync(sqsEvent).ConfigureAwait(false);

                            if(events != null && events.Any())
                            {
                                dynamoEvents.AddRange(events);
                            } // end if
                        } // end if
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(new EventId(0), $"Unable to deserialize json: {json}.");
                    } // end try/catch
                } // end if
            } // end using

            foreach(var dynamoEvent in dynamoEvents)
            {
                await HandleDynamoDbEventAsync(dynamoEvent).ConfigureAwait(false);
            } // end foreach
        } // end if

        protected async Task<IEnumerable<DynamoDBEvent>> ReadEventsFromSqsAsync(SQSEvent sqsEvent, CancellationToken token = default)
        {
            var dynamoDbEvents = new List<DynamoDBEvent>();

            if(sqsEvent != null && sqsEvent.Records != null && sqsEvent.Records.Any())
            {
                dynamoDbEvents = sqsEvent.Records
                    .Select(msg => JsonSerializer.Deserialize<DynamoDBEvent>(msg.Body))
                    .Where(x => x != null)
                    .ToList();
            } // end if

            return dynamoDbEvents;
        } // end method

        protected async Task HandleDynamoDbEventAsync(DynamoDBEvent dynamoEvent, CancellationToken token = default)
        {
            _logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");
            
            if(dynamoEvent != null && dynamoEvent.Records != null && dynamoEvent.Records.Any())
            {
                var piiProcessor = _serviceProvider.GetRequiredService<IDynamoDbCollectedPiiProcessor>();
                var piiService = _serviceProvider.GetRequiredService<IPiiService>();

                foreach (var record in dynamoEvent.Records)
                {
                    _logger.LogInformation($"Event ID: {record.EventID}");
                    _logger.LogInformation($"Event Name: {record.EventName}");
                    
                    try
                    {
                        var collectedPii = BuildCollectedPii(record.Dynamodb);
                        if(collectedPii != null) 
                        {
                            var piiValue = await piiService.GetPiiAsync(collectedPii.PiiId.Value).ConfigureAwait(false);

                            if(piiValue != null)
                            {
                                collectedPii.Pii = piiValue;
                                await piiProcessor.ProcessAsync(collectedPii).ConfigureAwait(false);
                            }
                            else
                            {
                                throw new Exception($"Unable to retrieve pii by identifier: {collectedPii.PiiId}.");
                            } // end if
                        } // end if                        
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(new EventId(0), ex, "Failed to process collected pii.");
                        throw;
                    } // end try/catch                    
                } // end foreach
            } // end if

            _logger.LogInformation("Stream processing complete.");
        } // end method

        protected CollectedPii? BuildCollectedPii(StreamRecord streamRecord)
        {            
            CollectedPii? result = null;
            if(streamRecord != null && streamRecord.NewImage != null && streamRecord.NewImage.Any())
            {
                result = streamRecord.NewImage.ReadCollectedPii();
            }
            else
            {
                int keyCount = 0;
                if(streamRecord.Keys != null && streamRecord.Keys.Any())
                {
                    keyCount = streamRecord.Keys.Count();
                } // end if

                int oldImageCount = 0;
                if(streamRecord.OldImage != null && streamRecord.OldImage.Any())
                {
                    oldImageCount = streamRecord.OldImage.Count();
                } // end if
                _logger.LogWarning("Stream record did not contain the new image. Keys: {0}, OldImage: {1}", keyCount, oldImageCount);
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace