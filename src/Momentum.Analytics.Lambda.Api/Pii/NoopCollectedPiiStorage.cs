using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Lambda.Api.Pii
{
    public class NoopCollectedPiiStorage : ICollectedPiiStorage
    {
        public Task<IEnumerable<CollectedPii>?> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public Task InsertAysnc(CollectedPii collectedPii, CancellationToken token = default)
        {
            return Task.CompletedTask;
        } // end method
    } // end class
} // end namespace