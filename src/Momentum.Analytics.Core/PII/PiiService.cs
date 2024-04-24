using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PII
{
    public class PiiService<TPage, TCollectedPiiSearchResponse, TCollectedPiiStorage> : IPiiService
        where TCollectedPiiSearchResponse : class, ISearchResponse<CollectedPii, TPage>
        where TCollectedPiiStorage : ICollectedPiiStorage<TPage, TCollectedPiiSearchResponse>
    {
        protected readonly TCollectedPiiStorage _collectedPiiStorage;
        protected readonly IPiiValueStorage _piiValueStorage;
        protected readonly IEmailHasher _emailHasher;
        protected readonly ILogger _logger;

        public PiiService(
            TCollectedPiiStorage collectedPiiStorage,
            IPiiValueStorage piiValueStorage,
            IEmailHasher emailHasher,
            ILogger<PiiService<TPage, TCollectedPiiSearchResponse, TCollectedPiiStorage>> logger)
        {
            _collectedPiiStorage = collectedPiiStorage ?? throw new ArgumentNullException(nameof(collectedPiiStorage));
            _piiValueStorage = piiValueStorage ?? throw new ArgumentNullException(nameof(piiValueStorage));
            _emailHasher = emailHasher ?? throw new ArgumentNullException(nameof(emailHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task<PiiValue?> GetPiiAsync(Guid id, CancellationToken token = default)
        {
            return await _piiValueStorage.GetByIdAsync(id, token).ConfigureAwait(false);
        } // end method

        public async Task<PiiValue?> GetPiiAsync(string value, CancellationToken token = default)
        {
            return await _piiValueStorage.GetByValueAsync(value, token).ConfigureAwait(false);
        } // end method

        public async Task<IEnumerable<CollectedPii>> GetUniqueUserIdsAsync(Guid cookieId, int maximum = 10, CancellationToken token = default)
        {
            var result = new List<CollectedPii>();

            if(maximum > 0)
            {
                TCollectedPiiSearchResponse searchResponse = default;
                do
                {
                    TPage nextPage = default;
                    searchResponse = await _collectedPiiStorage
                            .GetLatestUserIdsAsync(cookieId, maximum, nextPage, token)
                            .ConfigureAwait(false);
                    
                    if(searchResponse != null)
                    {
                        nextPage = searchResponse.NextPage;
                        if(searchResponse.Data != null && searchResponse.Data.Any())
                        {
                            var uniqueUserIdsInPage = searchResponse.Data.GroupBy(x => x.PiiId)
                                // get the latest collected version of the pii
                                .Select(x => x.OrderByDescending(x => x.UtcTimestamp).First())
                                .ToList();

                            var newUniqueIds = uniqueUserIdsInPage
                                .Where(newId => !result.Any(knownId => knownId.PiiId.Equals(newId.PiiId)))
                                .Take(maximum - result.Count());
                            if(newUniqueIds.Any())
                            {
                                result.AddRange(newUniqueIds);
                            } // end if
                        } // end if
                    }
                    else
                    {
                        nextPage = default;
                    }
                } while(searchResponse != default && searchResponse.HasMore && result.Count < maximum);
            } // end if

            if(result.Any())
            {
                _logger.LogDebug("Found {0} user ids for cookie {1}.", result.Count(), cookieId);

                foreach(var collectedPii in result)
                {
                    var piiValue = await _piiValueStorage.GetByIdAsync(collectedPii.PiiId.Value, token).ConfigureAwait(false);
                    collectedPii.Pii = piiValue;
                } // end foreach
            } // end if            

            return result;
        } // end method

        public async Task RecordAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            if(collectedPii.Pii.PiiType == PiiTypeEnum.Email)
            {
                _logger.LogDebug("Hashing email provided as pii.");
                collectedPii.Pii.Value = await _emailHasher.HashEmailAsync(collectedPii.Pii.Value, token).ConfigureAwait(false);
            } // end if

            var pii = await _piiValueStorage.GetByValueAsync(collectedPii.Pii.Value, token).ConfigureAwait(false);
            if(pii != null)
            {
                _logger.LogDebug("Pii value already exists in the dataset.");
                collectedPii.Pii = pii;
                collectedPii.PiiId = pii.Id;
            }
            
            if(!collectedPii.PiiId.HasValue)
            {
                _logger.LogDebug("Pii value did not exist in the dataset.");
                var piiValue = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    Value = collectedPii.Pii.Value,
                    PiiType = collectedPii.Pii.PiiType,
                    UtcTimestamp = collectedPii.UtcTimestamp
                };
                await _piiValueStorage.InsertAsync(piiValue, token).ConfigureAwait(false);

                collectedPii.PiiId = piiValue.Id;
            } // end if

            await _collectedPiiStorage.InsertAysnc(collectedPii, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace