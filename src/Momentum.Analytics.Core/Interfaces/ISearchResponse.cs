using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface ISearchResponse<T>
    {
        /// <summary>
        /// Gets the total number of matching items for the search.
        /// </summary>
        long Total { get; }

        /// <summary>
        /// Gets if there is more data available on the next page.
        /// </summary>
        bool HasMore { get; }

        /// <summary>
        /// Gets the data returned.
        /// </summary>
        IEnumerable<T>? Data { get; }
    } // end interface
} // end namespace