using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.Client
{
    public class DynamoClientConfiguration : IDynamoClientConfiguration
    {
        public string? ServiceUrl { get; set; }

        public DynamoClientConfiguration(IConfiguration configuration)
        {
            ServiceUrl = configuration.GetValue<string?>(ClientConstants.SERVICE_URL, ClientConstants.SERVICE_URL_DEFAULT);
        } // end method
    } // end class
} // end namespace