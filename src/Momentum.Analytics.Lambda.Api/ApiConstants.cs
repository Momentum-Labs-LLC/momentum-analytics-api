using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.Lambda.Api
{
    public static class ApiConstants
    {
        public static byte[] GIF_BYTES = Convert.FromBase64String("R0lGODlhAQABAAAAACwAAAAAAQABAAA=");
        public const string GIF_MEME_TYPE = "image/gif";
        public const string PIXEL_PATH = "1x1.gif";
        public const string QUERY_STRING_EVENT_KEY = "e";
    } // end class
} // end namespace