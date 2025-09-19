namespace Momentum.Analytics.Core.PII.Models
{
    [Flags]
    public enum PiiTypeEnum
    {
        None = 0,
        UserId = 1,
        Email = 2,
        AppointmentId = 4
    } // end enum
} // end namespace