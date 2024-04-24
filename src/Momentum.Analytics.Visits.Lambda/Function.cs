using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core;
using Momentum.Analytics.Processing.Visits.Interfaces;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

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
                        config.AddLambdaLogger();
                    })
                .AddMemoryCache()
                .AddSingleton<IConfiguration>(configuration)
                .AddNodaTime()
                .AddSingleton<IIdentifiedVisitTimeRangeProvider, IdentifiedVisitTimeRangeProvider>()
                .AddIdentifiedVisitProcessor()
                .AddUnidentifiedVisitProcessor()
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
        public async Task FunctionHandlerAsync(Stream input)
        {
            var timeRangeProvider = _serviceProvider.GetRequiredService<IIdentifiedVisitTimeRangeProvider>();

            
            var unidentifiedVisitProcessor = _serviceProvider.GetRequiredService<IUnidentifiedVisitProcessor>();

            _logger.LogDebug("Starting unidentified visits.");
            await unidentifiedVisitProcessor.ProcessAsync(timeRangeProvider.TimeRange).ConfigureAwait(false);

            var identifiedVisitProcessor = _serviceProvider.GetRequiredService<IIdentifiedVisitProcessor>();

            _logger.LogDebug("Starting identified visits.");
            await identifiedVisitProcessor.ReportAsync(timeRangeProvider.TimeRange).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace