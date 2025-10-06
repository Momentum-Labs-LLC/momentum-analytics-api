using FluentResults;

namespace Momentum.Analytics.Core.UserActions.Interfaces;

public interface IFieldChangeStorage
{
    Task<Result> InsertAsync(FieldChange fieldChange, CancellationToken cancellationToken = default);
    Task<Result<FieldChange>> GetLatestAsync(Guid cookieId, CancellationToken cancellationToken = default);
} // end interface