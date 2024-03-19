namespace Momentum.Analytics.Core.PII.Interfaces
{
    public interface IEmailHasher
    {
        Task<string> HashEmailAsync(string email, CancellationToken token = default);
    } // end interface
} // end namespace