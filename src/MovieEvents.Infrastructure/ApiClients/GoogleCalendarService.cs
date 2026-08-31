using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Core.Results;
using MovieEvents.Infrastructure.Authentication;

namespace MovieEvents.Infrastructure.ApiClients;

/// <summary>
/// Google Calendar API v3 client.
/// </summary>
public sealed class GoogleCalendarService : IGoogleCalendarService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<GoogleCalendarService> _logger;

    /// <summary>Initializes a new instance of <see cref="GoogleCalendarService"/>.</summary>
    public GoogleCalendarService(HttpClient httpClient, ITokenProvider tokenProvider, ILogger<GoogleCalendarService> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<string>> CreateEventAsync(MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthHeaderAsync(cancellationToken);
            var calEvent = BuildCalendarEvent(movieEvent, friends);
            var response = await _httpClient.PostAsJsonAsync(
                "https://www.googleapis.com/calendar/v3/calendars/primary/events?sendUpdates=all",
                calEvent, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CalendarEventResponse>(cancellationToken: cancellationToken);
            var eventId = result?.Id ?? string.Empty;
            _logger.LogInformation("Created calendar event {EventId}", eventId);
            return Result<string>.Success(eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create calendar event");
            return Result<string>.Failure($"Failed to create calendar event: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteEventAsync(string calendarEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthHeaderAsync(cancellationToken);
            var response = await _httpClient.DeleteAsync(
                $"https://www.googleapis.com/calendar/v3/calendars/primary/events/{calendarEventId}?sendUpdates=all",
                cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Deleted calendar event {EventId}", calendarEventId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete calendar event {EventId}", calendarEventId);
            return Result.Failure($"Failed to delete calendar event: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> UpdateEventAsync(string calendarEventId, MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthHeaderAsync(cancellationToken);
            var calEvent = BuildCalendarEvent(movieEvent, friends);
            var response = await _httpClient.PutAsJsonAsync(
                $"https://www.googleapis.com/calendar/v3/calendars/primary/events/{calendarEventId}?sendUpdates=all",
                calEvent, cancellationToken);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Updated calendar event {EventId}", calendarEventId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update calendar event {EventId}", calendarEventId);
            return Result.Failure($"Failed to update calendar event: {ex.Message}");
        }
    }

    private async Task SetAuthHeaderAsync(CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static object BuildCalendarEvent(MovieEvent movieEvent, List<Friend> friends) => new
    {
        summary = $"Movie: {movieEvent.Movie.Title}",
        location = movieEvent.Location.Address,
        description = BuildDescription(movieEvent),
        start = new { dateTime = movieEvent.EventDate.ToString("o"), timeZone = "UTC" },
        end = new { dateTime = movieEvent.EventDate.AddHours(3).ToString("o"), timeZone = "UTC" },
        attendees = friends.Select(f => new { email = f.Email }).ToArray()
    };

    private static string BuildDescription(MovieEvent movieEvent)
    {
        var desc = $"Movie: {movieEvent.Movie.Title}";
        if (!string.IsNullOrEmpty(movieEvent.Notes))
            desc += $"\nNotes: {movieEvent.Notes}";
        return desc;
    }

    private sealed class CalendarEventResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
