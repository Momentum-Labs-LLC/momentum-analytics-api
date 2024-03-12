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
        protected readonly ILogger _logger;

        public PiiService(
            ICollectedPiiStorage collectedPiiStorage,
            IPiiValueStorage piiValueStorage,
            ILogger<PiiService> logger)
        {
            _collectedPiiStorage = collectedPiiStorage ?? throw new ArgumentNullException(nameof(collectedPiiStorage));
            _piiValueStorage = piiValueStorage ?? throw new ArgumentNullException(nameof(piiValueStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public Task<IEnumerable<PiiValue>> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public Task<PiiValue?> GetPiiAsync(string value, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public Task RecordAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method
    } // end class
} // end namespace