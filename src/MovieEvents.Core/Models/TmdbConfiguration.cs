namespace MovieEvents.Core.Models;

/// <summary>
/// Represents TMDb image configuration.
/// </summary>
public sealed class TmdbConfiguration
{
    /// <summary>Gets or sets the base URL for images.</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the available poster sizes.</summary>
    public List<string> PosterSizes { get; set; } = [];

    /// <summary>Gets or sets the available backdrop sizes.</summary>
    public List<string> BackdropSizes { get; set; } = [];
}
