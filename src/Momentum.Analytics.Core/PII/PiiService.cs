using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PII
{
    public class PiiService : IPiiService
    {
        protected readonly ICollectedPiiStorage _collectedPiiStorage;
        protected readonly IPiiValueStorage _piiValueStorage;
        protected readonly IEmailHasher _emailHasher;
        protected readonly ILogger _logger;

        public PiiService(
            ICollectedPiiStorage collectedPiiStorage,
            IPiiValueStorage piiValueStorage,
            IEmailHasher emailHasher,
            ILogger<PiiService> logger)
        {
            _collectedPiiStorage = collectedPiiStorage ?? throw new ArgumentNullException(nameof(collectedPiiStorage));
            _piiValueStorage = piiValueStorage ?? throw new ArgumentNullException(nameof(piiValueStorage));
            _emailHasher = emailHasher ?? throw new ArgumentNullException(nameof(emailHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public Task<IEnumerable<PiiValue>> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public async Task<PiiValue?> GetPiiAsync(Guid id, CancellationToken token = default)
        {
            return await _piiValueStorage.GetByIdAsync(id, token).ConfigureAwait(false);
        } // end method

        public async Task<PiiValue?> GetPiiAsync(string value, CancellationToken token = default)
        {
            return await _piiValueStorage.GetByValueAsync(value, token).ConfigureAwait(false);
        } // end method

        public async Task RecordAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            if(collectedPii.Pii.PiiType == PiiTypeEnum.Email)
            {
                _logger.LogDebug("Hashing email provided as pii.");
                collectedPii.Pii.Value = await _emailHasher.HashEmailAsync(collectedPii.Pii.Value, token).ConfigureAwait(false);
            } // end mkethod

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