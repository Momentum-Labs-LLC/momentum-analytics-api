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

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Momentum.Analytics.Pii.Lambda
{
    public class Function
    {
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

        public async Task FunctionHandlerAsync(DynamoDBEvent dynamoEvent)
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
                throw new ArgumentException("Stream record did not contain the new image.");
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace