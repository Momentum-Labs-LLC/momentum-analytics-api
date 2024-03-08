using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IIdentifiedVisitSearchRequest
    {
        string PiiValue { get; }
        PiiTypeEnum PiiType { get; }
    } // end interface
} // end namespace