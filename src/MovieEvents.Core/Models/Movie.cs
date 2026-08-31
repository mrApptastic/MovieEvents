namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a movie sourced from TMDb.
/// </summary>
public sealed class Movie
{
    /// <summary>Gets or sets the TMDb movie identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the movie title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the movie overview.</summary>
    public string Overview { get; set; } = string.Empty;

    /// <summary>Gets or sets the poster path from TMDb.</summary>
    public string? PosterPath { get; set; }

    /// <summary>Gets or sets the release date string.</summary>
    public string? ReleaseDate { get; set; }

    /// <summary>Gets or sets the vote average.</summary>
    public double VoteAverage { get; set; }

    /// <summary>Gets or sets the genre names.</summary>
    public List<string> Genres { get; set; } = [];

    /// <summary>Gets or sets the backdrop path from TMDb.</summary>
    public string? BackdropPath { get; set; }

    /// <summary>Builds the full poster URL.</summary>
    public string GetPosterUrl(string baseUrl, string size) =>
        PosterPath is not null ? $"{baseUrl}{size}{PosterPath}" : string.Empty;

    /// <summary>Builds the full backdrop URL.</summary>
    public string GetBackdropUrl(string baseUrl, string size) =>
        BackdropPath is not null ? $"{baseUrl}{size}{BackdropPath}" : string.Empty;
}
