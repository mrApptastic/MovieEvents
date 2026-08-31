using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Orchestrates movie event creation and cancellation.
/// </summary>
public interface IMovieEventService
{
    /// <summary>Creates a movie event, calendar entry, and sends invitations.</summary>
    Task<Result> CreateEventAsync(MovieEvent movieEvent, CancellationToken cancellationToken = default);

    /// <summary>Cancels an event, removes calendar entry, and sends cancellations.</summary>
    Task<Result> CancelEventAsync(Guid eventId, CancellationToken cancellationToken = default);
}
