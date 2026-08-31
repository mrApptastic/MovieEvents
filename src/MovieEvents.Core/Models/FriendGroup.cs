namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a reusable group of friends.
/// </summary>
public sealed class FriendGroup
{
    /// <summary>
    /// Gets or sets the unique identifier for the friend group.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the display name for the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifiers of the friends included in the group.
    /// </summary>
    public List<Guid> FriendIds { get; set; } = [];
}
