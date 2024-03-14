using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public class SearchResponse<T, TPage> : ISearchResponse<T, TPage>
    {
        public long Total { get; set; }

        public bool HasMore { get; set; }

        public IEnumerable<T>? Data { get; set; }

        public TPage NextPage { get; set; }
    } // end class

    public class SearchResponse<T> : SearchResponse<T, int>, ISearchResponse<T> {} // end class
} // end namespace