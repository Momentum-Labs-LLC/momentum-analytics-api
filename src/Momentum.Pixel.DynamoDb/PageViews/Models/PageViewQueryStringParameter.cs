using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Pixel.DynamoDb.PageViews.Models
{
    public class PageViewQueryStringParameter
    {
        public string RequestId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    } // end class
} // end namespace