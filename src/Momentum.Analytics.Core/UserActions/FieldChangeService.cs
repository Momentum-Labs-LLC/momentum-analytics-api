using FluentResults;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.UserActions.Interfaces;

namespace Momentum.Analytics.Core.UserActions;

public class FieldChangeService : IFieldChangeService
{
    protected readonly IFieldChangeStorage _fieldChangeStorage;
    protected readonly ILogger _logger;

    public FieldChangeService(IFieldChangeStorage fieldChangeStorage, ILogger<FieldChangeService> logger)
    {
        _fieldChangeStorage = fieldChangeStorage ?? throw new ArgumentNullException(nameof(fieldChangeStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    } // end method

    public virtual Task<Result> RecordAsync(FieldChange fieldChange, CancellationToken cancellationToken = default)
    {
        return _fieldChangeStorage.InsertAsync(fieldChange, cancellationToken);
    } // end method

    public virtual Task<Result<FieldChange>> GetLatestAsync(Guid cookieId, CancellationToken cancellationToken = default)
    {
        return _fieldChangeStorage.GetLatestAsync(cookieId, cancellationToken);
    } // end method
} // end class