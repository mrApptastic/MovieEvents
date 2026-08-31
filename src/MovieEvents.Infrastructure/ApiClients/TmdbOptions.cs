namespace MovieEvents.Infrastructure.ApiClients;

/// <summary>
/// Configuration options for the TMDb API.
/// </summary>
public sealed class TmdbOptions
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "Tmdb";

    /// <summary>Gets or sets the TMDb API key.</summary>
    public string ApiKey { get; set; } = string.Empty;
}
