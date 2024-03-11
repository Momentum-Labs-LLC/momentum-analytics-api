using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public class SearchResponse<T> : ISearchResponse<T>
    {
        public long Total { get; set; }

        public bool HasMore { get; set; }

        public IEnumerable<T>? Data { get; set; }        
    } // end class
} // end namespace