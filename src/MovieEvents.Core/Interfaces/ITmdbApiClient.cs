using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Client for The Movie Database API.
/// </summary>
public interface ITmdbApiClient
{
    /// <summary>Searches movies by query string.</summary>
    Task<Result<TmdbSearchResult>> SearchMoviesAsync(string query, int page = 1, CancellationToken cancellationToken = default);

    /// <summary>Gets movie details by TMDb ID.</summary>
    Task<Result<Movie>> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken = default);

    /// <summary>Gets the TMDb image configuration.</summary>
    Task<Result<TmdbConfiguration>> GetConfigurationAsync(CancellationToken cancellationToken = default);
}
