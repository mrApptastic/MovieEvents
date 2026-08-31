namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Manages UI theme preferences.
/// </summary>
public interface IThemeService
{
    /// <summary>Occurs when the theme changes.</summary>
    event Action<string>? OnThemeChanged;

    /// <summary>Gets the current theme name.</summary>
    Task<string> GetThemeAsync(CancellationToken cancellationToken = default);

    /// <summary>Sets and persists the theme.</summary>
    Task SetThemeAsync(string theme, CancellationToken cancellationToken = default);
}
