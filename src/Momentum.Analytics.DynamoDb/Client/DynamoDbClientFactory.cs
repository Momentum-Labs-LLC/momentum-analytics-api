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

        public virtual Task<IAmazonDynamoDB> GetAsync(CancellationToken token = default)
        {
            IAmazonDynamoDB result = new AmazonDynamoDBClient();
            if(!string.IsNullOrWhiteSpace(_clientConfiguration.ServiceUrl))
            {
                var config = new AmazonDynamoDBConfig()
                {
                    ServiceURL = _clientConfiguration.ServiceUrl
                };
                result = new AmazonDynamoDBClient(config);
            } // end if
            
            return Task.FromResult(result);
        } // end method
    } // end class
} // end namespace