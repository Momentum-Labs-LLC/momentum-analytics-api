using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.PageViews.Interfaces;

namespace Momentum.Analytics.DynamoDb.PageViews
{
    public class PageViewTableConfiguration : TableConfigurationBase, IPageViewTableConfiguration
    {
        public PageViewTableConfiguration(IConfiguration configuration)
        {
            TableName = configuration.GetValue<string>(PageViewConstants.TABLE_NAME_KEY, PageViewConstants.TABLE_NAME_DEFAULT);

            if(string.IsNullOrWhiteSpace(TableName))
            {
                throw new ArgumentNullException(PageViewConstants.TABLE_NAME_KEY);
            } // end if
        } // end method
    } // end class
} // end namespace