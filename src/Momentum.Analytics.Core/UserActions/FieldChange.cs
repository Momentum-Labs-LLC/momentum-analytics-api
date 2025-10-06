using Momentum.Analytics.Core.Models;

namespace Momentum.Analytics.Core.UserActions;

public class FieldChange : UserActivity
{
    /// <summary>
    /// The url of the page that the field change occurred on.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The field that was changed.
    /// This should be an id or name.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// The value that the field was changed to.
    /// </summary>
    public string Value { get; set; } = string.Empty;
} // end class