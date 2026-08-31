using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MovieEvents.Core.Interfaces;
using MovieEvents.Infrastructure.Serialization;

namespace MovieEvents.Infrastructure.Storage;

/// <summary>
/// Browser local storage implementation using JS interop.
/// </summary>
public sealed class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<LocalStorageService> _logger;

    /// <summary>Initializes a new instance of <see cref="LocalStorageService"/>.</summary>
    public LocalStorageService(IJSRuntime jsRuntime, ILogger<LocalStorageService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _jsRuntime.InvokeAsync<string?>("localStorageInterop.getItem", cancellationToken, key);
        if (string.IsNullOrEmpty(json)) return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, AppJsonContext.Default.Options);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize value for key {Key}", key);
            return default;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, AppJsonContext.Default.Options);
        await _jsRuntime.InvokeVoidAsync("localStorageInterop.setItem", cancellationToken, key, json);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _jsRuntime.InvokeVoidAsync("localStorageInterop.removeItem", cancellationToken, key);
    }

    /// <inheritdoc />
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _jsRuntime.InvokeVoidAsync("localStorageInterop.clear", cancellationToken);
    }
}
