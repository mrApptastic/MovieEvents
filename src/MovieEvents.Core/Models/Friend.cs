namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a friend who can be invited to movie events.
/// </summary>
public sealed class Friend
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the friend's name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the friend's email address.</summary>
    public string Email { get; set; } = string.Empty;
}
