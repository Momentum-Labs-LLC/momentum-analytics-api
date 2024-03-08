namespace Momentum.Analytics.Core.PageViews.Models
{
    /// <summary>
    /// A class for capturing urchin tracking parameters.
    /// https://en.wikipedia.org/wiki/UTM_parameters
    /// </summary>
    public class UrchinTrackingParameter
    {
        /// <summary>
        /// Specifies the parameter.
        /// </summary>
        public UrchinParameterEnum Parameter { get; set; }

        /// <summary>
        /// The value of the parameter.
        /// </summary>
        public string Value { get; set; }
    } // end class
} // end namespace