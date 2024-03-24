using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Lambda.Api.Pii
{
    public class NoopPiiValueStorage : IPiiValueStorage
    {
        public Task<PiiValue?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public async Task<PiiValue?> GetByValueAsync(string value, CancellationToken token = default)
        {
            return null;
        } // end method

        public Task InsertAsync(PiiValue piiValue, CancellationToken token = default)
        {
            return Task.CompletedTask;
        } // end method
    } // end class
} // end namespace