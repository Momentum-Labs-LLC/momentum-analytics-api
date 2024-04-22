using Microsoft.Extensions.Configuration;

namespace Momentum.Analytics.Processing.Cookies
{
    public class SharedCookieConfiguration : ISharedCookieConfiguration
    {
        public int Threshold { get; protected set; }

        public SharedCookieConfiguration(IConfiguration configuration)
        {
            Threshold = configuration.GetValue<int>(SharedCookieConstants.SHARED_COOKIE_THRESHOLD, SharedCookieConstants.SHARED_COOKIE_THRESHOLD_DEFAULT);

            if(Threshold < 2) 
            {
                throw new ArgumentOutOfRangeException(SharedCookieConstants.SHARED_COOKIE_THRESHOLD);
            } // end if
        } // end method
    } // end class
} // end namespace