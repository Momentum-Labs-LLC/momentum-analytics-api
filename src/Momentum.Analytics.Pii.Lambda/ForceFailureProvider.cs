using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Momentum.Analytics.Pii.Lambda
{
    public interface IForceFailureProvider
    {
        bool ShouldForceFailure { get; }
    }
    public class ForceFailureProvider : IForceFailureProvider
    {
        public const string FORCE_FAILURE_KEY = "FORCE_FAILURE";
        public const bool FORCE_FAILURE_DEFAULT = false;
        public bool ShouldForceFailure { get; protected set; }

        public ForceFailureProvider(IConfiguration configuration)
        {
            ShouldForceFailure = configuration.GetValue<bool>(FORCE_FAILURE_KEY, FORCE_FAILURE_DEFAULT);
        } // end method
    } // end class
} // end namespace