using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.DynamoDb.Abstractions
{
    public interface IDynamoSearchRequest : ISearchRequest<Dictionary<string, AttributeValue>>
    {
        
    } // end interface
} // end namespace