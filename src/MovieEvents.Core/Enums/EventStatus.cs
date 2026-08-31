namespace MovieEvents.Core.Enums;

/// <summary>
/// Represents the lifecycle state of a movie event.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// The movie event is scheduled and active.
    /// </summary>
    Scheduled,

    /// <summary>
    /// The movie event has been cancelled.
    /// </summary>
    Cancelled,
}
