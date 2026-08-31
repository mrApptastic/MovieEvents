using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Manages persisted application state.
/// </summary>
public interface IAppStateService
{
    /// <summary>Loads the current application state from storage.</summary>
    Task<AppState> GetStateAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves the application state to storage.</summary>
    Task SaveStateAsync(AppState state, CancellationToken cancellationToken = default);

    /// <summary>Exports the application state as a JSON string.</summary>
    Task<string> ExportStateAsync(CancellationToken cancellationToken = default);

    /// <summary>Imports application state from a JSON string.</summary>
    Task<Result> ImportStateAsync(string json, CancellationToken cancellationToken = default);
}
