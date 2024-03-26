using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.Processing.DynamoDb.Visits;
using Momentum.Analytics.Processing.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Visits.Interfaces;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Momentum.Analytics.Visits.Lambda
{
    public class Function
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;

        public Function()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var services = new ServiceCollection();
            services
                .AddLogging(config => 
                    {
                        config.AddFilter("Microsoft", LogLevel.Warning);
                        config.AddFilter("System", LogLevel.Warning);
                        config.SetMinimumLevel(LogLevel.Debug);
                    })
                .AddMemoryCache()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IVisitTimeRangeProvider, VisitTimeRangeProvider>()
                .AddDynamoDbVisitService()
                .AddSingleton<IS3ClientFactory, S3ClientFactory>()
                .AddSingleton<IS3OutputConfiguration, S3OutputConfiguration>()
                .AddTransient<IIdentifiedVisitWriter, S3IdentifiedVisitWriter>()
                //.AddTransient<IDynamoDbIdentifiedVisitProcessor, DynamoDbIdentifiedVisitProcessor>();
                ;
            _serviceProvider = services.BuildServiceProvider();    
            _logger = _serviceProvider.GetRequiredService<ILogger<Function>>();
        } // end method

        public Function(IServiceProvider serviceProvider, ILogger<Function> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(Stream input)
        {
            var timeRangeProvider = _serviceProvider.GetRequiredService<IVisitTimeRangeProvider>();
            var visitProcessor = _serviceProvider.GetRequiredService<IDynamoDbIdentifiedVisitProcessor>();

            await visitProcessor.ExportAsync(timeRangeProvider.TimeRange).ConfigureAwait(false);
        } // end method
    } // end class

    public class S3OutputConfiguration : IS3OutputConfiguration
    {
        public const string OUTPUT_BUCKET = "OUTPUT_BUCKET";
        public const string OUTPUT_BUCKET_DEFAULT = "momentum-prd-visits";

        public string Bucket { get; protected set; }

        public S3OutputConfiguration(IConfiguration configuration)
        {
            Bucket = configuration.GetValue(OUTPUT_BUCKET, OUTPUT_BUCKET_DEFAULT);
        } // end method

        public virtual async Task<string> BuildKeyAsync(ITimeRange timeRange, CancellationToken token = default)
        {
            return $"{timeRange.UtcStart.Value.ToString("yyyyMM")}/{timeRange.UtcStart.Value.ToString("yyyyMMddHH")}_{timeRange.UtcEnd.Value.ToString("yyyyMMddHH")}.csv";
        } // end method
    } // end class
} // end namespace