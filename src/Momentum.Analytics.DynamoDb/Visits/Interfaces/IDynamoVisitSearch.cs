using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Visits.Interfaces
{
    public interface IDynamoVisitSearch : IDynamoSearchRequest, IVisitSearchRequest
    {
        
    } // end interface
} // end namespace