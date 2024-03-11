using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface IUserActivity
    {
        Guid CookieId { get; }
        DateTime UtcTimestamp { get; }
    } // end interface
} // end namespace