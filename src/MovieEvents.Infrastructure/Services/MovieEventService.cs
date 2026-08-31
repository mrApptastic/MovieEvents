using Microsoft.Extensions.Logging;
using MovieEvents.Core.Enums;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Infrastructure.Services;

/// <summary>
/// Orchestrates movie event creation and cancellation.
/// </summary>
public sealed class MovieEventService : IMovieEventService
{
    private readonly IAppStateService _appStateService;
    private readonly IGoogleCalendarService _calendarService;
    private readonly IGmailService _gmailService;
    private readonly ILogger<MovieEventService> _logger;

    /// <summary>Initializes a new instance of <see cref="MovieEventService"/>.</summary>
    public MovieEventService(
        IAppStateService appStateService,
        IGoogleCalendarService calendarService,
        IGmailService gmailService,
        ILogger<MovieEventService> logger)
    {
        _appStateService = appStateService;
        _calendarService = calendarService;
        _gmailService = gmailService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> CreateEventAsync(MovieEvent movieEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var state = await _appStateService.GetStateAsync(cancellationToken);
            var friends = ResolveInvitedFriends(state, movieEvent);

            // Create Google Calendar event
            var calResult = await _calendarService.CreateEventAsync(movieEvent, friends, cancellationToken);
            if (calResult.IsSuccess && calResult.Value is not null)
            {
                movieEvent.CalendarEventId = calResult.Value;
            }
            else
            {
                _logger.LogWarning("Calendar event creation failed, continuing: {Error}", calResult.Error);
            }

            // Add event to state
            state.Club.AddEvent(movieEvent);
            await _appStateService.SaveStateAsync(state, cancellationToken);

            // Send invitation emails
            if (friends.Count > 0)
            {
                var emailResult = await _gmailService.SendInvitationAsync(movieEvent, friends, cancellationToken);
                if (!emailResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to send invitation emails: {Error}", emailResult.Error);
                }
            }

            _logger.LogInformation("Created event {EventId} for movie {MovieTitle}", movieEvent.Id, movieEvent.Movie.Title);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create event");
            return Result.Failure($"Failed to create event: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> CancelEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        try
        {
            var state = await _appStateService.GetStateAsync(cancellationToken);
            var movieEvent = state.Club.GetEvent(eventId);

            if (movieEvent is null)
                return Result.Failure("Event not found.");

            movieEvent.Status = EventStatus.Cancelled;
            movieEvent.UpdatedAt = DateTimeOffset.UtcNow;

            var friends = ResolveInvitedFriends(state, movieEvent);

            // Delete from Google Calendar
            if (!string.IsNullOrEmpty(movieEvent.CalendarEventId))
            {
                var deleteResult = await _calendarService.DeleteEventAsync(movieEvent.CalendarEventId, cancellationToken);
                if (!deleteResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete calendar event: {Error}", deleteResult.Error);
                }
            }

            // Send cancellation emails
            if (friends.Count > 0)
            {
                var emailResult = await _gmailService.SendCancellationAsync(movieEvent, friends, cancellationToken);
                if (!emailResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to send cancellation emails: {Error}", emailResult.Error);
                }
            }

            await _appStateService.SaveStateAsync(state, cancellationToken);
            _logger.LogInformation("Cancelled event {EventId}", eventId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel event {EventId}", eventId);
            return Result.Failure($"Failed to cancel event: {ex.Message}");
        }
    }

    private static List<Friend> ResolveInvitedFriends(AppState state, MovieEvent movieEvent)
    {
        var friendIds = new HashSet<Guid>(movieEvent.InvitedFriendIds);

        // Add friends from invited groups
        foreach (var groupId in movieEvent.InvitedGroupIds)
        {
            var group = state.Club.GetGroup(groupId);
            if (group is not null)
            {
                foreach (var friendId in group.FriendIds)
                {
                    friendIds.Add(friendId);
                }
            }
        }

        return friendIds
            .Select(id => state.Club.GetFriend(id))
            .Where(f => f is not null)
            .Cast<Friend>()
            .ToList();
    }
}
