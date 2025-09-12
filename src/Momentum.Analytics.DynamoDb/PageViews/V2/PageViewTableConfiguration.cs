using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.PageViews.Interfaces;

namespace Momentum.Analytics.DynamoDb.PageViews.V2
{
    public interface IPageViewV2TableConfiguration : IPageViewTableConfiguration {} // end interface
    public class PageViewTableConfiguration : TableConfigurationBase, IPageViewV2TableConfiguration
    {
        public PageViewTableConfiguration(IConfiguration configuration)
        {
            TableName = configuration.GetValue<string>(PageViewConstants.V2_TABLE_NAME_KEY, PageViewConstants.V2_TABLE_NAME_DEFAULT);

            if(string.IsNullOrWhiteSpace(TableName))
            {
                throw new ArgumentNullException(PageViewConstants.V2_TABLE_NAME_KEY);
            } // end if
        } // end method
    } // end class
} // end namespace