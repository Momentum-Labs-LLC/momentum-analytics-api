using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.DynamoDb.PageViews;
using Momentum.Analytics.Processing.DynamoDb.PageViews;
using Momentum.Analytics.Processing.DynamoDb.PageViews.Interfaces;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Momentum.Analytics.PageViews.Lambda
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
                .AddLogging()
                .AddSingleton<IConfiguration>(config)
                .AddDynamoDbPageViewProcessor()
                .BuildServiceProvider();

            _logger = _serviceProvider.GetRequiredService<ILogger<Function>>();
        } // end method

        public Function(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        } // end method

        public async void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            _logger.LogInformation($"Beginning to process {dynamoEvent.Records.Count} records...");
            
            if(dynamoEvent != null && dynamoEvent.Records != null && dynamoEvent.Records.Any())
            {
                var pageViewProcessor = _serviceProvider.GetRequiredService<IDynamoDbPageViewProcessor>();

                foreach (var record in dynamoEvent.Records)
                {
                    _logger.LogInformation($"Event ID: {record.EventID}");
                    _logger.LogInformation($"Event Name: {record.EventName}");
                    
                    try
                    {
                        var pageView = BuildPageView(record.Dynamodb);
                        await pageViewProcessor.ProcessAsync(pageView).ConfigureAwait(false);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("Unable to process page view in event: {EventId}.", record.EventID);
                    } // end try/catch                    
                } // end foreach
            } // end if

            _logger.LogInformation("Stream processing complete.");
        } // end method

        protected PageView? BuildPageView(StreamRecord streamRecord)
        {            
            PageView? result = null;
            if(streamRecord != null && streamRecord.NewImage != null && streamRecord.NewImage.Any())
            {
                result = streamRecord.NewImage.ToPageView();
            }
            else
            {
                throw new ArgumentException("Stream record did not contain the new image.");
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace
