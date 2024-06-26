using Amazon.S3;

namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces
{
    public interface IS3ClientFactory
    {
        Task<IAmazonS3> GetAsync(CancellationToken token = default);
    } // end interface
} // end namespace