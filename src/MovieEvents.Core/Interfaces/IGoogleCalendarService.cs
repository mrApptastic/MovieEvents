using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Manages Google Calendar events.
/// </summary>
public interface IGoogleCalendarService
{
    /// <summary>Creates a calendar event and returns the calendar event ID.</summary>
    Task<Result<string>> CreateEventAsync(MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default);

    /// <summary>Deletes a calendar event.</summary>
    Task<Result> DeleteEventAsync(string calendarEventId, CancellationToken cancellationToken = default);

    /// <summary>Updates a calendar event.</summary>
    Task<Result> UpdateEventAsync(string calendarEventId, MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default);
}
