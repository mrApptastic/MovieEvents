using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Infrastructure.ApiClients;

/// <summary>
/// TMDb API client implementation.
/// </summary>
public sealed class TmdbApiClient : ITmdbApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TmdbOptions _options;
    private readonly ILogger<TmdbApiClient> _logger;

    /// <summary>Initializes a new instance of <see cref="TmdbApiClient"/>.</summary>
    public TmdbApiClient(HttpClient httpClient, IOptions<TmdbOptions> options, ILogger<TmdbApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<TmdbSearchResult>> SearchMoviesAsync(string query, int page = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"3/search/movie?api_key={_options.ApiKey}&query={Uri.EscapeDataString(query)}&page={page}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<TmdbSearchDto>(cancellationToken: cancellationToken);
            if (dto is null)
                return Result<TmdbSearchResult>.Failure("Failed to parse TMDb response.");

            var result = new TmdbSearchResult
            {
                Page = dto.Page,
                TotalPages = dto.TotalPages,
                TotalResults = dto.TotalResults,
                Results = dto.Results.Select(MapToMovie).ToList()
            };

            return Result<TmdbSearchResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TMDb search failed for query: {Query}", query);
            return Result<TmdbSearchResult>.Failure($"Search failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<Movie>> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"3/movie/{movieId}?api_key={_options.ApiKey}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<TmdbMovieDto>(cancellationToken: cancellationToken);
            if (dto is null)
                return Result<Movie>.Failure("Failed to parse TMDb response.");

            return Result<Movie>.Success(MapToMovie(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TMDb get details failed for movie: {MovieId}", movieId);
            return Result<Movie>.Failure($"Failed to get movie details: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<TmdbConfiguration>> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"3/configuration?api_key={_options.ApiKey}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<TmdbConfigDto>(cancellationToken: cancellationToken);
            if (dto?.Images is null)
                return Result<TmdbConfiguration>.Failure("Failed to parse TMDb configuration.");

            return Result<TmdbConfiguration>.Success(new TmdbConfiguration
            {
                BaseUrl = dto.Images.SecureBaseUrl,
                PosterSizes = dto.Images.PosterSizes,
                BackdropSizes = dto.Images.BackdropSizes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TMDb configuration fetch failed");
            return Result<TmdbConfiguration>.Failure($"Failed to get configuration: {ex.Message}");
        }
    }

    private static Movie MapToMovie(TmdbMovieDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title ?? string.Empty,
        Overview = dto.Overview ?? string.Empty,
        PosterPath = dto.PosterPath,
        BackdropPath = dto.BackdropPath,
        ReleaseDate = dto.ReleaseDate,
        VoteAverage = dto.VoteAverage,
        Genres = dto.Genres?.Select(g => g.Name).ToList() ?? []
    };

    // Internal DTOs for TMDb API responses
    private sealed class TmdbSearchDto
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

        [JsonPropertyName("results")]
        public List<TmdbMovieDto> Results { get; set; } = [];
    }

    private sealed class TmdbMovieDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenreDto>? Genres { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }
    }

    private sealed class TmdbGenreDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TmdbConfigDto
    {
        [JsonPropertyName("images")]
        public TmdbImagesConfigDto? Images { get; set; }
    }

    private sealed class TmdbImagesConfigDto
    {
        [JsonPropertyName("secure_base_url")]
        public string SecureBaseUrl { get; set; } = string.Empty;

        [JsonPropertyName("poster_sizes")]
        public List<string> PosterSizes { get; set; } = [];

        [JsonPropertyName("backdrop_sizes")]
        public List<string> BackdropSizes { get; set; } = [];
    }
}
