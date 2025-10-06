using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using NodaTime;

namespace Momentum.Analytics.DynamoDb.Visits.Interfaces
{
    public interface IDynamoDbVisitStorage : 
        IVisitStorage<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>>
    {
        Task<IDynamoSearchResponse<Visit>> GetIdentifiedAsync(
            Instant hour, 
            Dictionary<string, AttributeValue>? page,
            CancellationToken token = default);

        Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            Instant hour,
            Dictionary<string, AttributeValue>? page,
            CancellationToken token = default);
    } // end interface
} // end namespace