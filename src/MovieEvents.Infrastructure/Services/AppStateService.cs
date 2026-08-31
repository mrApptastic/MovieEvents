using System.Text.Json;
using Microsoft.Extensions.Logging;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Core.Results;
using MovieEvents.Infrastructure.Serialization;

namespace MovieEvents.Infrastructure.Services;

/// <summary>
/// Manages application state persistence in local storage.
/// </summary>
public sealed class AppStateService : IAppStateService
{
    private const string StorageKey = "movieevents_appstate";
    private readonly ILocalStorageService _storage;
    private readonly ILogger<AppStateService> _logger;

    /// <summary>Initializes a new instance of <see cref="AppStateService"/>.</summary>
    public AppStateService(ILocalStorageService storage, ILogger<AppStateService> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AppState> GetStateAsync(CancellationToken cancellationToken = default)
    {
        var state = await _storage.GetAsync<AppState>(StorageKey, cancellationToken);
        return state ?? AppState.Create();
    }

    /// <inheritdoc />
    public async Task SaveStateAsync(AppState state, CancellationToken cancellationToken = default)
    {
        await _storage.SetAsync(StorageKey, state, cancellationToken);
        _logger.LogDebug("Application state saved");
    }

    /// <inheritdoc />
    public async Task<string> ExportStateAsync(CancellationToken cancellationToken = default)
    {
        var state = await GetStateAsync(cancellationToken);
        state.ExportedAt = DateTimeOffset.UtcNow;
        return JsonSerializer.Serialize(state, new JsonSerializerOptions(AppJsonContext.Default.Options)
        {
            WriteIndented = true
        });
    }

    /// <inheritdoc />
    public async Task<Result> ImportStateAsync(string json, CancellationToken cancellationToken = default)
    {
        try
        {
            var state = JsonSerializer.Deserialize<AppState>(json, AppJsonContext.Default.Options);
            if (state is null)
                return Result.Failure("Invalid JSON: deserialization returned null.");

            await SaveStateAsync(state, cancellationToken);
            _logger.LogInformation("Application state imported successfully");
            return Result.Success();
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to import application state");
            return Result.Failure($"Invalid JSON format: {ex.Message}");
        }
    }
}
