using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public abstract class SearchResponse<T, TPage> : ISearchResponse<T, TPage>
    {
        public long Total { get; set; }

        public abstract bool HasMore { get; }

        public IEnumerable<T>? Data { get; set; }

        public TPage NextPage { get; set; }
    } // end class

    public class SearchResponse<T> : SearchResponse<T, int>, ISearchResponse<T>
    {
        public override bool HasMore { get { return NextPage > 1; } }
    } // end class
} // end namespace