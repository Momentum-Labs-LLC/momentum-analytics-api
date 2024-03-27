using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Momentum.Analytics.Core.Visits
{
    public static class VisitConstants
    {
        public const string IS_SLIDING = "VISIT_IS_SLIDING";
        public const bool IS_SLIDING_DEFAULT = false;
        public const string WINDOW_LENGTH = "VISIT_LENGTH";

        /// <summary>
        /// Minutes
        /// </summary>
        public const int WINDOW_LENGTH_DEFAULT = 60*24; 
        
        public const string FIXED_VISIT_WINDOW_START = "FIXED_VISIT_WINDOW_START";

        /// <summary>
        /// MINUTES after midnight
        /// </summary>
        public const int FIXED_VISIT_WINDOW_START_DEFAULT = 0;

        public const string TIMEZONE = "VISIT_TIMEZONE";
        public const string TIMEZONE_DEFAULT = "America/New_York";

        public const string LOCAL_CACHE_EXPIRATION = "VISIT_LOCAL_CACHE_EXP";

        /// <summary>
        /// Seconds
        /// </summary>
        public const int LOCAL_CACHE_EXPIRATION_DEFAULT = 10;
    } // end class
} // end namespace