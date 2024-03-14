using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Models
{
    public class DynamoSearchResponse<T> : 
        SearchResponse<T, Dictionary<string, AttributeValue>>, 
        IDynamoSearchResponse<T>
    {
        
    } // end class
} // end namespace