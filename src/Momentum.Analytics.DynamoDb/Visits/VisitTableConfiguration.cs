using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public class VisitTableConfiguration : TableConfigurationBase, IVisitTableConfiguration
    {
        public string VisitStartIndex { get; protected set; }

        public string IdentifiedIndex { get; protected set; }

        public VisitTableConfiguration(IConfiguration configuration)
        {
            TableName = configuration.GetValue<string>(VisitConstants.TABLE_NAME, VisitConstants.TABLE_NAME_DEFAULT);

            if(string.IsNullOrWhiteSpace(TableName))
            {
                throw new ArgumentNullException(VisitConstants.TABLE_NAME);
            } // end if

            VisitStartIndex = configuration.GetValue<string>(VisitConstants.START_INDEX, VisitConstants.START_INDEX_DEFAULT);
            if(string.IsNullOrWhiteSpace(VisitStartIndex))
            {
                throw new ArgumentNullException(VisitConstants.START_INDEX);
            } // end if

            IdentifiedIndex = configuration.GetValue<string>(VisitConstants.IDENTIFIED_INDEX, VisitConstants.IDENTIFIED_INDEX_DEFAULT);
            if(string.IsNullOrWhiteSpace(IdentifiedIndex))
            {
                throw new ArgumentNullException(VisitConstants.IDENTIFIED_INDEX);
            } // end if
        } // end method
    } // end class
} // end namespace