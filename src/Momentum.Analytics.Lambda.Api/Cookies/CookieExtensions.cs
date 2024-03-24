using Microsoft.Extensions.Primitives;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public static class CookieExtensions 
    {
        public static double MillisecondsSinceEpoch(this DateTime datetime)
        {
            return (datetime - DateTime.UnixEpoch).TotalMilliseconds;
        } // end method

        public static DateTime ToDateTime(this double msSinceEpoch)
        {
            return DateTime.UnixEpoch.AddMilliseconds(msSinceEpoch);
        } // end method

        public static StringValues ToHeaderValue(this Cookie cookie)
        {
            return new StringValues(
                [
                    $"{CookieConstants.COOKIE_ID}{CookieConstants.COOKIE_VALUE_DELIMITER}{cookie.Id}", 
                    $"{CookieConstants.VISIT_EXPIRATION}{CookieConstants.COOKIE_VALUE_DELIMITER}{DateTime.UtcNow.Date.MillisecondsSinceEpoch()}", 
                    $"{CookieConstants.MAX_FUNNEL_STEP}{CookieConstants.COOKIE_VALUE_DELIMITER}{cookie.MaxFunnelStep}", 
                    $"{CookieConstants.PII_BITMAP}{CookieConstants.COOKIE_VALUE_DELIMITER}{(int)cookie.CollectedPii}"
                ]);
        } // end method

        public static Cookie ToCookieModel(this string headerValue)
        {
            Cookie? result = null;

            if(!string.IsNullOrWhiteSpace(headerValue))
            {
                var cookieDict = headerValue.Split(",").ToDictionary(
                        x => x.Split(CookieConstants.COOKIE_VALUE_DELIMITER)[0], 
                        x => x.Split(CookieConstants.COOKIE_VALUE_DELIMITER)[1]);

                result = new Cookie()
                {
                    Id = Guid.Parse(cookieDict[CookieConstants.COOKIE_ID]),
                    VisitExpiration = cookieDict.ReadDouble(CookieConstants.VISIT_EXPIRATION).ToDateTime(),
                    CollectedPii = (PiiTypeEnum)cookieDict.ReadInteger(CookieConstants.PII_BITMAP),
                    MaxFunnelStep = cookieDict.ReadInteger(CookieConstants.MAX_FUNNEL_STEP)
                };
            }
            else
            {
                result = new Cookie()
                {
                    Id = Guid.NewGuid(),
                    VisitExpiration = DateTime.UtcNow.Date,
                    CollectedPii = 0,
                    MaxFunnelStep = 0
                };
            } // end if

            return result;
        } // end method

        public static int ReadInteger(this Dictionary<string, string> dictionary, string key, int defaultValue = 0)
        {
            int result = defaultValue;
            if(int.TryParse(dictionary.GetValueOrDefault(key), out int tmpValue)
                && tmpValue != defaultValue)
            {
                result = tmpValue;
            } // end if

            return result;
        } // end method
        
        public static double ReadDouble(this Dictionary<string, string> dictionary, string key, double defaultValue = 0)
        {
            double result = defaultValue;
            if(double.TryParse(dictionary.GetValueOrDefault(key), out double tmpValue)
                && tmpValue != defaultValue)
            {
                result = tmpValue;
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace