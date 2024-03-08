using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitConfiguration
    {
        /// <summary>
        /// Gets or sets if the window for a visit is fixed or sliding.
        /// </summary>
        bool IsSliding { get; }

        /// <summary>
        /// Gets or sets the window's length of time.
        /// </summary>
        TimeSpan WindowLength { get; }

        /// <summary>
        /// An offset of the day to start queries from.
        /// </summary>
        TimeSpan FixedOffset { get; }
    } // end interface
} // end namespace