using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MovieEvents.Core.Interfaces;

namespace MovieEvents.Infrastructure.Services;

/// <summary>
/// Manages dark/light theme preferences.
/// </summary>
public sealed class ThemeService : IThemeService
{
    private const string StorageKey = "movieevents_theme";
    private const string DefaultTheme = "light";
    private readonly ILocalStorageService _storage;
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ThemeService> _logger;

    /// <inheritdoc />
    public event Action<string>? OnThemeChanged;

    /// <summary>Initializes a new instance of <see cref="ThemeService"/>.</summary>
    public ThemeService(ILocalStorageService storage, IJSRuntime jsRuntime, ILogger<ThemeService> logger)
    {
        _storage = storage;
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> GetThemeAsync(CancellationToken cancellationToken = default)
    {
        var theme = await _storage.GetAsync<string>(StorageKey, cancellationToken);
        return theme ?? DefaultTheme;
    }

    /// <inheritdoc />
    public async Task SetThemeAsync(string theme, CancellationToken cancellationToken = default)
    {
        await _storage.SetAsync(StorageKey, theme, cancellationToken);
        await _jsRuntime.InvokeVoidAsync("setTheme", cancellationToken, theme);
        _logger.LogDebug("Theme changed to {Theme}", theme);
        OnThemeChanged?.Invoke(theme);
    }
}
