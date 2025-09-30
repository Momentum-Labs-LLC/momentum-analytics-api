using FluentResults;

namespace Momentum.Analytics.Core.UserActions.Interfaces;

public interface IFieldChangeService
{
    Task<Result> RecordAsync(FieldChange fieldChange, CancellationToken cancellationToken = default);
    Task<Result<FieldChange>> GetLatestAsync(Guid cookieId, CancellationToken cancellationToken = default);
} // end interface