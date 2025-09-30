using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public class SearchRequest<TPage> : ISearchRequest<TPage>
    {
        public required TPage Page { get; set; }
        public int Size { get; set; } = 10;
    } // end class

    public class SearchRequest : SearchRequest<int>, ISearchRequest 
    {
        public SearchRequest()
        {
            Page = 1;
        } // end method
    } // end class
} // end namespace