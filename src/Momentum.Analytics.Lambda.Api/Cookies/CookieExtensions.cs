using Microsoft.Extensions.Primitives;
using Momentum.Analytics.Core.PII.Models;
using NodaTime;

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
                    $"{CookieConstants.VISIT_EXPIRATION}{CookieConstants.COOKIE_VALUE_DELIMITER}{cookie.VisitExpiration.ToUnixTimeMilliseconds()}", 
                    $"{CookieConstants.MAX_FUNNEL_STEP}{CookieConstants.COOKIE_VALUE_DELIMITER}{cookie.MaxFunnelStep}", 
                    $"{CookieConstants.PII_BITMAP}{CookieConstants.COOKIE_VALUE_DELIMITER}{(int)cookie.CollectedPii}",
                    $"{CookieConstants.USER_ID}{CookieConstants.COOKIE_VALUE_DELIMITER}{cookie.UserId}"
                ]);
        } // end method

        public static Cookie ToCookieModel(this string headerValue, Instant visitExpiration)
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
                    VisitExpiration = visitExpiration,
                    CollectedPii = (PiiTypeEnum)cookieDict.ReadInteger(CookieConstants.PII_BITMAP),
                    MaxFunnelStep = cookieDict.ReadInteger(CookieConstants.MAX_FUNNEL_STEP),
                    UserId = cookieDict.ReadString(CookieConstants.USER_ID)
                };
            }
            else
            {
                result = new Cookie()
                {
                    Id = Guid.NewGuid(),
                    VisitExpiration = visitExpiration,
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

        public static string? ReadString(this Dictionary<String, string> dictionary, string key, string? defaultValue = null)
        {
            var result = defaultValue;

            if(dictionary.TryGetValue(key, out string tmpValue)
                && !string.IsNullOrWhiteSpace(tmpValue))
            {
                result = tmpValue;
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace