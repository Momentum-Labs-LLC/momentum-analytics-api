using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public class SearchRequest : ISearchRequest
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    } // end class
} // end namespace