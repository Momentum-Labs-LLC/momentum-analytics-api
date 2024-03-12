using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.DynamoDb.Abstractions
{
    public interface ITableConfiguration
    {
        string TableName { get; }
    } // end interface
} // end namespace