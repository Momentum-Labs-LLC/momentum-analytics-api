using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface ISearchResponse<T, TNextPage>
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

        /// <summary>
        /// Gets or sets the information required to get the next page.
        /// </summary>
        TNextPage? NextPage { get; }
    } // end interface

    public interface ISearchResponse<T> : ISearchResponse<T, int> {} // end interface
} // end namespace