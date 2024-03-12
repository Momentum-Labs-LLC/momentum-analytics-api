using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.Abstractions
{
    public abstract class ResourceStorageBase<TConfig> : IResourceStorage
        where TConfig : ITableConfiguration
    {
        protected readonly IDynamoDBClientFactory _clientFactory;
        protected readonly TConfig _tableConfiguration;
        protected readonly ILogger _logger;

        protected ResourceStorageBase(
            IDynamoDBClientFactory clientFactory,
            TConfig tableConfiguration, 
            ILogger<ResourceStorageBase<TConfig>> logger)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _tableConfiguration = tableConfiguration ?? throw new ArgumentNullException(nameof(tableConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method
    } // end class
} // end namespace