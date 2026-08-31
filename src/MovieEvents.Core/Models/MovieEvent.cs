using MovieEvents.Core.Enums;

namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a planned movie event.
/// </summary>
public sealed class MovieEvent
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the movie for the event.</summary>
    public Movie Movie { get; set; } = new();

    /// <summary>Gets or sets the event date and time.</summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>Gets or sets the event location.</summary>
    public Location Location { get; set; } = new();

    /// <summary>Gets or sets the IDs of invited friends.</summary>
    public List<Guid> InvitedFriendIds { get; set; } = [];

    /// <summary>Gets or sets the IDs of invited groups.</summary>
    public List<Guid> InvitedGroupIds { get; set; } = [];

    /// <summary>Gets or sets optional event notes.</summary>
    public string? Notes { get; set; }

    /// <summary>Gets or sets the Google Calendar event ID.</summary>
    public string? CalendarEventId { get; set; }

    /// <summary>Gets or sets the event status.</summary>
    public EventStatus Status { get; set; } = EventStatus.Scheduled;

    /// <summary>Gets or sets when the event was created.</summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Gets or sets when the event was last updated.</summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
