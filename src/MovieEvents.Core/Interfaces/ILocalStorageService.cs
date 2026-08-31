namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Provides access to browser local storage.
/// </summary>
public interface ILocalStorageService
{
    /// <summary>Gets a value from local storage.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>Sets a value in local storage.</summary>
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>Removes a value from local storage.</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>Clears all local storage.</summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
