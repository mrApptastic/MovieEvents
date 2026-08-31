namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a paginated TMDb movie search result.
/// </summary>
public sealed class TmdbSearchResult
{
    /// <summary>Gets or sets the current page number.</summary>
    public int Page { get; set; }

    /// <summary>Gets or sets the total number of pages.</summary>
    public int TotalPages { get; set; }

    /// <summary>Gets or sets the total number of results.</summary>
    public int TotalResults { get; set; }

    /// <summary>Gets or sets the movie results for this page.</summary>
    public List<Movie> Results { get; set; } = [];
}
