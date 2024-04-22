using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Processing.Visits.Interfaces
{
    public interface IUnidentifiedVisitProcessor
    {
        Task ProcessAsync(ITimeRange timeRange, CancellationToken token = default);
    } // end interface
} // end namespace