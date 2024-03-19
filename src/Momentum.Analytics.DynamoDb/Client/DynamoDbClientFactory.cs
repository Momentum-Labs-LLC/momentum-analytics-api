using Amazon;
using Amazon.DynamoDBv2;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.Client
{
    public class DynamoDbClientFactory : IDynamoDBClientFactory
    {
        protected IDynamoClientConfiguration _clientConfiguration;
        public DynamoDbClientFactory(IDynamoClientConfiguration clientConfiguration)
        {
            _clientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));
        } // end method

        public virtual async Task<IAmazonDynamoDB?> GetAsync(CancellationToken token = default)
        {
            IAmazonDynamoDB? result = null;
            if(string.IsNullOrWhiteSpace(_clientConfiguration.ServiceUrl))
            {
                result = new AmazonDynamoDBClient();
            }
            else
            {
                var config = new AmazonDynamoDBConfig()
                {
                    ServiceURL = _clientConfiguration.ServiceUrl
                };
                result = new AmazonDynamoDBClient(config);
            } // end if
            
            return result;
        } // end method
    } // end class
} // end namespace