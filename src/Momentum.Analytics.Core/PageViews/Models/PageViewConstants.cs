namespace Momentum.Analytics.Core.PageViews.Models
{
    public static class PageViewConstants
    {
        public static Dictionary<string, UrchinParameterEnum> UTM_PARAMETERS = 
            new Dictionary<string, UrchinParameterEnum>(StringComparer.OrdinalIgnoreCase)
            {
                { "utm_source", UrchinParameterEnum.Source },
                { "utm_medium", UrchinParameterEnum.Medium },
                { "utm_campaign", UrchinParameterEnum.Campaign },
                { "utm_term", UrchinParameterEnum.Term },
                { "utm_content", UrchinParameterEnum.Content }
            };
    } // end class
} // end namespace